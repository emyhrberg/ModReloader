using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;

namespace SquidTestingMod.UI.Panels
{
    /// <summary>
    /// A panel to display the contents of client.log.
    /// </summary>
    public class ModsPanel : RightParentPanel
    {
        public ModsPanel() : base(title: "Mods List", scrollbarEnabled: false)
        {
            var mods = ModLoader.Mods.Skip(1);//ignore the built in Modloader mod

            foreach (var mod in mods)
            {
                AddOnOffOption(null, title: mod.DisplayNameClean, hoverText: mod.Name + " (v" + mod.Version + ")");
            }
            AddPadding();
        }
    }



    // public override void Update(GameTime gameTime)
    // {
    //     base.Update(gameTime);

    //     double currentTime = gameTime.TotalGameTime.TotalSeconds;
    //     if (currentTime % 1 == 0) // Update every second
    //     {
    //         Log.Info("Updating log panel");

    //         string content = "";
    //         string logPath = @"C:\Program Files (x86)\Steam\steamapps\common\tModLoader\tModLoader-Logs\client.log";

    //         try
    //         {
    //             if (File.Exists(logPath))
    //             {
    //                 // Read all lines with shared access.
    //                 List<string> allLines = new List<string>();
    //                 using (FileStream stream = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
    //                 using (StreamReader reader = new StreamReader(stream))
    //                 {
    //                     string line;
    //                     while ((line = reader.ReadLine()) != null)
    //                     {
    //                         allLines.Add(line);
    //                     }
    //                 }
    //                 // Prepend line numbers.
    //                 for (int i = 0; i < allLines.Count; i++)
    //                 {
    //                     allLines[i] = $"{i + 1}: {allLines[i]}";
    //                 }
    //                 content = string.Join("\n", allLines);
    //             }
    //             else
    //             {
    //                 content = "Log file not found.";
    //             }
    //         }
    //         catch (Exception ex)
    //         {
    //             content = "Error reading log: " + ex.Message;
    //         }

    //         // Update the text panel with the processed content.
    //         logTextPanel.SetText(content);

    //         // Force scroll to bottom:
    //         if (scrollbar != null && uiList != null)
    //         {
    //             uiList.Recalculate(); // Ensure layout is up-to-date.
    //             float maxScroll = Math.Max(0, uiList.GetInnerDimensions().Height - uiList.GetDimensions().Height);
    //             scrollbar.ViewPosition = maxScroll;
    //         }
    //     }
    // }
}
