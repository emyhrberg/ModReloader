using ModReloader.Core.Features.Reload;
using System;
using System.Collections.Generic;
using Terraria.IO;
using Terraria.ModLoader.Config.UI;

namespace ModReloader.Common.Configs.ConfigElements.WorldPicker;

public sealed class WorldDefinitionElement : DefinitionElement<WorldDefinition>
{
    protected override DefinitionOptionElement<WorldDefinition> CreateDefinitionOptionElement()
        => new WorldDefinitionOptionElement(Value, 0.5f);

    protected override List<DefinitionOptionElement<WorldDefinition>> CreateDefinitionOptionElementList()
    {
        OptionScale = 0.8f;

        Main.LoadWorlds();

        // Snapshot to avoid "collection was modified"
        var worlds = Main.WorldList.ToArray();

        var options = new List<DefinitionOptionElement<WorldDefinition>>(
            capacity: worlds.Length
        );

        foreach (WorldFileData file in worlds)
        {
            if (file == null)
                continue;

            Log.Info("Adding world to config: " + file.Name);

            var def = new WorldDefinition(file.Path);
            var option = new WorldDefinitionOptionElement(def, OptionScale);

            option.OnLeftClick += (_, _) =>
            {
                Value = option.Definition;
                UpdateNeeded = true;
                SelectionExpanded = false;
            };

            options.Add(option);
        }

        return options;
    }


    protected override List<DefinitionOptionElement<WorldDefinition>>
        GetPassedOptionElements()
    {
        var passed = new List<DefinitionOptionElement<WorldDefinition>>();

        foreach (var option in Options)
        {
            var file = Utilities.FindWorldId(option.Definition.Name);
            if (file == -1)
                continue;

            //if (!file.Name.Contains(
                    //ChooserFilter.CurrentString,
                    //StringComparison.OrdinalIgnoreCase))
                //continue;

            passed.Add(option);
        }

        return passed;
    }

    public override void OnBind()
    {
        base.OnBind();
        ChooserFilterMod.Parent.Remove();
        ChooserFilterMod.Parent.Deactivate();
    }
}
