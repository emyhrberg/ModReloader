using ModReloader.Core.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using Terraria.ModLoader.Config.UI;

namespace ModReloader.Common.Configs.ConfigElements.PlayerPicker;

public sealed class PlayerDefinitionElement : DefinitionElement<PlayerDefinition>
{
    protected override DefinitionOptionElement<PlayerDefinition> CreateDefinitionOptionElement() => new PlayerDefinitionOptionElement(Value, 0.5f, isActiveSelection: true);

    public override void OnBind()
    {
        // Load once before the base binding creates the selected option.
        Main.LoadPlayers();
        base.OnBind();

        // Remove mod filter search box
        if (ChooserFilterMod?.Parent != null) { ChooserFilterMod.Parent.Remove(); ChooserFilterMod.Parent.Deactivate(); }

        // Remove zoom buttons
        if (ChooserPanel != null) foreach (var c in ChooserPanel.Children.ToArray()) if (c is UIModConfigHoverImageSplit) c.Remove();

        // Remove scrollbar
        if (Main.PlayerList.Count <= 11)
            RemoveScrollbar();

        UpdateNeeded = true;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        // The base update can reset the height when expanding or filtering.
        int count = Main.PlayerList?.Count ?? 0;
        int rows = Math.Max(1, (count + 11 - 1) / 11);

        float collapsed = 30f;
        float expanded = 130f + (rows - 1) * 44f;
        float h = SelectionExpanded ? expanded : collapsed;
        var parent = Parent as UISortableElement;

        if (Height.Pixels == h &&
            (parent == null || parent.Height.Pixels == h) &&
            (ChooserPanel == null || ChooserPanel.Height.Pixels == h - collapsed))
            return;

        Height.Set(h, 0f);
        parent?.Height.Set(h, 0f);
        ChooserPanel?.Height.Set(h - collapsed, 0f);

        Recalculate();
    }

    protected override List<DefinitionOptionElement<PlayerDefinition>> CreateDefinitionOptionElementList()
    {
        OptionScale = 0.8f;

        var arr = Main.PlayerList.ToArray();
        var options = new List<DefinitionOptionElement<PlayerDefinition>>(arr.Length);

        foreach (var f in arr)
        {
            if (f == null)
                continue;

            var def = new PlayerDefinition(f.Path);
            var opt = new PlayerDefinitionOptionElement(def, OptionScale, isActiveSelection: true);

            opt.OnLeftClick += (_, _) =>
            {
                Value = opt.Definition;
                UpdateNeeded = true;
                SelectionExpanded = false;
            };

            options.Add(opt);
        }

        return options;
    }

    protected override List<DefinitionOptionElement<PlayerDefinition>> GetPassedOptionElements()
    {
        var passed = new List<DefinitionOptionElement<PlayerDefinition>>();
        string filter = ChooserFilter?.CurrentString ?? "";

        foreach (var o in Options)
        {
            // If player is journey and user is searching for journey, add it
            bool isJourney = o.Definition.File?.Player?.difficulty == GameModeID.Creative;

            // Filter name
            if (!o.Definition.File?.Name.Contains(filter, StringComparison.OrdinalIgnoreCase) ?? true)
                continue;

            passed.Add(o);
        }

        return passed;
    }

    private void RemoveScrollbar()
    {
        if (ChooserPanel == null)
            return;

        ChooserGrid.SetScrollbar(null);
        ChooserGrid.Width.Set(0f, 1f);

        foreach (var c in ChooserPanel.Children.ToArray())
        {
            if (c is UIScrollbar)
                c.Remove();
        }
    }
}
