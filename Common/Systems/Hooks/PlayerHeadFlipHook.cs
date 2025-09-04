using System;
using Microsoft.Xna.Framework;
using MonoMod.Cil;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;

namespace ModReloader.Common.Systems.Hooks;

public class PlayerHeadFlipHook : ModSystem
{
    public static bool shouldFlipHeadDraw = true;

    public override void Load()
    {
        MonoModHooks.Modify(typeof(MapHeadRenderer).GetMethod("DrawPlayerHead"), IL_MapHeadRenderer_DrawPlayerHead);
    }

    public static void IL_MapHeadRenderer_DrawPlayerHead(ILContext il)
    {
        try
        {
            ILCursor c = new(il);

            // extra code
            c.GotoNext(MoveType.Before, i => i.MatchLdcR4(2));
            c.Index += 2;

            ILLabel skipCentering = il.DefineLabel();

            c.EmitLdsfld(typeof(PlayerHeadFlipHook).GetField(nameof(shouldFlipHeadDraw)));
            c.EmitBrfalse(skipCentering);
            c.EmitDelegate<Func<Vector2, Vector2>>(inCenter =>
            {
                 //return new Vector2(inCenter.X * 0.8f, inCenter.Y); // original, almost works but 0.1f offset. should go more left than this.
                return new Vector2(inCenter.X - 8f, inCenter.Y); // orig
            });
            c.MarkLabel(skipCentering);

            // find where the draw data loads the 
            c.GotoNext(MoveType.Before, i => i.MatchLdcI4(0));

            // define the labels for the if, else statement
            ILLabel normRet = il.DefineLabel();
            ILLabel altLabel = il.DefineLabel();

            c.EmitLdsfld(typeof(PlayerHeadFlipHook).GetField(nameof(shouldFlipHeadDraw)));
            c.EmitBrtrue(altLabel); // if(!shouldFlipHeadDraw)
                                    // {
            c.Index++;                // push 0
            c.EmitBr(normRet);        // }
                                      // else
            c.MarkLabel(altLabel);    // {
            c.EmitLdcI4(1);            // push 1
            c.MarkLabel(normRet);    // }
        }
        catch (Exception e)
        {
            Log.Info("Error in IL_MapHeadRenderer_DrawPlayerHead: " + e.Message);
            // oop!
            // MonoModHooks.DumpIL(ModContent.GetInstance<XGWorld>(), il);
        }
    }
}