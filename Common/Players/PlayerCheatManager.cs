using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ModHelper.Common.Configs;
using ModHelper.Common.Systems;
using ModHelper.Helpers;
using ModHelper.PacketHandlers;
using ModHelper.UI;
using ModHelper.UI.Elements;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ModHelper.Common.Players
{
    public class PlayerCheatManager : ModPlayer
    {
        // Defines a record (a record is like a class but immutable) for cheats
        // It has a name, description, a function to get the value, and a function to set the value
        // Toggle is a method to toggle the value of the cheat
        // ToggleGod is a method to toggle the god mode cheat (special case because we send packets to the server to sync the state in multiplayer)
        public record Cheat(string Name, string Description, Func<bool> GetValue, Action<bool> SetValue)
        {
            public void Toggle() => SetValue(!GetValue());

            public void ToggleGod()
            {
                // Toggle god mode
                SetValue(!GetValue());

                // Send packet to server to sync the state in multiplayer
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    ModNetHandler.GodModePacketHandler.SendGodMode(
                        toWho: -1, // -1 means all players
                        fromWho: Main.LocalPlayer.whoAmI,
                        godMode: Main.LocalPlayer.GetModPlayer<PlayerCheatManager>().GetGod()
                    );
                }
            }
        }

        // Booleans for each cheat
        private bool God = false;
        private bool Noclip = false;
        private bool LightMode = false;
        private bool KillAura = false;
        private bool MineAura = false;
        private bool BuildAnywhere = false;
        private bool BuildFaster = false;
        private bool TeleportWithRightClick = false;

        public bool GetGod() => God;
        public bool GetNoclip() => Noclip;
        public bool GetLightMode() => LightMode;
        public bool GetKillAura() => KillAura;
        public bool GetMineAura() => MineAura;
        public bool GetBuildAnywhere() => BuildAnywhere;
        public bool GetBuildFaster() => BuildFaster;
        public bool GetTeleportWithRightClick() => TeleportWithRightClick;

        // Master list of cheats
        private List<Cheat> Cheats = [];
        public List<Cheat> GetCheats() => Cheats =
            [
                new Cheat("God", "Makes you immortal", () => God, v => God = v),
                new Cheat("Light", "Light up your surroundings", () => LightMode, v => LightMode = v),
                new Cheat("Build Anywhere", "Place blocks in mid-air", () => BuildAnywhere, v => BuildAnywhere = v),
                new Cheat("Build Faster", "Place and mine faster", () => BuildFaster, v => BuildFaster = v),
                new Cheat("Noclip", "Fly through blocks (use shift+space to go faster)", () => Noclip, v => Noclip = v),
                new Cheat("Teleport With Right Click", "Right click to teleport to your mouse position", () => TeleportWithRightClick, v => TeleportWithRightClick = v),
                new Cheat("Kill Aura", "Insta-kill touching enemies", () => KillAura, v => KillAura = v),
                new Cheat("Mine Aura", "Mine tiles around you", () => MineAura, v => MineAura = v),
            ];

        // Super mode
        private bool SuperMode => Conf.C.EnterWorldSuperMode;
        public bool GetSuperMode() => SuperMode;
        public bool SetSuperMode(bool value) => Conf.C.EnterWorldSuperMode = value;
        public void ToggleSuperMode()
        {
            // Toggle super mode
            SetSuperMode(!GetSuperMode());
            // Log.Info("Super mode toggled: " + SuperMode);
            Conf.C.EnterWorldSuperMode = !Conf.C.EnterWorldSuperMode;
            Conf.ForceSaveConfig(Conf.C);

            if (SuperMode)
            {
                DisableSupermode();
            }
            else
            {
                EnableSupermode();
            }
        }

        // Called by “Toggle All”
        public void SetAllCheats(bool value)
        {
            foreach (var cheat in Cheats)
                cheat.SetValue(value);
        }

        public override void OnEnterWorld()
        {
            base.OnEnterWorld();

            // Toggle super mode
            if (Conf.C.EnterWorldSuperMode)
            {
                EnableSupermode(print: false); // Don't print when entering world
            }
        }

        public void EnableSupermode(bool print = true)
        {
            // Force config option to true.
            Conf.C.EnterWorldSuperMode = true;
            Conf.ForceSaveConfig(Conf.C);

            SetAllCheats(true);
            Noclip = false;
            MineAura = false;
            KillAura = false;
            TeleportWithRightClick = false;
            SpawnRateMultiplier.Multiplier = 0f;

            // Update the spawn rate slider to 0
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            WorldPanel w = sys.mainState.worldPanel;
            w.spawnRateSlider.SetValue(0f);

            // Update the enabled texts all enabled except mine aura and noclip
            PlayerPanel p = sys.mainState.playerPanel;
            foreach (OptionElement o in p.cheatOptions)
            {
                if (o.text == "Mine Aura" || o.text == "Noclip" || o.text == "Kill Aura" || o.text == "Teleport With Right Click")
                {
                    o.SetState(OptionElement.State.Disabled);
                }
                else
                {
                    o.SetState(OptionElement.State.Enabled);
                }
            }

            // WorldPanel w = sys.mainState.worldPanel;
            // w.spawnRateSlider.SetValue(0f);

            // Butcher all NPCs
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && !npc.friendly && !npc.townNPC && !npc.dontTakeDamage)
                {
                    npc.StrikeInstantKill();
                }
            }
            // Dont need to print in every enter world, its annoying.
            if (print)
            {
                ChatHelper.NewText("Super Mode enabled and all hostile NPCs butchered!", Color.Green);
            }

            Conf.C.EnterWorldSuperMode = true;
            Conf.ForceSaveConfig(Conf.C);
        }

        public void DisableSupermode()
        {
            // Force config option to false.
            // To set it for future reloads, for the users convenience.
            Conf.C.EnterWorldSuperMode = false;
            Conf.ForceSaveConfig(Conf.C);

            SetAllCheats(false);
            SpawnRateMultiplier.Multiplier = 1f;
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            // WorldPanel w = sys.mainState.worldPanel;
            // w.spawnRateSlider.SetValue(0f);

            // Update the spawn rate slider to 0
            WorldPanel w = sys.mainState.worldPanel;
            w.spawnRateSlider.SetValue(1f);


            // Update the enabled texts all Disabled
            PlayerPanel p = sys.mainState.playerPanel;
            foreach (OptionElement o in p.cheatOptions)
            {
                o.SetState(OptionElement.State.Disabled);
            }

            // Disable the config option
            Conf.C.EnterWorldSuperMode = false;
            Conf.ForceSaveConfig(Conf.C);
        }
    }
}
