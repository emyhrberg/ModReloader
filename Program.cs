using Microsoft.Xna.Framework;
using MonoMod.RuntimeDetour;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace ModHelper
{
    internal static class Program
    {
        // --------------------------------------------------------------------- //
        //  Entry points                                                         //
        // --------------------------------------------------------------------- //

        public static void Main(string[] args) => Run(args, 0);  // main client / mod loader
        public static void LaunchClient(string[] args) => Run(args, 1);  // extra client
        public static void LaunchServer(string[] args) => Run(args, -1); // dedicated server

        // --------------------------------------------------------------------- //
        //  Runtime flags                                                        //
        // --------------------------------------------------------------------- //

        public static bool IsClientEx { get; private set; }
        public static bool IsServer { get; private set; }
        public static bool IsMod { get; private set; }

        // --------------------------------------------------------------------- //
        //  Implementation                                                       //
        // --------------------------------------------------------------------- //

        private static void Run(string[] args, int clientIndex)
        {
            string file = args.FirstOrDefault();
            Console.WriteLine($"tML executable: {file}");
            if (string.IsNullOrWhiteSpace(file) || !File.Exists(file))
            {
                Console.WriteLine("Missing tML path, does not exist, or is not accessible.");
                return;
            }

            Environment.CurrentDirectory = Path.GetDirectoryName(Path.GetFullPath(file));

            IsMod = clientIndex == 0;
            IsServer = clientIndex == -1;
            IsClientEx = clientIndex > 0;

            // Kick off detours in the background
            _detourTask = Task.Run(ApplyDetours)
                              .ContinueWith(t => Console.WriteLine($"Finished applying detours in {_sw.Elapsed}"));

            // Compose argv for Terraria.Program.Main
            string[] mainArgs = args;
            if (IsServer) mainArgs = new[] { "-server" }.Concat(args).ToArray();
            else
                mainArgs = new[] { "-console" }.Concat(args).ToArray();

            typeof(ModLoader).Assembly.EntryPoint!
                              .Invoke(null, new object[] { mainArgs });
        }

        // --------------------------------------------------------------------- //
        //  Detours & IL‑hooks                                                   //
        // --------------------------------------------------------------------- //

        public static event Action<List<Hook>> RegisterDetours;  // external mods may add their own

        private static readonly List<Hook> _hooks = new(10);
        private static readonly List<ILHook> _ilHooks = new(10);
        private static Task _detourTask;
        private static Stopwatch _sw;

        private const BindingFlags FINSTANCE = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private const BindingFlags FSTATIC = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void ApplyDetours()
        {
            _sw = Stopwatch.StartNew();
            Assembly tmlAsm = typeof(ModLoader).Assembly;

            Type AssemblyManager = tmlAsm.GetType("Terraria.ModLoader.Core.AssemblyManager");
            Type ProgramT = tmlAsm.GetType("Terraria.Program");
            Type MainT = typeof(Main);

            // 1.  Disable most of tML’s expensive assembly checks / JIT passes
            _hooks.Add(new Hook(
                AssemblyManager.GetMethod("IsLoadable", FSTATIC)!,
                (Func<object, Type, bool>)((_, __) => true), applyByDefault: false));

            _hooks.Add(new Hook(
                AssemblyManager.GetMethod("JITAssemblies", FSTATIC)!,
                (Action<IEnumerable<Assembly>, PreJITFilter>)((_, __) => { }), applyByDefault: false));

            _hooks.Add(new Hook(
                ProgramT.GetMethod("ForceJITOnAssembly", FSTATIC)!,
                (Action<IEnumerable<Type>>)(_ => { }), applyByDefault: false));

            _hooks.Add(new Hook(
                ProgramT.GetMethod("ForceStaticInitializers", FSTATIC, new[] { typeof(Assembly) })!,
                (Action<Assembly>)(_ => { }), applyByDefault: false));

            // 2.  Wait for detours to finish before Main.LoadContent runs
            _hooks.Add(new Hook(
                MainT.GetMethod("LoadContent", FINSTANCE)!,
                (Action<Main>)((self) =>
                {
                    if (_detourTask?.IsCompleted == false)
                    {
                        Console.WriteLine("Waiting for detours…");
                        _detourTask.GetAwaiter().GetResult();
                    }
                    (typeof(Main).GetMethod("LoadContent", FINSTANCE)!)
                        .CreateDelegate<Action<Main>>()(self); // call original
                }), applyByDefault: false));

            // 3.  Fast‑forward the splash‑screen
            _hooks.Add(new Hook(
                MainT.GetMethod("DrawSplash", FINSTANCE)!,
                (Action<Main, GameTime>)((self, gt) =>
                {
                    Console.WriteLine("Fast splash start");
                    var swLocal = Stopwatch.StartNew();
                    for (int i = 0; i < 900 && Terraria.Main.showSplash; i++)
                    {
                        (typeof(Main).GetMethod("DrawSplash", FINSTANCE)!)
                            .CreateDelegate<Action<Main, GameTime>>()(self, gt);
                        Terraria.Main.Assets.TransferCompletedAssets();
                    }
                    Console.WriteLine($"Fast DrawSplash time: {swLocal.Elapsed}");
                }), applyByDefault: false));

            // ----------------------------------------------------------------- //
            //  Allow external registration before hooks are applied concurrently
            // ----------------------------------------------------------------- //
            RegisterDetours?.Invoke(_hooks);

            // Fix for CS0411: Explicitly specify the type arguments for the Select method.  
            var actions = _hooks.Select<Hook, Action>(h => h.Apply)
                               .Concat(_ilHooks.Select<ILHook, Action>(h => h.Apply))
                               .ToArray();
            Parallel.Invoke(actions);

            _sw.Stop();
        }

        // --------------------------------------------------------------------- //
        //  (Optional) Helpers you were experimenting with                       //
        // --------------------------------------------------------------------- //

        // Example reflection helper, kept from your original code
        private static void LoadMods()
        {
            Type modOrg = typeof(ModLoader).Assembly
                                           .GetType("Terraria.ModLoader.Core.ModOrganizer");
            modOrg.GetMethod("FindAllMods", FSTATIC)!.Invoke(null, null);
        }
    }
}
