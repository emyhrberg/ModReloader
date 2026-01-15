using ModReloader.Core.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.IO;
using Terraria.ModLoader.Config.UI;

namespace ModReloader.Common.Configs.ConfigElements.WorldPicker;

public sealed class WorldDefinitionElement : DefinitionElement<WorldDefinition>
{
    protected override DefinitionOptionElement<WorldDefinition> CreateDefinitionOptionElement() => new WorldDefinitionOptionElement(Value, 0.5f);

    public override void OnBind()
    {
        base.OnBind();

        // Remove mod filter search box
        if (ChooserFilterMod?.Parent != null) { ChooserFilterMod.Parent.Remove(); ChooserFilterMod.Parent.Deactivate(); }

        // Remove zoom buttons
        if (ChooserPanel != null) foreach (var c in ChooserPanel.Children.ToArray()) if (c is UIModConfigHoverImageSplit) c.Remove();

        // Remove scrollbar
        if (Main.WorldList.Count <= 11) RemoveScrollbar();

        Main.LoadWorlds(); 
        UpdateNeeded = true;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        // Adjust height based on number of worlds
        Main.LoadWorlds();
        int count = Main.WorldList?.Count ?? 0;
        int rows = Math.Max(1, (count + 11 - 1) / 11);

        float collapsed = 30f;
        float expanded = 130f + (rows - 1) * 44f;
        float h = SelectionExpanded ? expanded : collapsed;

        Height.Set(h, 0f);
        if (Parent is UISortableElement s) 
            s.Height.Set(h, 0f);
        ChooserPanel?.Height.Set(h - collapsed, 0f);

        if (SelectionExpanded && count <= 11) RemoveScrollbar();

        Recalculate();
    }

    protected override List<DefinitionOptionElement<WorldDefinition>> CreateDefinitionOptionElementList()
    {
        OptionScale = 0.8f;

        // Load worlds
        Main.LoadWorlds();

        var arr = Main.WorldList.ToArray();
        var options = new List<DefinitionOptionElement<WorldDefinition>>(arr.Length);

        foreach (var f in arr)
        {
            if (f == null) continue;

            var def = new WorldDefinition(f.Path);
            var opt = new WorldDefinitionOptionElement(def, OptionScale);

            opt.OnLeftClick += (_, _) => { Value = opt.Definition; UpdateNeeded = true; SelectionExpanded = false; };

            options.Add(opt);
        }

        return options;
    }

    protected override List<DefinitionOptionElement<WorldDefinition>> GetPassedOptionElements()
    {
        var passed = new List<DefinitionOptionElement<WorldDefinition>>();
        string filter = ChooserFilter?.CurrentString ?? "";
        if (string.IsNullOrWhiteSpace(filter)) filter = "";
        Main.LoadWorlds();
        foreach (var o in Options)
        {
            if (o?.Definition == null) continue;
            if (Utilities.FindWorldId(o.Definition.Name) == -1) continue;
            if (filter.Length == 0) { passed.Add(o); continue; }
            if (o.Definition.DisplayName.Contains(filter, StringComparison.OrdinalIgnoreCase)) { passed.Add(o); continue; }
            WorldFileData f = Main.WorldList.FirstOrDefault(w => string.Equals(w.Path, o.Definition.Name, StringComparison.OrdinalIgnoreCase));
            if (f == null) continue;
            string diff = WorldDefinitionOptionElement.GetDifficultyNameForFilter(f);
            if (diff.Contains(filter, StringComparison.OrdinalIgnoreCase)) { passed.Add(o); continue; }
        }
        return passed;
    }


    private void RemoveScrollbar()
    {
        if (ChooserGrid != null) { ChooserGrid.SetScrollbar(null); ChooserGrid.Width.Set(0f, 1f); }
        if (ChooserPanel == null) return;
        foreach (var c in ChooserPanel.Children.ToArray()) if (c is UIScrollbar) c.Remove();
        ChooserPanel.Recalculate(); ChooserPanel.RecalculateChildren();
    }
}
