using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using Terraria.ModLoader;

namespace ModReloader.Common.Systems.Hooks;

/// <summary>Moves ChatImprover’s chat UI by (OffsetX, OffsetY).</summary>
public class ChatHookChatImprover : ModSystem
{
    public static float OffsetX = 0;
    public static float OffsetY = -50;   // updated live in PostUpdateEverything
    private readonly List<ILHook> _hooks = new();

    public override void Load()
    {
        if (!ModLoader.TryGetMod("ChatImprover", out var cim))
            return;                       // ChatImprover not present

        var t = cim.Code.GetType("ChatImprover.ChatImprover");

        HookIL(t, "DrawPlayerChat", InjectVector2Offset); // caret + box
        HookIL(t, "DrawChat", InjectDrawChatOffset); // scroll-back
        HookIL(t, "DrawBackgroundPanel", InjectIntPanelOffset); // grey panel
    }

    /*──────────────────────── live toggle (unchanged) ───────*/
    public override void PostUpdateEverything()
    {
        if (!Main.drawingPlayerChat)
            return;

        OffsetY = Conf.C.MoveChat ? 0 : 0;
    }

    /*──────────────────────── helper ────────────────────────*/
    private void HookIL(Type type, string name, ILContext.Manipulator m)
    {
        var mi = type?.GetMethod(name,
                   BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (mi is not null) _hooks.Add(new ILHook(mi, m));
    }

    /*──────────────────────── input box / caret ─────────────*/
    private static void InjectVector2Offset(ILContext il)
    {
        var ctor = typeof(Vector2).GetConstructor(new[] { typeof(float), typeof(float) });
        var add = typeof(Vector2).GetMethod("op_Addition",
                      BindingFlags.Public | BindingFlags.Static,
                      null, new[] { typeof(Vector2), typeof(Vector2) }, null);
        var fldX = typeof(ChatHookChatImprover).GetField(nameof(OffsetX));
        var fldY = typeof(ChatHookChatImprover).GetField(nameof(OffsetY));

        var c = new ILCursor(il);
        while (c.TryGotoNext(i => i.MatchNewobj(ctor)))
        {
            c.Index++;                   // after newobj
            c.Emit(OpCodes.Ldsfld, fldX);
            c.Emit(OpCodes.Ldsfld, fldY);
            c.Emit(OpCodes.Newobj, ctor);
            c.Emit(OpCodes.Call, add);  // original + (OffsetX, OffsetY)
        }
    }

    /*──────────────────────── scroll-back lines ─────────────*/
    /// <summary>Add (OffsetX,OffsetY) **only** to the vector that positions
    /// every chat line (X literal 88f), so sizes etc. stay intact.</summary>
    private static void InjectDrawChatOffset(ILContext il)
    {
        var ctor = typeof(Vector2).GetConstructor(new[] { typeof(float), typeof(float) });
        var add = typeof(Vector2).GetMethod("op_Addition",
                      BindingFlags.Public | BindingFlags.Static,
                      null, new[] { typeof(Vector2), typeof(Vector2) }, null);
        var fldX = typeof(ChatHookChatImprover).GetField(nameof(OffsetX));
        var fldY = typeof(ChatHookChatImprover).GetField(nameof(OffsetY));

        var c = new ILCursor(il);

        // Look for: ldc.r4 88  (base X) … newobj Vector2
        while (c.TryGotoNext(
                   MoveType.Before,
                   i => i.MatchLdcR4(88f),
                   i => i.MatchLdarg(out _)))       // 88f is followed by Y calc
        {
            // Skip forward to *after* the new Vector2(...)
            if (!c.TryGotoNext(MoveType.After, i => i.MatchNewobj(ctor)))
                break;

            c.Emit(OpCodes.Ldsfld, fldX);
            c.Emit(OpCodes.Ldsfld, fldY);
            c.Emit(OpCodes.Newobj, ctor);
            c.Emit(OpCodes.Call, add);
        }
    }

    /*──────────────────────── grey background panel ─────────*/
    private static void InjectIntPanelOffset(ILContext il)
    {
        var fldX = typeof(ChatHookChatImprover).GetField(nameof(OffsetX));
        var fldY = typeof(ChatHookChatImprover).GetField(nameof(OffsetY));
        var c = new ILCursor(il).Goto(0);

        // int startX += (int)OffsetX;
        c.Emit(OpCodes.Ldarg_1);
        c.Emit(OpCodes.Ldsfld, fldX);
        c.Emit(OpCodes.Conv_I4);
        c.Emit(OpCodes.Add);
        c.Emit(OpCodes.Starg_S, (byte)1);

        // int startY += (int)OffsetY;
        c.Emit(OpCodes.Ldarg_2);
        c.Emit(OpCodes.Ldsfld, fldY);
        c.Emit(OpCodes.Conv_I4);
        c.Emit(OpCodes.Add);
        c.Emit(OpCodes.Starg_S, (byte)2);
    }

    public override void Unload()
    {
        foreach (var h in _hooks) h.Dispose();
        _hooks.Clear();
    }
}
