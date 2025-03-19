using System;
using System.Linq;
using Microsoft.Xna.Framework;
using SquidTestingMod.Common.Systems;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;

namespace SquidTestingMod.UI.Elements
{
    /// <summary>
    /// A panel containing options to modify player behaviour like God, Fast, Build, etc.
    /// </summary>
    public class WorldPanel : OptionPanel
    {
        // Variables
        // private string worldName = "";
        private OnOffOption moon;
        private OnOffOption difficulty;
        private SliderOption timeOption;
        private bool timeSliderActive = false;
        private SliderOption townNpcSlider; // New slider for town NPCs

        public WorldPanel() : base(title: "World", scrollbarEnabled: true)
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

            // AddHeader("World Info");
            // AddOnOffOption(null, "Name: " + worldName);
            // AddOnOffOption(null, "Size: " + worldSize);


            AddHeader("World Name: " + worldName);
            AddHeader("Size: " + worldSize);
            difficulty = AddOnOffOption(CycleDifficultyForward, "Difficulty: " + difficultyText, "Click to cycle difficulty", rightClick: CycleDifficultyBackward);
            AddPadding();

            AddHeader("Time");
            timeOption = new("Time", 0f, 1f, GetCurrentTimeNormalized(), UpdateInGameTime, 1800f / 86400f, hover: "Click and drag to change time");
            moon = AddOnOffOption(IncreaseMoonphase, $"Moon Phase: {Main.moonPhase} ({GetMoonPhaseName()})", "Click to cycle moon phases", DecreaseMoonphase);
            uiList.Add(timeOption);
            AddPadding();

            AddHeader("Enemies");
            SliderOption spawnRate = new("Spawn Rate", 0, 30, SpawnRateMultiplier.Multiplier, SpawnRateMultiplier.SetSpawnRateMultiplier, increment: 1, hover: "Set the spawn rate multiplier");
            uiList.Add(spawnRate);

            // Town NPCs
            // Force at least 1 for the max, just so the slider has a range
            int numberOfTownNPCs = Main.npc.Count(npc => npc.active && npc.townNPC);
            float maxValue = Math.Max(1f, numberOfTownNPCs);
            townNpcSlider = new(
                "Town NPCs",
                0f,                   // min
                maxValue,             // max
                maxValue,             // default value
                UpdateTownNpcSlider,  // callback
                increment: 1f,                   // increment
                hover: "Set the number of town NPCs (scuffed)"
            );
            uiList.Add(townNpcSlider);

            // tracking
            AddOnOffOption(DebugEnemyTrackingSystem.ToggleEnemyTracking, "Track Enemies Off", "Show all enemies position with an arrow");
            AddOnOffOption(DebugEnemyTrackingSystem.ToggleCritterTracking, "Track Critters Off", "Show all critters position with an arrow");
            AddOnOffOption(DebugEnemyTrackingSystem.ToggleTownNPCTracking, "Track Town NPCs Off", "Show all town NPCs position with an arrow");

            AddPadding();

            AddHeader("Pre-HM Events");
            AddOnOffOption(BirthdayParty.ToggleManualParty, "Party Off");
            AddOnOffOption(StartBloodMoon, "Start Blood Moon", "Start a Blood Moon event and set the time to 7:30 PM");
            AddOnOffOption(() => TryStartInvasion(InvasionID.GoblinArmy), "Start Goblin Invasion");
            AddOnOffOption(SpawnSlimeRain, "Start Slime Rain");
            // AddOnOffOption(null, "Start Old One's Army (todo)");
            // AddOnOffOption(null, "Start Torch God (todo)");
            AddOnOffOption(TryStopInvasion, "Stop Invasion");
            AddPadding();

            AddHeader("HM Events");
            // AddOnOffOption(null, "Start Frost Legion (todo)");
            AddOnOffOption(ToggleSolarEclipse, "Start Solar Eclipse");
            AddOnOffOption(() => TryStartInvasion(InvasionID.PirateInvasion), "Start Pirate Invasion");
            AddOnOffOption(() => TryStartInvasion(InvasionID.CachedPumpkinMoon), "Start Pumpkin Moon");
            AddOnOffOption(() => TryStartInvasion(InvasionID.CachedFrostMoon), "Start Frost Moon");
            AddOnOffOption(() => TryStartInvasion(InvasionID.MartianMadness), "Start Martian Madness");
            // AddOnOffOption(null, "Start Lunar Events (todo)");
            AddOnOffOption(TryStopInvasion, "Stop Invasion");
            AddPadding();

            AddHeader("World");
            AddOnOffOption(SpawnMeteor, "Spawn Meteor");
            AddOnOffOption(() => Main.forceXMasForToday = !Main.forceXMasForToday, "Force XMas");
            AddOnOffOption(() => Main.forceHalloweenForToday = !Main.forceHalloweenForToday, "Force Halloween");
            AddPadding();
        }

        private void UpdateTownNpcSlider(float newValue)
        {
            int targetCount = (int)newValue;
            int currentCount = GetTownNpcCount();

            if (targetCount < currentCount)
            {
                int countToKill = currentCount - targetCount;
                KillTownNPCs(countToKill);
            }

            // Re-sync the slider in case the count actually changed
            int updatedCount = GetTownNpcCount();
            townNpcSlider.SetValue(updatedCount);
            townNpcSlider.UpdateText("Town NPCs: " + updatedCount);
        }

        private int GetTownNpcCount() => Main.npc.Count(npc => npc.active && npc.townNPC);


        // Helper method to “kill” a given number of town NPCs.
        private void KillTownNPCs(int count)
        {
            foreach (NPC npc in Main.npc)
            {
                if (count <= 0)
                    break;

                if (npc.active && npc.townNPC)
                {
                    // Use StrikeNPC with damage equal to npc.lifeMax to ensure death.
                    npc.StrikeInstantKill();
                    count--;
                }
            }
        }

        private void TryStopInvasion()
        {
            Main.NewText("Stopping invasion...");
            Main.invasionType = 0;
            Main.invasionProgress = 0;
        }

        private string GetInvasionName(int invasionType)
        {
            return invasionType switch
            {
                InvasionID.GoblinArmy => "Goblin Army Invasion",
                InvasionID.PirateInvasion => "Pirate Invasion",
                InvasionID.CachedPumpkinMoon => "Pumpkin Moon",
                InvasionID.MartianMadness => "Martian Madness",
                _ => "Unknown Invasion"
            };
        }

        private void ToggleSolarEclipse()
        {
            Main.eclipse = !Main.eclipse;
            Main.NewText(Main.eclipse ? "Starting Solar Eclipse..." : "Ending Solar Eclipse...");
        }

        private void StartOldOnesArmy()
        {
            Main.NewText("Starting Old One's Army...");
        }

        private void StartBloodMoon()
        {
            if (Main.bloodMoon)
            {
                Main.bloodMoon = false;
                Main.NewText("Ending Blood Moon...");
            }
            else
            {
                // set time to 7:29 PM
                Main.dayTime = true;
                Main.time = 53999.0;

                Main.bloodMoon = true;
                Main.NewText("Starting Blood Moon... (and night)");
            }
        }

        private void SpawnSlimeRain()
        {
            // check status of slime rain and toggle it
            if (Main.slimeRain)
            {
                Main.StopSlimeRain();
                Main.NewText("Ending Slime Rain...");
            }
            else
            {
                Main.StartSlimeRain();
                Main.NewText("Starting Slime Rain...");
            }
        }

        private void UpdateDifficultyText()
        {
            string diffText = Main.GameMode switch
            {
                GameModeID.Normal => "Normal",
                GameModeID.Expert => "Expert",
                GameModeID.Master => "Master",
                GameModeID.Creative => "Journey",
                _ => "Unknown Difficulty"
            };
            difficulty.UpdateText("Difficulty: " + diffText);
        }

        private void CycleDifficultyForward()
        {
            Main.GameMode = Main.GameMode switch
            {
                GameModeID.Normal => GameModeID.Expert,
                GameModeID.Expert => GameModeID.Master,
                GameModeID.Master => GameModeID.Normal,
                _ => Main.GameMode
            };
            UpdateDifficultyText();
        }

        private void CycleDifficultyBackward()
        {
            Main.GameMode = Main.GameMode switch
            {
                GameModeID.Normal => GameModeID.Master,
                GameModeID.Expert => GameModeID.Normal,
                GameModeID.Master => GameModeID.Expert,
                _ => Main.GameMode
            };
            UpdateDifficultyText();
        }

        private void SpawnMeteor()
        {
            Main.NewText("Trying to spawn meteor...");
            Log.Info("Trying to spawn meteor...");
            WorldGen.dropMeteor();
        }

        private void DecreaseMoonphase()
        {
            Main.moonPhase--;
            if (Main.moonPhase < 0)
            {
                Main.moonPhase = 7;
            }
            // Set the text element
            moon.UpdateText("Moon Phase: " + Main.moonPhase + " (" + GetMoonPhaseName() + ")");
        }

        private void IncreaseMoonphase()
        {
            Main.moonPhase++;
            if (Main.moonPhase >= 8)
            {
                Main.moonPhase = 0;
            }
            // Set the text element
            moon.UpdateText("Moon Phase: " + Main.moonPhase + " (" + GetMoonPhaseName() + ")");
        }

        private void TryStartInvasion(int invasionType)
        {
            if (Main.CanStartInvasion(invasionType, ignoreDelay: true))
            {
                if (Main.invasionType == 0)
                {
                    Main.invasionDelay = 0;
                    Main.StartInvasion(invasionType);
                    Main.NewText($"Starting {GetInvasionName(invasionType)}... ");
                }
                else
                {
                    Main.NewText("An invasion is already in progress!");
                }
            }
            else
            {
                Main.NewText("An invasion cannot be started.");
            }
        }

        private float GetCurrentTimeNormalized()
        {
            // Get the total time in ticks
            double totalTicks = Main.time;

            if (!Main.dayTime)
            {
                // If it's nighttime, add 54,000 ticks (full daytime length) to make time continuous
                totalTicks += Main.dayLength;
            }

            // Normalize to a 0-1 scale based on 86,400 ticks per full day
            float normalizedTime = (float)(totalTicks / 86400.0);

            // Get the in-game formatted time
            string formattedTime = CalcIngameTime();

            // Log information for debugging
            // Log.Info($"[DEBUG] In-Game Time: {formattedTime}, Ticks: {totalTicks}, Normalized: {normalizedTime:F6}");

            return normalizedTime;
        }

        private string CalcIngameTime()
        {
            string textValueAMorPM = "AM"; // Default is morning

            // Get the current time in ticks and adjust if it's night
            double currentTime = Main.time;
            if (!Main.dayTime)
            {
                currentTime += 54000.0;
            }

            // Convert ticks (0-86400) to hours (0-24)
            currentTime = currentTime / 86400.0 * 24.0;

            // Offset by Terraria’s reference time (7:30 AM starts at 0 ticks)
            double referenceOffset = 7.5;
            currentTime = currentTime - referenceOffset - 12.0;

            // Wrap around midnight
            if (currentTime < 0.0)
            {
                currentTime += 24.0;
            }

            // Convert to 12-hour format
            if (currentTime >= 12.0)
            {
                textValueAMorPM = "PM";
            }

            int hours = (int)currentTime;
            int minutes = (int)((currentTime - hours) * 60.0);

            // Format to 12-hour clock
            if (hours > 12) hours -= 12;
            if (hours == 0) hours = 12;

            return $"{hours}:{minutes:D2} {textValueAMorPM}";
        }

        private string GetMoonPhaseName()
        {
            return Main.moonPhase switch
            {
                0 => "Full Moon",
                1 => "Waning Gibbous",
                2 => "Third Quarter",
                3 => "Waning Crescent",
                4 => "New Moon",
                5 => "Waxing Crescent",
                6 => "First Quarter",
                7 => "Waxing Gibbous",
                _ => "Unknown Moon Phase"
            };
        }

        private void UpdateInGameTime(float normalizedValue)
        {
            timeSliderActive = true;

            // Convert slider value to in-game time (0 to 86400 ticks)
            double totalTicks = normalizedValue * 86400.0;

            // Determine if it's day or night and adjust `Main.time`
            if (totalTicks < 54000.0) // Daytime threshold (0 to 53999)
            {
                Main.dayTime = true;
                Main.time = totalTicks;
            }
            else // Nighttime threshold (54000 to 86399)
            {
                Main.dayTime = false;
                Main.time = totalTicks - 54000.0;
            }

            // Get formatted in-game time
            // string formattedTime = CalcIngameTime();

            // Log the new time to the console
            // Log.Info($"Slider adjusted: Normalized = {normalizedValue}, In-Game Time = {formattedTime}");

            // Sync in multiplayer (if needed)
            // if (Main.netMode == NetmodeID.Server)
            // {
            //     NetMessage.SendData(MessageID.WorldData);
            // }
        }

        public override void Update(GameTime gameTime)
        {
            // Check if unknown name still. Then we have to update it
            // if (TitlePanel.HeaderText.Text.Length == 5)
            // {
            //     if (Main.ActiveWorldFileData.GetWorldName() != null)
            //     {
            //         TitlePanel.HeaderText.SetText("World: " + Main.ActiveWorldFileData.GetWorldName());
            //         Main.NewText("World name updated to: " + Main.ActiveWorldFileData.GetWorldName());
            //         Log.Info("World name updated to: " + Main.ActiveWorldFileData.GetWorldName());
            //     }
            // }

            if (!timeSliderActive && Main.GameUpdateCount % 60 == 0)
            {
                // Normal game time progression logic
                timeOption.SetValue(GetCurrentTimeNormalized());
            }
            else
            {
                // Reset the flag after slider use (prevents conflicts)
                timeSliderActive = false;
            }

            // Update the UI display
            timeOption.UpdateText("Time: " + CalcIngameTime());
            base.Update(gameTime);
        }
    }
}
