using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using ModReloader.Core.Features.ModToggler.UI;
using ReLogic.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.Config;
using Terraria.UI;
using static ModReloader.Core.Features.ModToggler.UI.OptionElement;

namespace ModReloader.Core.Features.MainMenuFeatures.UI;

internal sealed class MainMenuModElement : UIPanel
{
    private readonly string internalModName;
    private readonly Action<string, bool> stateChanged;
    private EnabledState state;
    private readonly OptionEnabledText enabledText;

    public MainMenuModElement(string cleanModName, string internalModName, Texture2D icon, string version, bool enabled, Action<string, bool> stateChanged, string modDescription = "")
    {
        this.internalModName = internalModName;
        this.stateChanged = stateChanged;
        Width.Set(0f, 1f);
        Height.Set(30f, 0f);

        Append(new ModEnabledIcon(TextureAssets.MagicPixel.Value, internalModName, icon) { Left = { Pixels = -6f } });

        Append(new ModInfoIcon(Ass.ModInfo, internalModName, "More Info", modDescription, cleanModName, () => Main.menuMode = 0) { Left = { Pixels = -6+26f }, VAlign = 0.5f, Top = { Pixels = -1f } });

        if (HasConfig(internalModName))
            Append(new ConfigIcon(Ass.ConfigOpen, internalModName, "Open config", cleanModName, () => Main.menuMode = 0) { Left = { Pixels = -6+26+24 }, VAlign = 0.5f, Top = { Pixels = -1f } });

        float maxWidth = 148f;
        string text = TruncateToWidth(cleanModName, FontAssets.MouseText.Value, maxWidth);
        Append(new ModTitleText(text, $"{internalModName} v{version}", internalModName: internalModName) { Left = { Pixels = 74f }, VAlign = 0.5f });

        enabledText = new OptionEnabledText("Enabled");
        enabledText.Left.Set(-64f, 1f);
        Append(enabledText);
        SetState(enabled ? EnabledState.Enabled : EnabledState.Disabled);
    }

    private static bool HasConfig(string internalModName)
    {
        FieldInfo configsProp = typeof(ConfigManager).GetField("Configs", BindingFlags.Static | BindingFlags.NonPublic);
        var configs = configsProp?.GetValue(null) as IDictionary<Mod, List<ModConfig>>;
        Mod mod = ModLoader.GetMod(internalModName);
        return mod != null && configs != null && configs.TryGetValue(mod, out List<ModConfig> modConfigs) && modConfigs.Count > 0;
    }

    private void SetState(EnabledState newState)
    {
        state = newState;
        enabledText.SetTextState(state);
    }

    public override void LeftClick(UIMouseEvent evt)
    {
        if (evt.Target is not OptionEnabledText)
            return;

        base.LeftClick(evt);
        SetState(state == EnabledState.Enabled ? EnabledState.Disabled : EnabledState.Enabled);
        bool enabled = state == EnabledState.Enabled;
        typeof(ModLoader).GetMethod("SetModEnabled", BindingFlags.NonPublic | BindingFlags.Static)?.Invoke(null, [internalModName, enabled]);
        stateChanged?.Invoke(internalModName, enabled);
    }

    private static string TruncateToWidth(string text, DynamicSpriteFont font, float maxWidth)
    {
        const string ellipsis = "...";
        if (font.MeasureString(text).X <= maxWidth)
            return text;

        for (int len = text.Length - 1; len > 0; len--)
        {
            string candidate = text[..len] + ellipsis;
            if (font.MeasureString(candidate).X <= maxWidth)
                return candidate;
        }

        return ellipsis;
    }
}
