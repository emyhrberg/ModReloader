using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Systems
{
    public class FocusSystem : ModSystem
    {
        public override void Load()
        {
            IL_Main.DoUpdate += GameUpdate;
            IL_Main.DoUpdate += AudioUpdate;
        }

        public void GameUpdate(ILContext il)
        {
            ILCursor c = new(il);
            MoveType moveType = MoveType.Before;
            Func<Instruction, bool>[] array = [x => x.MatchLdsfld(typeof(Main), "hasFocus")];
            if (c.TryGotoNext(moveType, array))
            {
                c.Emit(OpCodes.Ldc_I4_1);
                c.Emit<Main>(OpCodes.Stsfld, "hasFocus");
            }
        }

        public void AudioUpdate(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.After, i => i.MatchLdarg0(), i => i.MatchCall(out _), i => i.MatchStloc(0)))
            {
                c.Emit(OpCodes.Ldloca, 0);
                c.EmitDelegate((ref bool local) =>
                {
                    local = true;
                });
            }
        }
    }
}