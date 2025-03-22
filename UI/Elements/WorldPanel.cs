using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SquidTestingMod.Common.Players;
using SquidTestingMod.Common.Systems;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ID;

namespace SquidTestingMod.UI.Elements
{
    public class WorldPanel : OptionPanel
    {
        private SliderPanel timeOption;
        private bool timeSliderActive = false;
        private SliderPanel townNpcSlider;

        public WorldPanel() : base(title: "World", scrollbarEnabled: true)
        {
            AddPadding(5);
            AddHeader("World");
            timeOption = new(
                title: "Time",
                min: 0f,
                max: 1f,
                defaultValue: GetCurrentTimeNormalized(),
                onValueChanged: UpdateInGameTime,
                increment: 1800f / 86400f,
                hover: "Click and drag to change time"
            );
            uiList.Add(timeOption);
            AddPadding(3f);

            SliderPanel spawnRate = new(
                title: "Spawn Rate",
                min: 0,
                max: 30,
                defaultValue: SpawnRateMultiplier.Multiplier,
                onValueChanged: SpawnRateMultiplier.SetSpawnRateMultiplier,
                increment: 1,
                hover: "Set the spawn rate multiplier",
                onClickText: () => SpawnRateMultiplier.Multiplier = 0
            );
            uiList.Add(spawnRate);
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
                hover: "Set the number of town NPCs"
            );
            uiList.Add(townNpcSlider);
            AddPadding();

            // tracking
            AddHeader("Tracking");
            AddOption("Track Enemies", DebugEnemyTrackingSystem.ToggleEnemyTracking, "Track all enemies");
            AddOption("Track Town NPCs", DebugEnemyTrackingSystem.ToggleTownNPCTracking, "Track all town NPCs");
            AddOption("Track Critters", DebugEnemyTrackingSystem.ToggleCritterTracking, "Track all critters");
            AddPadding();

            AddHeader("Hitboxes");
            AddOption("Show Hitboxes", HitboxSystem.ToggleAllHitboxes, "Show hitboxes for player, NPCs, melee projectiles");
            AddPadding();
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

                townNpcSlider.HoverText = "Town NPCs:\n" + formattedNames;
            }
            else
            {
                townNpcSlider.HoverText = "Town NPCs:\nSet the number of town NPCs";
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
