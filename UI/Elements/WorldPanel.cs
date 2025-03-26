using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using EliteTestingMod.Common.Configs;
using EliteTestingMod.Common.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using static EliteTestingMod.UI.Elements.Option;

namespace EliteTestingMod.UI.Elements
{
    public class WorldPanel : OptionPanel
    {
        private SliderPanel timeOption;
        private bool timeSliderActive = false;
        private SliderPanel townNpcSlider;
        public SliderPanel spawnRateSlider;
        public SliderPanel rainSlider;

        #region Constructor
        public WorldPanel() : base(title: "World", scrollbarEnabled: true)
        {
            AddPadding(5);
            AddHeader("World");

            timeOption = AddSlider(
                title: "Time",
                min: 0f,
                max: 1f,
                defaultValue: GetCurrentTimeNormalized(),
                onValueChanged: UpdateInGameTime,
                increment: 1800f / 86400f,
                hover: "Click to freeze time",
                textSize: 0.9f,
                leftClickText: ToggleFreezeTime
            );

            float spawnRate = 1f;
            if (Conf.EnterWorldSuperMode)
            {
                spawnRate = 0f;
            }

            spawnRateSlider = new(
                title: "Spawn Rate",
                min: 0,
                max: 30,
                defaultValue: spawnRate,
                onValueChanged: SpawnRateMultiplier.SetSpawnRateMultiplier,
                increment: 1,
                hover: "Set the spawn rate multiplier",
                textSize: 0.9f,
                leftClickText: () => SpawnRateMultiplier.Multiplier = 0f,
                rightClickText: () => SpawnRateMultiplier.Multiplier = 1f
            );
            uiList.Add(spawnRateSlider);
            AddPadding(3f);

            rainSlider = new(
                title: "Rain",
                min: 0,
                max: 1,
                defaultValue: Main.raining ? 1 : 0,
                onValueChanged: UpdateRainSlider,
                increment: 0.1f,
                hover: "Set rain and cloud intensity",
                textSize: 0.9f
            );
            uiList.Add(rainSlider);
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
                var option = new Option(
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
                var option = new Option(
                    leftClick: hitbox.Toggle,
                    text: hitbox.Name,
                    hover: hitbox.HoverTooltipText
                );
                hitboxOptions.Add(option);
                uiList.Add(option);
                AddPadding(3f);
            }
            AddPadding();

            // save
            AddHeader("Save");

            var savePlayerCurrentOption = new ActionOption(savePlayer, "Player", "Save the current player by overwriting the save file\nRight click to open folder", rightClick: () => OpenFolder(Main.ActivePlayerFileData.Path));
            uiList.Add(savePlayerCurrentOption);
            AddPadding(3f);

            var saveWorldOption = new ActionOption(saveWorld, "World", "Save the current world by overwriting the save file\nRight click to open folder", rightClick: () => OpenFolder(Main.ActiveWorldFileData.Path));
            uiList.Add(saveWorldOption);
            AddPadding(3f);
            AddPadding();
        }
        #endregion // Here is the end of the constructor

        #region Open Folder
        private void OpenFolder(string path)
        {
            // go up one directory
            path = System.IO.Path.GetDirectoryName(path);
            Process.Start(new ProcessStartInfo($@"{path}") { UseShellExecute = true });
        }

        #endregion

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

        #region Methods

        #region Freeze Time
        private void ToggleFreezeTime()
        {
            FreezeTimeManager.FreezeTime = !FreezeTimeManager.FreezeTime;
            if (Conf.LogToChat) Main.NewText("Time is now " + (FreezeTimeManager.FreezeTime ? "frozen" : "unfrozen"));
            timeOption.optionTitle.hover = FreezeTimeManager.FreezeTime ? "Click to unfreeze time" : "Click to freeze time";
        }
        #endregion

        private void ToggleAllTracking()
        {
            // Decide whether to enable or disable everything
            bool newVal = !TrackingSystem.AreAllActive;
            TrackingSystem.SetAll(newVal);

            // Update each option’s UI text
            State newState = newVal ? State.Enabled : State.Disabled;
            foreach (Option option in trackingOptions)
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
            foreach (Option option in hitboxOptions)
            {
                option.SetState(newState);
            }
            // Set itself
            toggleAllHitboxes.SetState(newState);
        }

        private Option toggleAllTracking;
        public List<Option> trackingOptions = [];
        private Option toggleAllHitboxes;
        public List<Option> hitboxOptions = [];

        private void UpdateRainSlider(float newValue)
        {
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
            Main.cloudAlpha = newValue / 2;
            // Main.rainTime = 3600; // Set a reasonable duration for rain

            // Optional: Send network message if in multiplayer
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                // You'd need to implement network sync if in multiplayer
                // NetMessage.SendData(MessageID.WorldData);
            }
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

            // this is preferable to having the code in update but idk how to make it work
            // townNpcSlider.SetValue(GetTownNpcCount());
            // townNpcSlider.UpdateText("Town NPCs: " + GetTownNpcCount());
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

            // Determine if it's day or night and adjust Main.time
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

            // Sync in multiplayer
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
            if (Main.GameUpdateCount % 30 == 0)
            {
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
                    timeOption.SetValue(GetCurrentTimeNormalized());
                }
                else
                {
                    // Reset the flag after slider use (prevents conflicts)
                    timeSliderActive = false;
                }

                // Update the UI display
                timeOption.UpdateText("Time: " + CalcIngameTime());
            }
        }
    }
    #endregion
}