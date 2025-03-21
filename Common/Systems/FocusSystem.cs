using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Systems
{
    public class FocusSystem : ModSystem
    {
        public override void Load()
        {
            // IL_Main.DoUpdate += GameUpdate;
            // IL_Main.DoUpdate += AudioUpdate;
        }

        public void GameUpdate(ILContext il)
        {
            // This is the IL cursor, it's a bit like a text cursor in an editor.
            ILCursor c = new(il);

            // The cursor starts at the first instruction of the method.
            MoveType moveType = MoveType.Before;

            // We're looking for the first occurrence of the following instructions:
            // - Load the static field "hasFocus" from the Main class.
            // - Store the value in a local variable.
            // If we find these instructions, we'll insert our own code after them.
            // We're using a lambda expression to match the instructions.
            // The lambda takes an instruction and returns a boolean.
            Func<Instruction, bool>[] array = [x => x.MatchLdsfld(typeof(Main), "hasFocus")];
            if (c.TryGotoNext(moveType, array))
            {
                c.Emit(OpCodes.Ldc_I4_1); // Ldc_I4_1 comes from the OpCodes class and pushes the integer 1 onto the stack.
                c.Emit<Main>(OpCodes.Stsfld, "hasFocus"); // Stsfld means "store static field" and pops the value from the stack and stores it in the specified field.
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