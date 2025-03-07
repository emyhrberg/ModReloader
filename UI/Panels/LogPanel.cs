using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SquidTestingMod.Helpers;
using Terraria.GameContent.UI.Elements;

namespace SquidTestingMod.UI.Panels
{
    /// <summary>
    /// A panel to display the contents of client.log.
    /// </summary>
    public class LogPanel : RightParentPanel
    {
        private UITextPanel<string> logTextPanel;

        public LogPanel() : base(title: "Log", scrollbarEnabled: true)
        {
            // Resize panel
            Height.Set(500f, 0f);
            Width.Set(900f, 0f);
            HAlign = 0.5f;
            VAlign = 1.0f;
            Top.Set(0, 0);
            Left.Set(0, 0);

            // Create and add a log text panel that fills the UI list.
            logTextPanel = new UITextPanel<string>("", 0.3f, true)
            {
                Width = { Percent = 1f },
                Height = { Percent = 1f },
                TextColor = Color.White,
                PaddingLeft = 4f,
                PaddingRight = 4f,
                PaddingTop = 4f,
                PaddingBottom = 4f
            };
            uiList.Add(logTextPanel);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            double currentTime = gameTime.TotalGameTime.TotalSeconds;
            if (currentTime % 1 == 0) // Update every second
            {
                Log.Info("Updating log panel");

                string content = "";
                string logPath = @"C:\Program Files (x86)\Steam\steamapps\common\tModLoader\tModLoader-Logs\client.log";

                try
                {
                    if (File.Exists(logPath))
                    {
                        // Read all lines with shared access.
                        List<string> allLines = new List<string>();
                        using (FileStream stream = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                allLines.Add(line);
                            }
                        }
                        // Prepend line numbers.
                        for (int i = 0; i < allLines.Count; i++)
                        {
                            allLines[i] = $"{i + 1}: {allLines[i]}";
                        }
                        content = string.Join("\n", allLines);
                    }
                    else
                    {
                        content = "Log file not found.";
                    }
                }
                catch (Exception ex)
                {
                    content = "Error reading log: " + ex.Message;
                }

                // Update the text panel with the processed content.
                logTextPanel.SetText(content);

                // Force scroll to bottom:
                if (scrollbar != null && uiList != null)
                {
                    uiList.Recalculate(); // Ensure layout is up-to-date.
                    float maxScroll = Math.Max(0, uiList.GetInnerDimensions().Height - uiList.GetDimensions().Height);
                    scrollbar.ViewPosition = maxScroll;
                }
            }
        }
    }
}
