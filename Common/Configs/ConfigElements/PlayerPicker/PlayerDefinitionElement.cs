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
        base.OnBind();

        // Remove mod filter search box
        if (ChooserFilterMod?.Parent != null) { ChooserFilterMod.Parent.Remove(); ChooserFilterMod.Parent.Deactivate(); }

        // Remove zoom buttons
        if (ChooserPanel != null) foreach (var c in ChooserPanel.Children.ToArray()) if (c is UIModConfigHoverImageSplit) c.Remove();

        // Remove scrollbar
        if (Main.PlayerList.Count <= 11)
            RemoveScrollbar();

        Main.LoadPlayers();
        UpdateNeeded = true;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        // Update height dynamically based on number of players player has.
        int count = Main.PlayerList?.Count ?? 0;
        int rows = Math.Max(1, (count + 11 - 1) / 11);

        float collapsed = 30f;
        float expanded = 130f + (rows - 1) * 44f;
        float h = SelectionExpanded ? expanded : collapsed;

        Height.Set(h, 0f);

        if (Parent is UISortableElement s)
            s.Height.Set(h, 0f);

        if (ChooserPanel != null)
            ChooserPanel.Height.Set(h - collapsed, 0f);

        if (SelectionExpanded && count <= 11)
            RemoveScrollbar();

        Recalculate();
    }

    protected override List<DefinitionOptionElement<PlayerDefinition>> CreateDefinitionOptionElementList()
    {
        OptionScale = 0.8f;
        Main.LoadPlayers();

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
            bool isJourney = Utilities.FindPlayer(o.Type).Player.difficulty == GameModeID.Creative;

            // Filter name
            if (!Utilities.FindPlayer(o.Type).Name.Contains(filter, StringComparison.OrdinalIgnoreCase))
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

        ChooserPanel.Recalculate();
        ChooserPanel.RecalculateChildren();
    }
}
