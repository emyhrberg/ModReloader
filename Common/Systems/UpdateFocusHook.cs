using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace ModHelper.Common.Systems
{
    public class KeepGameRunning : ModSystem
    {
        public static bool KeepRunning = true;

        public override void Load()
        {
            IL_Main.DoUpdate += DoUpdate;
            IL_Main.DrawRain += DrawRain;
            IL_Main.UpdateAudio += UpdateAudio;
        }

        private void DoUpdate(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            if (c.TryGotoNext(i => i.MatchLdsfld(typeof(Main), "hasFocus")))
            {
                // Replace the instruction that loads hasFocus with our delegate
                c.Remove();
                c.EmitDelegate<Func<bool>>(() =>
                {
                    if (!KeepRunning)
                    {
                        return Main.hasFocus;
                    }
                    return true;
                });
            }
        }

        private void DrawRain(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext((MoveType)2,
            [
                (Instruction i) => ILPatternMatchingExt.MatchLdcI4(i, 1),
                (Instruction i) => ILPatternMatchingExt.MatchStloc0(i)
            ]))
            {
                // Remove the original instructions.
                c.Emit(OpCodes.Ldloca, 0);

                // Emit the delegate that returns a bool.
                c.EmitDelegate(delegate (ref bool local)
                {
                    local = !KeepRunning ? Main.hasFocus : true;
                });
            }
        }

        private void UpdateAudio(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.After, i => i.MatchLdarg0(), i => i.MatchCall(out _), i => i.MatchStloc(0)))
            {
                c.Remove();
                c.EmitDelegate<Func<bool>>(() => !KeepRunning ? Main.hasFocus : true);
            }
        }
    }
}