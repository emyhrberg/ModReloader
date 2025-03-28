using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using ModHelper.Common.Configs;
using ModHelper.Common.Systems;
using ModHelper.Helpers;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.Events;
using Terraria.ID;
using static ModHelper.UI.Elements.OptionElement;

namespace ModHelper.UI.Elements
{
    public class WorldPanel : OptionPanel
    {
        // Time and sliders
        private bool timeSliderActive = false;
        private SliderPanel timeSlider;
        private SliderPanel townNpcSlider;
        public SliderPanel spawnRateSlider;
        public SliderPanel rainSlider;
        public SliderPanel windSlider;

        // Tracking & Hitboxes
        private OptionElement toggleAllTracking;
        public List<OptionElement> trackingOptions = [];
        private OptionElement toggleAllHitboxes;
        public List<OptionElement> hitboxOptions = [];

        // Actions on slider texts:
        // Clickable spawn rates are: 0, 1, 10, 30
        // Clickable rain rates are: 0, 0.1, 0.5, 1
        // Click on Town NPC does nothing
        private List<float> spawnRates = [0, 1, 2, 5, 10, 30];
        private List<float> windRates = [-1.2f, -0.6f, 0, 0.6f, 1.2f];
        private List<float> rainRates = [0, 0.1f, 0.2f, 0.6f, 1];
        private float currentSpawnRate = 1;
        private float currentRainRate = 0f;
        private float currentWindRate = 0f;
        private Dictionary<float, string> rainStrings = new()
        {
            {0, "No Rain"},
            {0.1f, "Light Rain"},
            {0.2f, "Rain"},
            {0.3f, "Rain"},
            {0.4f, "Rain"},
            {0.5f, "Rain"},
            {0.6f, "Heavy Rain"},
            {0.7f, "Heavy Rain"},
            {0.8f, "Heavy Rain"},
            {0.9f, "Heavy Rain"},
            {1, "Storm"}
        };

        #region Constructor
        public WorldPanel() : base(title: "World", scrollbarEnabled: true)
        {
            AddPadding(5);
            AddHeader("World");
            timeSlider = new(
                title: "Time",
                min: 0f,
                max: 1f,
                defaultValue: GetCurrentTimeNormalized(),
                onValueChanged: UpdateInGameTime,
                increment: 1800f / 86400f,
                hover: "Click to freeze time",
                textSize: 0.9f,
                leftClickText: ToggleFreezeTime,
                rightClickText: () => Main.NewText("No right click action")
            );
            uiList.Add(timeSlider);
            AddPadding(3f);

            spawnRateSlider = new(
                title: "Spawn Rate",
                min: 0,
                max: 30,
                defaultValue: SpawnRateMultiplier.Multiplier,
                onValueChanged: SpawnRateMultiplier.SetSpawnRateMultiplier,
                increment: 1,
                hover: "Set the spawn rate multiplier",
                textSize: 0.9f,
                leftClickText: () =>
                {
                    currentSpawnRate = spawnRates[(spawnRates.IndexOf(currentSpawnRate) + 1) % spawnRates.Count];
                    SpawnRateMultiplier.Multiplier = currentSpawnRate;
                    spawnRateSlider.SetValue(currentSpawnRate); // set normalized value 0 to 1
                    // set normalized value 0 to 1
                    ChatHelper.NewText("Spawn rate set to " + currentSpawnRate);
                },
                rightClickText: () =>
                {
                    currentSpawnRate = spawnRates[(spawnRates.IndexOf(currentSpawnRate) + spawnRates.Count - 1) % spawnRates.Count];
                    SpawnRateMultiplier.Multiplier = currentSpawnRate;
                    spawnRateSlider.SetValue(currentSpawnRate);
                }
            );
            uiList.Add(spawnRateSlider);
            AddPadding(3f);

            rainSlider = new(
                title: "Rain",
                min: 0,
                max: 1,
                defaultValue: Main.maxRaining,
                onValueChanged: UpdateRainSlider,
                increment: 0.1f,
                hover: "Set rain intensity",
                textSize: 0.9f,
                valueFormatter: value =>
                {
                    // round value to nearest 0.1
                    value = (float)Math.Round(value, 1);
                    return rainStrings[value];
                },
                leftClickText: () =>
                {
                    currentRainRate = rainRates[(rainRates.IndexOf(currentRainRate) + 1) % rainRates.Count];
                    // round to nearest 0.1
                    currentRainRate = (float)Math.Round(currentRainRate, 1);

                    // Main.rainTime = 3600; // Set a reasonable duration for rain
                    Main.maxRaining = currentRainRate;
                    Main.cloudAlpha = currentRainRate;
                    rainSlider.SetValue(currentRainRate);
                    ChatHelper.NewText("Rain set to " + rainStrings[currentRainRate]);
                },
                rightClickText: () =>
                {
                    currentRainRate = rainRates[(rainRates.IndexOf(currentRainRate) + rainRates.Count - 1) % rainRates.Count];

                    // round to nearest 0.1
                    // Main.rainTime = 3600; // Set a reasonable duration for rain
                    Main.maxRaining = currentRainRate;
                    Main.cloudAlpha = currentRainRate;
                    rainSlider.SetValue(currentRainRate);
                }
            );
            uiList.Add(rainSlider);
            AddPadding(3f);


            windSlider = new(
                title: "Wind",
                min: -1.2f, // -1.2f is -60 mph
                max: 1.2f,  // 1.2f is 60 mph
                defaultValue: MathHelper.Clamp(Main.windSpeedCurrent, -1.2f, 1.2f),
                onValueChanged: UpdateWindSlider,
                increment: 0.05f, // This gives us ~24 increments across the range, good precision for mph
                hover: "Set wind speed",
                textSize: 0.9f,
                leftClickText: () =>
                {
                    currentWindRate = windRates[(windRates.IndexOf(currentWindRate) + 1) % windRates.Count];
                    Main.windSpeedCurrent = currentWindRate;
                    Main.windSpeedTarget = currentWindRate; // Also set target to avoid drift
                    windSlider.SetValue(currentWindRate);

                    // prettify wind rate to -60 to 60 mph
                    string direction = currentWindRate switch
                    {
                        < 0f => "E",
                        > 0f => "W",
                        _ => ""
                    };

                    ChatHelper.NewText($"Wind set to {Math.Abs(currentWindRate * 60):F0} mph {direction}");
                },
                rightClickText: () =>
                {
                    currentWindRate = windRates[(windRates.IndexOf(currentWindRate) + windRates.Count - 1) % windRates.Count];
                    Main.windSpeedCurrent = currentWindRate;
                    Main.windSpeedTarget = currentWindRate; // Also set target to avoid drift
                    windSlider.SetValue(currentWindRate);
                },
                valueFormatter: value =>
                {
                    // -1.2 to 1.2 represents -60 to 60 mph
                    string direction = value switch
                    {
                        < -0.1f => "E",
                        > 0.1f => "W",
                        _ => ""
                    };
                    return $"{Math.Abs(value * 50):F0} mph {direction}";
                }
            );

            uiList.Add(windSlider);
            AddPadding(3f);

            // Town NPCs
            // Force at least 1 for the max, just so the slider has a range
            int numberOfTownNPCs = GetTownNpcCount();
            if (numberOfTownNPCs <= 0)
            {
                numberOfTownNPCs = 0;
            }

            townNpcSlider = new(
                title: "Town NPCs",
                defaultValue: 1,
                min: 0f,
                max: 1f,
                onValueChanged: UpdateTownNpcSlider,
                increment: 1f,
                hover: "Set the number of town NPCs",
                textSize: 0.9f
            );
            uiList.Add(townNpcSlider);
            AddPadding();

            // tracking
            AddHeader("Tracking");
            toggleAllTracking = AddOption("Toggle All", ToggleAllTracking, "Toggle all tracking");
            foreach (var tracking in TrackingSystem.Trackings)
            {
                var option = new OptionElement(
                    leftClick: tracking.Toggle,
                    text: tracking.Name,
                    hover: tracking.TooltipHoverText
                );
                trackingOptions.Add(option);
                uiList.Add(option);
                AddPadding(3f);
            }
            AddPadding();

            AddHeader("Hitboxes");
            toggleAllHitboxes = AddOption("Toggle All", ToggleAllHitboxes, "Toggle all hitboxes");
            foreach (var hitbox in HitboxSystem.Hitboxes)
            {
                var option = new OptionElement(
                    leftClick: hitbox.Toggle,
                    text: hitbox.Name,
                    hover: hitbox.HoverTooltipText
                );
                hitboxOptions.Add(option);
                uiList.Add(option);
                AddPadding(3f);
            }
            AddPadding();

            AddHeader("Misc");

            string playerPath = Main.ActivePlayerFileData.Path;
            string playerName = Path.GetFileName(playerPath);
            AddAction(savePlayer, "Save Player", $"Save the current player file as '{playerName}' \nRight click to open folder", rightClick: () => OpenFolder(Main.ActivePlayerFileData.Path));

            string worldPath = Main.ActiveWorldFileData.Path;
            string worldName = Path.GetFileName(worldPath);
            AddAction(saveWorld, "Save World", $"Save the current world file as '{worldName}'\nRight click to open folder", rightClick: () => OpenFolder(Main.ActiveWorldFileData.Path));

            AddAction(StopInvasion, "Stop Invasion", "Stop the current invasion if one is in progress", rightClick: () => Main.NewText("No right click action"));

            // invasion / toggles
            // AddHeader("Invasions");
            // AddOption("Party", ToggleParty, "Start a party in the world");
            // AddOption("Slime Rain", ToggleSlimeRain, "Start or stop slime rain");
            // AddOption("Blood Moon", StartBloodMoon, "Start or stop a blood moon");
            // AddOption("Solar Eclipse", ToggleSolarEclipse, "Start or stop a solar eclipse");
            // AddPadding(5);

            // // invasions
            // AddHeader("More Invasions");
            // AddAction(() => TryStartInvasion(InvasionID.GoblinArmy), "Goblin Army", "Start Goblin Army invasion");
            // AddAction(() => TryStartInvasion(InvasionID.PirateInvasion), "Pirate Invasion", "Start Pirate Invasion");
            // AddAction(() => TryStartInvasion(InvasionID.CachedPumpkinMoon), "Pumpkin Moon", "Start Pumpkin Moon invasion");
            // AddAction(() => TryStartInvasion(InvasionID.CachedFrostMoon), "Frost Moon", "Start Frost Moon invasion");
            // AddAction(() => TryStartInvasion(InvasionID.MartianMadness), "Martian Madness", "Start Martian Madness invasion");
            // AddPadding(5);
            // AddAction(() => TryStartInvasion(InvasionID.CachedOldOnesArmy), "Old Ones Army", "Start Old Ones Army invasion");
            // AddAction(StopInvasion, "Stop Invasion", "Stop the current invasion");
        }
        #endregion // end of constructor

        #region Freeze Time
        private void ToggleFreezeTime()
        {
            FreezeTimeManager.FreezeTime = !FreezeTimeManager.FreezeTime;
            ChatHelper.NewText("Time is now " + (FreezeTimeManager.FreezeTime ? "frozen" : "unfrozen"));
            timeSlider.optionTitle.hover = FreezeTimeManager.FreezeTime ? "Click to unfreeze time" : "Click to freeze time";
        }
        #endregion

        #region Tracking
        private void ToggleAllTracking()
        {
            // Decide whether to enable or disable everything
            bool newVal = !TrackingSystem.AreAllActive;
            TrackingSystem.SetAll(newVal);

            // Update each option’s UI text
            State newState = newVal ? State.Enabled : State.Disabled;
            foreach (OptionElement option in trackingOptions)
            {
                option.SetState(newState);
            }
            // Set itself
            toggleAllTracking.SetState(newState);
        }

        private void ToggleAllHitboxes()
        {
            // Decide whether to enable or disable everything
            bool newVal = !HitboxSystem.AreAllActive;
            HitboxSystem.SetAllHitboxes(newVal);

            // Update each option’s UI text
            State newState = newVal ? State.Enabled : State.Disabled;
            foreach (OptionElement option in hitboxOptions)
            {
                option.SetState(newState);
            }
            // Set itself
            toggleAllHitboxes.SetState(newState);
        }

        #endregion

        private void UpdateRainSlider(float newValue)
        {
            // Round to nearest 0.1
            newValue = (float)Math.Round(newValue, 1);

            if (newValue == 0)
            {
                // Stop rain completely
                Main.rainTime = 0;
                Main.StopRain();
                Main.cloudAlpha = 0f;
                Main.maxRaining = 0f;
                return;
            }

            // Start rain if it's not already raining
            if (!Main.raining)
            {
                Main.StartRain();
            }

            // Set rain and cloud intensity based on slider value (0.01-1.0)
            Main.maxRaining = newValue;
            Main.cloudAlpha = newValue;
            // Main.rainTime = 3600; // Set a reasonable duration for rain
        }

        private void UpdateWindSlider(float newValue)
        {
            // newValue is already in our desired range of -60 to 60
            // so we can directly set it to the current and target wind speeds
            Log.SlowInfo($"Wind Slider: {newValue}");

            Main.windSpeedCurrent = newValue;
            Main.windSpeedTarget = newValue;

            // Update our tracking variable to match
            currentWindRate = newValue;
        }

        private void UpdateTownNpcSlider(float newValue)
        {
            int targetCount = (int)newValue;
            int currentCount = GetTownNpcCount();

            // If the slider target is lower than the current count, kill one NPC only
            if (targetCount < currentCount)
            {
                var activeTownNpcs = Main.npc.Where(npc => npc.active && npc.townNPC).ToList();
                if (activeTownNpcs.Count > 0)
                {
                    int index = Main.rand.Next(activeTownNpcs.Count);
                    activeTownNpcs[index].StrikeInstantKill();
                }
            }
        }

        private int GetTownNpcCount() => Main.npc.Where(npc => npc.active && npc.townNPC).Count();

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
            base.Update(gameTime);

            // Update spawn rate slider hover text to say the spawn rate and the max spawns
            // every half second ish scuffed
            if (true)
            // if (Main.GameUpdateCount % 30 == 0)
            {
                // Update rain slider text
                if (!CustomSliderBase.IsAnySliderLocked)
                {
                    // Clamp the rain rate to our desired range
                    float clampedRainRate = MathHelper.Clamp(Main.maxRaining, 0f, 1f);
                    rainSlider.SetValue(clampedRainRate);
                }

                // Update wind slider text
                if (!CustomSliderBase.IsAnySliderLocked)
                {
                    // Clamp the wind speed to our desired range
                    float clampedWindSpeed = MathHelper.Clamp(Main.windSpeedCurrent, -1.2f, 1.2f);
                    windSlider.SetValue(clampedWindSpeed);
                }

                // Update the UI display for wind
                string windDirection = Main.windSpeedCurrent switch
                {
                    < 0f => "E",
                    > 0f => "W",
                    _ => ""
                };
                // windSlider.UpdateText($"Wind: {Math.Abs(Main.windSpeedCurrent * 50):F0} mph {windDirection}");

                // Update the hover text for the spawn rate slider
                spawnRateSlider.optionTitle.hover = $"Spawn Rate: {SpawnRateHook.StoredSpawnRate} (number of frames between spawn attempts)\nMax Spawns: {SpawnRateHook.StoredMaxSpawns} (max number of enemies in the world)";

                // Update the hover text town npc slider
                // If 0 town NPCs, show the default "Set the number of town NPCs" text
                // If more than 0 town NPCs, show the names of every town NPC sorted alphabetically and only typename.
                var townNPCs = Main.npc.Where(npc => npc.active && npc.townNPC).ToList();
                if (townNPCs.Count > 0)
                {
                    var npcNames = townNPCs.Select(npc => npc.TypeName).OrderBy(name => name).ToList();
                    var formattedNames = string.Join("\n", npcNames
                        .Select((name, index) => (name, index))
                        .GroupBy(x => x.index / 5)
                        .Select(group => string.Join(", ", group.Select(x => x.name)) + ","));

                    // Remove the trailing comma from the last row
                    if (formattedNames.EndsWith(","))
                    {
                        formattedNames = formattedNames.TrimEnd(',');
                    }

                    townNpcSlider.optionTitle.hover = "Town NPCs:\n" + formattedNames;
                }
                else
                {
                    townNpcSlider.optionTitle.hover = "Town NPCs:\nSet the number of town NPCs";
                }

                // Update the town NPC count
                if (!CustomSliderBase.IsAnySliderLocked)
                {
                    // Slider is not being used, update the max value
                    townNpcSlider.UpdateSliderMax(GetTownNpcCount());
                }
                else
                {
                    // Slider is being used, update the current value
                    townNpcSlider.SetValue(GetTownNpcCount());
                    //townNpcSlider.UpdateText("Town NPCs: " + GetTownNpcCount());
                    // disable mouse input
                    Main.LocalPlayer.mouseInterface = true;
                }

                // Update the time
                if (!timeSliderActive && Main.GameUpdateCount % 60 == 0)
                {
                    // Normal game time progression logic
                    timeSlider.SetValue(GetCurrentTimeNormalized());
                }
                else
                {
                    // Reset the flag after slider use (prevents conflicts)
                    timeSliderActive = false;
                }

                // Update the UI display
                timeSlider.UpdateText("Time: " + CalcIngameTime());
            }
        }

        #region Save
        // To use :
        // Main.ActiveWorldFileData.SaveWorld()?
        // Main.ActivePlayerFileData.SavePlayer()?
        // PlayerFileData.Path

        public void savePlayer()
        {
            // SAVE!
            WorldGen.saveToonWhilePlaying();

            // Write to chat that we saved the player to a path
            string filePath = Main.ActivePlayerFileData.Path;
            Main.NewText("Player saved to: " + filePath);
        }

        public void saveWorld()
        {
            // SAVE!
            WorldGen.saveAndPlay();

            // Write to chat that we saved the world to a path
            string filePath = Main.ActiveWorldFileData.Path;
            Main.NewText("World saved to: " + filePath);
        }

        #endregion

        private void OpenFolder(string path)
        {
            // go up one directory
            path = Path.GetDirectoryName(path);
            Process.Start(new ProcessStartInfo($@"{path}") { UseShellExecute = true });
        }

        #region Invasions

        private static string GetInvasionName(int invasionType)
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

        private static void TryStartInvasion(int invasionType)
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

        private static void StartBloodMoon()
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

        private static void ToggleSlimeRain()
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

        private static void ToggleParty()
        {
            BirthdayParty.ToggleManualParty();
        }

        private void ToggleSolarEclipse()
        {
            Main.eclipse = !Main.eclipse;
            Main.NewText(Main.eclipse ? "Starting Solar Eclipse..." : "Ending Solar Eclipse...");
        }

        private static void StopInvasion()
        {
            Main.NewText("Stopping invasion...");
            Main.invasionType = 0;
            Main.invasionProgress = 0;
        }

        #endregion
    }
}