using ModReloader.Common.Configs.ConfigElements.PlayerPicker;
using ModReloader.Core.Features.Reload;
using System;
using System.Collections.Generic;
using Terraria.IO;
using Terraria.ModLoader.Config.UI;

namespace ModReloader.Common.Configs.ConfigElements.PlayerPicker;


public class PlayerDefinitionElement : DefinitionElement<PlayerDefinition>
{
    protected override DefinitionOptionElement<PlayerDefinition> CreateDefinitionOptionElement() => new PlayerDefinitionOptionElement(Value, 0.5f);

    // Copy implementation from NPCDefinitionElement, except we iterate Main.PlayerList instead of the NPC List.
    protected override List<DefinitionOptionElement<PlayerDefinition>> CreateDefinitionOptionElementList()
    {
        OptionScale = 0.8f; // default scale for all options. can be modified with the zoom in, zoom out buttons.

        var options = new List<DefinitionOptionElement<PlayerDefinition>>();

        Main.LoadPlayers();

        foreach (PlayerFileData file in Main.PlayerList)
        {
            if (file == null)
                continue;

            Log.Info("Adding player to config: " + file.Name);

            var definition = new PlayerDefinition(file.Path);

            var option = new PlayerDefinitionOptionElement(definition, OptionScale);

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

    protected override List<DefinitionOptionElement<PlayerDefinition>> GetPassedOptionElements()
    {
        var passed = new List<DefinitionOptionElement<PlayerDefinition>>();

        foreach (var option in Options)
        {
            // Should this be the localized Player name?
            if (!Utilities.FindPlayer(option.Type).Name.Contains(ChooserFilter.CurrentString, StringComparison.OrdinalIgnoreCase))
                continue;

            string modname = "Terraria";

            if (modname.IndexOf(ChooserFilterMod.CurrentString, StringComparison.OrdinalIgnoreCase) == -1)
                continue;

            passed.Add(option);
        }
        return passed;
    }

    public override void OnBind()
    {
        base.OnBind();
        ChooserFilterMod.Parent.Remove();
        ChooserFilterMod.Parent.Deactivate();
    }}