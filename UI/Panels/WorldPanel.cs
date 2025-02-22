using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SquidTestingMod.Common.Systems;
using SquidTestingMod.Helpers;
using SquidTestingMod.UI.Panels;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI.Panels
{
    /// <summary>
    /// A panel containing options to modify player behaviour like God, Fast, Build, etc.
    /// </summary>
    public class WorldPanel : RightParentPanel
    {
        // Variables
        OptionPanel time;
        OptionPanel moon;

        public WorldPanel() : base("World")
        {
            // World info
            string worldName = "Unknown Name";
            string worldSize = "Unknown Size";
            if (Main.ActiveWorldFileData.GetWorldName() != null)
                worldName = Main.ActiveWorldFileData.Name;
            if (Main.ActiveWorldFileData._worldSizeName != null)
            {
                worldSize = Main.ActiveWorldFileData.WorldSizeName;
            }

            string difficultyText = Main.ActiveWorldFileData.GameMode switch
            {
                GameModeID.Normal => "Normal",
                GameModeID.Expert => "Expert",
                GameModeID.Master => "Master",
                GameModeID.Creative => "Journey",
                _ => "Unknown Difficulty"
            };

            // Add all content in the panel
            AddOptionPanel(
                title: "World Name: " + worldName,
                description: "The name of the world",
                checkBox: false,
                color: Color.Orange,
                onClick: null
            );
            AddOptionPanel(
                title: "World Size: " + worldSize,
                description: "The size of the world",
                checkBox: false,
                color: Color.Orange,
                onClick: null
            );
            AddOptionPanel(
                title: "World Difficulty: " + difficultyText,
                description: "The difficulty of the world",
                checkBox: false,
                color: Color.Orange,
                onClick: null
            );
            // Spawn a meteor at the player's position
            AddOptionPanel(
                title: "Spawn Meteor",
                description: "Spawn a meteor",
                checkBox: false,
                color: Color.Red,
                onClick: () =>
                {
                    Log.Info("Trying to drop meteor");
                    WorldGen.dropMeteor();
                }
            );

            AddOptionPanel(
                title: "Spawn Goblin Army",
                description: "Spawn a goblin army",
                checkBox: false,
                color: Color.Red,
                onClick: () =>
                {
                    // Only trigger the invasion if one isn't already active
                    if (Main.invasionType == 0)
                    {
                        Main.invasionType = InvasionID.GoblinArmy;
                        Main.invasionSize = 50; // Adjust size as needed
                        Main.invasionProgress = 0;
                        Main.invasionProgressWave = 0;
                        // Set the invasion origin to the player's X position
                        Main.invasionX = (int)Main.player[Main.myPlayer].position.X;
                        Main.invasionDelay = 0;
                        // Sync in multiplayer
                        // if (Main.netMode == NetmodeID.Server)
                        // {
                        //     NetMessage.SendData(MessageID.InvasionProgressUpdate);
                        // }
                    }
                    else
                    {
                        Main.NewText("An invasion is already in progress!");
                    }
                }

            );

            time = AddOptionPanel(
                title: "Time: ",
                description: "Increase the in-game time by 1 hour",
                checkBox: false,
                color: Color.Red,
                onClick: () =>
                {
                    const double cycleLength = Main.dayLength + Main.nightLength;
                    double fullTime = Main.time;
                    if (!Main.dayTime)
                    {
                        fullTime += Main.dayLength;
                    }
                    // Add one hour in ticks (1 hour = 3600 ticks)
                    fullTime += 3600;

                    // Ensure fullTime is within the valid cycle range.
                    fullTime %= cycleLength;
                    if (fullTime < 0)
                    {
                        fullTime += cycleLength;
                    }

                    Main.dayTime = fullTime < Main.dayLength;
                    if (!Main.dayTime)
                    {
                        fullTime -= Main.dayLength;
                    }
                    Main.time = fullTime;
                    // In multiplayer, sync the world data.
                    // if (Main.netMode == NetmodeID.Server)
                    // {
                    //     NetMessage.SendData(MessageID.WorldData);
                    // }
                });

            moon = AddOptionPanel(
                title: "Moon Phase: ",
                description: "Increase the moon phase by 1",
                checkBox: false,
                color: Color.Red,
                onClick: () =>
                {
                    Main.moonPhase++;
                    if (Main.moonPhase >= 8)
                    {
                        Main.moonPhase = 0;
                    }
                }
        );


        }

        private string CalcIngameTime()
        {
            // Unused variable from original code (num = 0)
            string text3 = Lang.inter[95].Value;
            string textValue = Language.GetTextValue("GameUI.TimeAtMorning");

            // Get the current time in ticks and adjust if it's night
            double currentTime = Main.time;
            if (!Main.dayTime)
            {
                currentTime += 54000.0;
            }

            // Convert the current time (0-86400 ticks) to hours (0-24)
            currentTime = currentTime / 86400.0 * 24.0;

            // Adjust the time relative to the morning reference:
            // Subtract a reference offset (7.5) and then shift by 12 hours
            double referenceOffset = 7.5;
            currentTime = currentTime - referenceOffset - 12.0;
            if (currentTime < 0.0)
            {
                currentTime += 24.0;
            }

            // Change the text value based on whether it's before or after morning
            if (currentTime >= 12.0)
            {
                textValue = Language.GetTextValue("GameUI.TimePastMorning");
            }

            // Extract hours and minutes
            int hours = (int)currentTime;
            double minuteFraction = currentTime - hours;
            int minutes = (int)(minuteFraction * 60.0);

            // Format minutes to always have two digits (no leading zeros for hours)
            string minuteStr = minutes.ToString("D2");

            // Convert to 12-hour clock format
            if (hours > 12)
            {
                hours -= 12;
            }
            if (hours == 0)
            {
                hours = 12;
            }

            // Adjust minutes based on the player's watch accessory settings
            if (Main.player[Main.myPlayer].accWatch == 1)
            {
                minuteStr = "00";
            }
            else if (Main.player[Main.myPlayer].accWatch == 2)
            {
                minuteStr = (minutes < 30) ? "00" : "30";
            }

            return hours + ":" + minuteStr + " " + textValue;
        }

        public override void Update(GameTime gameTime)
        {
            moon.SetText("Moon Phase: " + Main.moonPhase);
            time.SetText("Time: " + CalcIngameTime());

            base.Update(gameTime);
        }
    }
}
