// using System;
// using System.Reflection;
// using Mono.Cecil;
// using Mono.Cecil.Cil;
// using MonoMod.Cil;
// using Terraria;
// using Terraria.ModLoader;

// namespace SquidTestingMod.Common.Systems
// {
//     // This mod forces the game to continue updating (and playing audio) even when not focused.
//     public class NoPauseMod : Mod
//     {
//         // In tModLoader v0.11 and earlier you might see IL hooks attached via IL_Main.
//         // In newer versions these hooks may be available via IL.Terraria.Main.
//         // Adjust the hook names if necessary.
//         public override void Load()
//         {
//             // Attach IL hooks to patch the update and audio update routines.
//             IL_Main.DoUpdate += DoUpdatePatch;
//             IL_Main.UpdateAudio += UpdateAudioPatch;
//         }

//         public override void Unload()
//         {
//             // Detach IL hooks on unload so that your modifications don’t persist.
//             IL_Main.DoUpdate -= DoUpdatePatch;
//             IL_Main.UpdateAudio -= UpdateAudioPatch;
//         }

//         /// <summary>
//         /// IL hook for Main.DoUpdate.
//         /// This patch finds the instruction that loads the value of Main.hasFocus,
//         /// then forces it to always load “true” (1), effectively preventing auto‑pause.
//         /// </summary>
//         private void DoUpdatePatch(ILContext il)
//         {
//             ILCursor cursor = new ILCursor(il);

//             // Try to locate the IL instruction that loads the static field Main.hasFocus.
//             // (ILPatternMatchingExt.MatchLdsfld is a helper method that checks for "ldsfld typeof(Main).hasFocus".)
//             if (cursor.TryGotoNext(MoveType.After,
//                 instr => ILPatternMatchingExt.MatchLdsfld(instr, typeof(Main), "hasFocus")))
//             {
//                 // Replace the loaded value with the constant 1 (true)
//                 cursor.Emit(OpCodes.Ldc_I4_1); // Push 1 (true) onto the stack

//                 // Retrieve the FieldInfo for Main.hasFocus.
//                 FieldInfo hasFocusField = typeof(Main).GetField("hasFocus", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
//                 if (hasFocusField != null)
//                 {
//                     // Store the true value into Main.hasFocus.
//                     cursor.Emit(OpCodes.Stsfld, hasFocusField);
//                 }
//                 else
//                 {
//                     Logger.Warn("NoPauseMod: Could not find the 'hasFocus' field in Main.");
//                 }
//             }
//             else
//             {
//                 Logger.Warn("NoPauseMod: Failed to find the IL instruction for Main.hasFocus.");
//             }
//         }

//         /// <summary>
//         /// IL hook for Main.UpdateAudio.
//         /// This patch finds a sequence in the audio update method and then forces a local bool value to true,
//         /// ensuring that audio updates continue even when the game is unfocused.
//         /// </summary>
//         private void UpdateAudioPatch(ILContext il)
//         {
//             ILCursor cursor = new ILCursor(il);
//             MethodReference methodRef = null; // Used to capture the method reference in the pattern.

//             // Look for a pattern: ldarg.0, then a call (any method), then stloc.0.
//             // (The numeric cast (MoveType)2 is used here per the original snippet; adjust if needed.)
//             if (cursor.TryGotoNext((MoveType)2, new Func<Instruction, bool>[]
//                 {
//                     instr => ILPatternMatchingExt.MatchLdarg0(instr),
//                     instr => ILPatternMatchingExt.MatchCall(instr, out methodRef),
//                     instr => ILPatternMatchingExt.MatchStloc(instr, 0)
//                 }))
//             {
//                 // Load the address of local variable 0.
//                 cursor.Emit(OpCodes.Ldloca_S, 0);

//                 // Emit a delegate that forces the referenced boolean to true.
//                 cursor.EmitDelegate<SetTrueDelegate>(SetTrue);
//             }
//             else
//             {
//                 Logger.Warn("NoPauseMod: Failed to find the IL pattern in UpdateAudio.");
//             }
//         }

//         // Define a delegate type that takes a bool by reference.
//         private delegate void SetTrueDelegate(ref bool value);

//         // This method will be injected into the IL to force a bool to true.
//         private static void SetTrue(ref bool value)
//         {
//             value = true;
//         }
//     }
// }
