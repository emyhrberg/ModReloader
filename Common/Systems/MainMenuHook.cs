using System;
using Microsoft.Xna.Framework;
using ModHelper.Common.Configs;
using ModHelper.Helpers;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModHelper.Common.Systems
{
    // Only load on the _CLIENT_, not the server
    // Otherwise it gives an error about the UI not being available or something
    [Autoload(Side = ModSide.Client)]
    public class MainMenuHook : ModSystem
    {
        private static UserInterface menuInterface;
        private static MainMenuState menuUI;
        private static bool hooksActive = false;

        public override void Load()
        {
            // Check if we should hook at all
            if (Conf.C != null && !Conf.C.CreateMainMenuButtons)
            {
                Log.Info("MainMenuHook: CreateMainMenuButtons is set to false. Not hooking into Main Menu.");
                return;
            }

            // Create our UI
            menuUI = new MainMenuState();
            menuInterface = new UserInterface();
            menuInterface.SetState(menuUI);

            // Add menu hooks initially
            AddMenuHooks();
        }

        private void AddMenuHooks()
        {
            if (!hooksActive)
            {
                On_Main.Update += MenuUpdateHook;
                On_Main.DrawMenu += MenuDrawHook;
                hooksActive = true;
                Log.Info("MainMenuHook: Added menu hooks");
            }
        }

        private void RemoveMenuHooks()
        {
            if (hooksActive)
            {
                On_Main.Update -= MenuUpdateHook;
                On_Main.DrawMenu -= MenuDrawHook;
                hooksActive = false;
                Log.Info("MainMenuHook: Removed menu hooks");
            }
        }

        private static void MenuUpdateHook(On_Main.orig_Update orig, Main self, GameTime gameTime)
        {
            orig(self, gameTime);

            // Only update menu when actually in the menu
            if (Main.gameMenu)
            {
                menuInterface?.Update(gameTime);
            }
        }

        private static void MenuDrawHook(On_Main.orig_DrawMenu orig, Main self, GameTime gameTime)
        {
            orig(self, gameTime);

            if (!Main.gameMenu) return;

            Main.spriteBatch.Begin();
            try
            {
                menuInterface?.Draw(Main.spriteBatch, gameTime);
            }
            catch (Exception ex)
            {
                Log.Error("Error drawing menu UI: " + ex.Message);
            }
            finally
            {
                Main.spriteBatch.End();
            }
        }

        public override void Unload()
        {
            RemoveMenuHooks();
            menuInterface = null;
            menuUI = null;
        }
    }
}