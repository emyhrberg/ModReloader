using System.Reflection;
using System.Runtime.Loader;

const BindingFlags All = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
string tmlDirectory = Path.GetFullPath(args[0]);
string modDirectory = Path.GetFullPath(args[1]);
string assemblyPath = Path.GetFullPath(args[2]);
string[] dependencies = Directory.GetFiles(Path.Combine(tmlDirectory, "Libraries"), "*.dll", SearchOption.AllDirectories)
    .Concat(Directory.GetFiles(Path.Combine(modDirectory, "lib"), "*.dll")).ToArray();
AssemblyLoadContext.Default.Resolving += (context, name) =>
{
    string platform = OperatingSystem.IsWindows() ? "win" : OperatingSystem.IsMacOS() ? "osx" : "linux";
    string[] matches = dependencies.Where(p => Path.GetFileNameWithoutExtension(p) == name.Name).ToArray();
    string path = matches.FirstOrDefault(p => p.Replace('\\', '/').Contains($"/runtimes/{platform}/lib/"))
        ?? matches.FirstOrDefault(p => !p.Replace('\\', '/').Contains("/runtimes/"));
    return path == null ? null : context.LoadFromAssemblyPath(path);
};
Assembly tml = Assembly.LoadFrom(Path.Combine(tmlDirectory, "tModLoader.dll"));
tml.GetType("Terraria.Program").GetField("SavePath", All).SetValue(null, AppContext.BaseDirectory);
Assembly mod = Assembly.LoadFrom(assemblyPath);
Type main = tml.GetType("Terraria.Main", true);
main.GetField("dedServ", All).SetValue(null, true); // No graphics or game launch.
main.GetField("netMode", All).SetValue(null, 1); // Exercise client unload, without writing player data.
Type loader = tml.GetType("Terraria.ModLoader.ModLoader", true);
Type modNet = tml.GetType("Terraria.ModLoader.ModNet", true);
FieldInfo callbacks = loader.GetField("OnSuccessfulLoad", All);
FieldInfo netReloadActive = modNet.GetField("NetReloadActive", All);
Type systemType = mod.GetType("ModReloader.Core.Features.Reload.AutoloadPlayerInWorldSystem", true);
object system = Activator.CreateInstance(systemType);
MethodInfo unload = systemType.GetMethod("Unload");
MethodInfo onLoad = systemType.GetMethod("OnModLoad");
Action singlePlayer = systemType.GetMethod("EnterSingleplayerWorld", Type.EmptyTypes).CreateDelegate<Action>();
Action multiplayer = systemType.GetMethod("EnterMultiplayerWorld", Type.EmptyTypes).CreateDelegate<Action>();
Type configType = mod.GetType("ModReloader.Common.Configs.Config", true);
object config = Activator.CreateInstance(configType);
configType.GetField("AutoJoinWorld").SetValue(config, true);
configType.GetField("LogDebugMessages").SetValue(config, false);
tml.GetType("Terraria.ModLoader.ContentInstance").GetMethod("Register", All).Invoke(null, [config]);
Type storage = mod.GetType("ModReloader.Core.Features.Reload.ClientDataMemoryStorage", true);
PropertyInfo clientMode = storage.GetProperty("ClientMode");
int checks = 0;
void Check(bool passed, string name)
{
    if (!passed) throw new Exception("FAIL: " + name);
    checks++;
    Console.WriteLine("PASS: " + name);
}
// Use tML's actual server-join continuation, without invoking it or opening sockets.
Action continuation = (Action)modNet.GetMethod("NetReload", All).Invoke(null, null);
int otherCalls = 0;
Action otherMod = () => otherCalls++;
try
{
    callbacks.SetValue(null, continuation + singlePlayer + otherMod + multiplayer);
    unload.Invoke(system, null);
    var remaining = ((Action)callbacks.GetValue(null))?.GetInvocationList() ?? [];
    Check(remaining.SequenceEqual(new Delegate[] { continuation, otherMod }),
        "unload removes only Mod Reloader callbacks and preserves the actual tML join continuation");
    unload.Invoke(system, null);
    Check(((Action)callbacks.GetValue(null)).GetInvocationList().SequenceEqual(remaining), "repeated unload preserves external callbacks");

    foreach (string mode in new[] { "SinglePlayer", "MPMajor", "MPMinor", "FreshClient" })
    {
        clientMode.SetValue(null, Enum.Parse(clientMode.PropertyType, mode));
        netReloadActive.SetValue(null, true);
        onLoad.Invoke(system, null);
        Check(((Action)callbacks.GetValue(null)).GetInvocationList().SequenceEqual(remaining),
            "server-required reload does not enqueue a second join in " + mode);
    }

    foreach (var (mode, expected) in new[] { ("SinglePlayer", singlePlayer), ("MPMajor", multiplayer), ("MPMinor", multiplayer) })
    {
        callbacks.SetValue(null, otherMod);
        netReloadActive.SetValue(null, false);
        clientMode.SetValue(null, Enum.Parse(clientMode.PropertyType, mode));
        onLoad.Invoke(system, null);
        Check(((Action)callbacks.GetValue(null)).GetInvocationList().SequenceEqual(new Delegate[] { otherMod, expected }),
            "ordinary developer reload still auto-joins in " + mode);
        unload.Invoke(system, null);
        ((Action)callbacks.GetValue(null))();
        Check(((Action)callbacks.GetValue(null)) == otherMod, "ordinary reload cleanup leaves another mod's callback intact");
    }
    callbacks.SetValue(null, null);
    unload.Invoke(system, null);
    Check(callbacks.GetValue(null) == null, "unload accepts an empty callback list");
    Check(otherCalls == 3, "other mod callbacks remain callable");
    Console.WriteLine($"{checks} reload callback regression checks passed. No game or network connection was started.");
}
finally
{
    callbacks.SetValue(null, null);
    netReloadActive.SetValue(null, false);
    storage.GetMethod("ClearData").Invoke(null, null);
}
