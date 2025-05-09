// public class ExtractEdit : ILEditReflection
// {
//     private static Type BuildProperties => typeof(Main).Assembly.GetType("Terraria.ModLoader.Core.BuildProperties") ?? throw new Exception("'BuildProperties' is null");
//     private static MethodInfo ExtractMethod => (typeof(Main).Assembly.GetType("Terraria.ModLoader.UI.UIExtractMod")?.GetMethod("Extract", BindingFlags.NonPublic | BindingFlags.Instance)) ?? throw new Exception("'ExtractMethod' is null");
//     private static FieldInfo HideCode => BuildProperties.GetField("hideCode", BindingFlags.Instance | BindingFlags.NonPublic) ?? throw new Exception("'hideCode' is null");
//     private static FieldInfo HideResources => BuildProperties.GetField("hideResources", BindingFlags.Instance | BindingFlags.NonPublic) ?? throw new Exception("'hideResources' is null");
//     public override MethodInfo Method => ExtractMethod;
//     public override void Apply(ILCursor c)
//     {
//         while (c.TryGotoNext(i => i.MatchLdfld(HideCode)))
//         {
//             c.Index++;
//             c.Emit(OpCodes.Pop);
//             c.EmitDelegate(() => !Config.Instance.ExtractCode);
//         }
//         c.Index = 0;
//         while (c.TryGotoNext(i => i.MatchLdfld(HideResources)))
//         {
//             c.Index++;
//             c.Emit(OpCodes.Pop);
//             c.EmitDelegate(() => !Config.Instance.ExtractResources);
//         }
//     }
// }