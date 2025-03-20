using System;
using System.Linq;
using Microsoft.Xna.Framework;
using SquidTestingMod.Common.Systems;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace SquidTestingMod.UI.Elements
{
    /// <summary>
    /// A panel containing options to modify player behaviour like God,Fast,Build,etc.
    /// </summary>
    public class DebugPanel : OptionPanel
    {
        private SliderOption timeOption;
        private bool timeSliderActive = false;
        private SliderOption townNpcSlider;

        public DebugPanel() : base(title: "Debug", scrollbarEnabled: true)
        {
            // Add debug options
            AddHeader("Hitboxes");
            OnOffOption hitboxes = new(HitboxSystem.ToggleAllHitboxes, "Hitboxes Off", "Show player, NPC, melee and projectile hitboxes");
            uiList.Add(hitboxes);
            AddPadding();

            AddHeader("UI");
            DebugSystem debugSystem = ModContent.GetInstance<DebugSystem>();
            OnOffOption uiDebug = new(debugSystem.ToggleUIDebugDrawing, "UIElements Hitboxes Off", "Show all UI elements from mods");
            OnOffOption uiDebugSize = new(debugSystem.ToggleUIDebugSizeElementDrawing, "UIElements Size Text Off", "Show sizes of UI elements");
            // OnOffOption printUI = new(debugSystem.PrintAllUIElements, "Print UIElements", "Prints all UI elements and dimensions to chat");
            uiList.Add(uiDebug);
            uiList.Add(uiDebugSize);
            // uiList.Add(printUI);
            AddPadding();

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

            SliderOption spawnRate = new(
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

            // Town NPCs
            // Force at least 1 for the max, just so the slider has a range
            int numberOfTownNPCs = GetTownNpcCount();
            float maxValue = Math.Max(1f, numberOfTownNPCs);
            townNpcSlider = new(
                title: "Town NPCs",
                defaultValue: maxValue,
                min: 0f,
                max: maxValue,
                onValueChanged: UpdateTownNpcSlider,
                increment: 1f,
                hover: "Set the number of town NPCs (scuffed)"
            );
            uiList.Add(townNpcSlider);

            // tracking
            OnOffOption enemyTrack = new(DebugEnemyTrackingSystem.ToggleEnemyTracking, "Track Enemies Off", "Show all enemies position with an arrow");
            OnOffOption townTrack = new(DebugEnemyTrackingSystem.ToggleTownNPCTracking, "Track Town NPCs Off", "Show all town NPCs position with an arrow");
            OnOffOption critterTrack = new(DebugEnemyTrackingSystem.ToggleCritterTracking, "Track Critters Off", "Show all critters position with an arrow");
            uiList.Add(enemyTrack);
            uiList.Add(townTrack);
            uiList.Add(critterTrack);
            AddPadding();

            AddHeader("Logs");
            OnOffOption openClient = new(Log.OpenClientLog, "Open client.log", "Left click to open client.log\nRight click to open folder location", Log.OpenLogFolder);
            OnOffOption openEnabled = new(Log.OpenEnabledJson, "Open enabled.json", "Left click to open enabled.json\nRight click to open folder location", Log.OpenEnabledJsonFolder);
            OnOffOption clearClient = new(Log.ClearClientLog, "Clear client.log", "Clear the client.log file");
            uiList.Add(openClient);
            uiList.Add(openEnabled);
            uiList.Add(clearClient);
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

        private int GetTownNpcCount() => Main.npc.Where(npc => npc.active && npc.townNPC).Count();

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