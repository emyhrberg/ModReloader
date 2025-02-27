using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SquidTestingMod.Common.Systems;
using SquidTestingMod.Helpers;
using SquidTestingMod.UI.Panels;
using Terraria;
using Terraria.GameContent.Events;
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
        OnOffOption moon;
        OnOffOption difficulty;
        SliderOption timeOption;
        bool timeSliderActive = false;
        bool enableSpawning = true;

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

            AddHeader("World Info");
            AddOnOffOption(null, "Name: " + worldName);
            AddOnOffOption(null, "Size: " + worldSize);
            difficulty = AddOnOffOption(ChangeDifficulty, "Difficulty: " + difficultyText);
            AddPadding();

            AddHeader("Time");
            timeOption = AddSliderOption("Time", 0f, 1f, GetCurrentTimeNormalized(), UpdateInGameTime);
            timeOption.textElement.HAlign = 0.05f;
            moon = AddOnOffOption(IncreaseMoonphase, $"Moon Phase: {Main.moonPhase} ({GetMoonPhaseName()})", "Click to cycle moon phases", DecreaseMoonphase);
            AddPadding();

            AddHeader("Weather");
            AddOnOffOption(ToggleRain, "Rain Off");
            AddOnOffOption(ToggleSandstorm, "Sandstorm Off");
            AddPadding();

            AddHeader("Evil");
            AddOnOffOption(null, "Infection Spread Off");
            AddOnOffOption(null, "Crimson/Corruption: 0%");
            AddOnOffOption(null, "Hallow: 0%");
            AddPadding();

            AddHeader("Enemies");
            AddOnOffOption(() => ToggleEnemySpawnRate(), "Enemies Can Spawn: On");
            AddPadding();

            AddHeader("Peaceful Events");
            AddOnOffOption(BirthdayParty.ToggleManualParty, "Party Off");
            AddOnOffOption(LanternNight.ToggleManualLanterns, "Lantern Night Off");
            AddPadding();

            AddHeader("Pre-HM Events");
            AddOnOffOption(StartBloodMoon, "Start Blood Moon");
            AddOnOffOption(() => TryStartInvasion(InvasionID.GoblinArmy), "Start Goblin Invasion");
            AddOnOffOption(SpawnSlimeRain, "Start Slime Rain");
            AddOnOffOption(null, "Start Old One's Army");
            AddOnOffOption(null, "Start Torch God");
            AddPadding();

            AddHeader("HM Events");
            AddOnOffOption(null, "Start Frost Legion");
            AddOnOffOption(ToggleSolarEclipse, "Start Solar Eclipse");
            AddOnOffOption(() => TryStartInvasion(InvasionID.PirateInvasion), "Start Pirate Invasion");
            AddOnOffOption(() => TryStartInvasion(InvasionID.CachedPumpkinMoon), "Start Pumpkin Moon");
            AddOnOffOption(() => TryStartInvasion(InvasionID.CachedFrostMoon), "Start Frost Moon");
            AddOnOffOption(() => TryStartInvasion(InvasionID.MartianMadness), "Start Martian Madness");
            AddOnOffOption(null, "Start Lunar Events");
            AddPadding();

            AddHeader("World");
            AddOnOffOption(SpawnMeteor, "Spawn Meteor");
            AddOnOffOption(() => Main.forceXMasForToday = !Main.forceXMasForToday, "Force XMas");
            AddOnOffOption(() => Main.forceHalloweenForToday = !Main.forceHalloweenForToday, "Force Halloween");
            AddPadding();
        }

        private string GetInvasionName(int invasionType)
        {
            return invasionType switch
            {
                InvasionID.GoblinArmy => "Goblin Army Invasion",
                InvasionID.PirateInvasion => "Pirate Invasion",
                InvasionID.CachedPumpkinMoon => "Pumpkin Moon",
                // InvasionID.CachedFrostMoon => "Frost Moon",
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
                // TODO set time to 7:30 PM
                // Main.dayTime = false;
                // Main.time = 48600.0;

                Main.bloodMoon = true;
                Main.NewText("Starting Blood Moon...");
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

        private void ChangeDifficulty()
        {
            // Cycle through the different difficulty settings
            Main.ActiveWorldFileData.GameMode = Main.ActiveWorldFileData.GameMode switch
            {
                GameModeID.Normal => GameModeID.Expert,
                GameModeID.Expert => GameModeID.Master,
                GameModeID.Master => GameModeID.Normal,
                _ => Main.ActiveWorldFileData.GameMode
            };

            // Update the text element
            difficulty.UpdateText("Difficulty: " + Main.ActiveWorldFileData.GameMode switch
            {
                GameModeID.Normal => "Normal",
                GameModeID.Expert => "Expert",
                GameModeID.Master => "Master",
                GameModeID.Creative => "Journey",
                _ => "Unknown Difficulty"
            });
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

        private void ToggleEnemySpawnRate()
        {
            enableSpawning = !enableSpawning;

            if (!enableSpawning)
            {
                // Butcher all hostile NPCs
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].CanBeChasedBy()) // Checks if it's a hostile NPC
                    {
                        Main.npc[i].life = 0;
                        Main.npc[i].HitEffect();
                        Main.npc[i].active = false;
                        // NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, i); // Sync NPC despawn
                    }
                }

                // Set spawn rate to 0x (disable spawning)
                SpawnRateMultiplier.Multiplier = 0f;
                Main.NewText("Enemy spawn rate disabled. All hostiles removed.", 255, 0, 0);
            }
            else
            {
                // Restore normal spawn rate (1x)
                SpawnRateMultiplier.Multiplier = 1f;
                Main.NewText("Enemy spawn rate set to normal (1x).", 0, 255, 0);
            }
        }

        private void ToggleSandstorm()
        {
            Sandstorm.Happening = !Sandstorm.Happening;
            Main.NewText(Sandstorm.Happening ? "Starting Sandstorm..." : "Ending Sandstorm...");
        }

        private void ToggleRain()
        {
            Main.raining = !Main.raining;
            Main.maxRaining = Main.raining ? 1f : 0f;
            Main.NewText(Main.raining ? "Starting Rain..." : "Ending Rain...");
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
