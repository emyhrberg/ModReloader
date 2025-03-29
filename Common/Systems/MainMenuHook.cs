using System;
using Microsoft.Xna.Framework;
using ModHelper.Common.Configs;
using ModHelper.Helpers;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace ModHelper.Common.Systems
{
    public class MainMenuHook : ModSystem
    {
        private static UserInterface menuInterface;
        private static MainMenuState menuUI;

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

            // Hook Update and Draw
            On_Main.Update += MenuUpdateHook;
            On_Main.DrawMenu += MenuDrawHook;
        }

        public override void Unload()
        {
            On_Main.Update -= MenuUpdateHook;
            On_Main.DrawMenu -= MenuDrawHook;
            menuInterface = null;
            menuUI = null;
        }

        private static void MenuUpdateHook(On_Main.orig_Update orig, Main self, GameTime gameTime)
        {
            orig(self, gameTime);
            if (Main.gameMenu)
            {
                menuInterface?.Update(gameTime);
            }
        }

        private static void MenuDrawHook(On_Main.orig_DrawMenu orig, Main self, GameTime gameTime)
        {
            orig(self, gameTime);

            Main.spriteBatch.Begin();
            try
            {
                menuInterface.Draw(Main.spriteBatch, gameTime);
            }
            catch (Exception ex)
            {
                // Log the error but don't crash the game
                Log.Error("Error drawing menu UI: " + ex.Message);
            }
            finally
            {
                // Always end the sprite batch
                Main.spriteBatch.End();
            }
        }
    }
}