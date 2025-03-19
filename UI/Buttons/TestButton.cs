using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria.ID;
using Terraria;
using Terraria.UI;
using System.Reflection;
using Terraria.ModLoader;
using System;
using SquidTestingMod.CustomReload;
using System.Collections.Generic;
using log4net;

namespace SquidTestingMod.UI.Buttons
{
    public class TestButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText) : BaseButton(spritesheet, buttonText, hoverText)
    {
        // Set the button icon size
        protected override int FrameWidth => 37;
        protected override int FrameHeight => 15;

        private List<ModMenu> _menus => (List<ModMenu>)TMLData.MenuBackup.FieldValue;

        public async override void LeftClick(UIMouseEvent evt)
        {
            // Perform actions on leftclick here
            /*
            if (Conf.ClearClientLogOnReload)
                Log.ClearClientLog();

            ReloadUtilities.PrepareClient(ClientMode.SinglePlayer);

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                await ReloadUtilities.ExitWorldOrServer();
            }
            else if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                await ReloadUtilities.ExitAndKillServer();
            }
            */
            /*
            TMLData.MenuBackup.Restore();
            TMLData.EventBackup.Restore();

            Main.NewText($"Menus lenght: {_menus.Count}");
            Main.NewText($"Events lenght: {((Delegate)TMLData.EventBackup.FieldValue).GetInvocationList().Length}");
            */

        }

        public override void RightClick(UIMouseEvent evt)
        {
            /*
            _menus.Add(_menus[0]);
            TMLData.EventBackup.FieldValue = Delegate.Combine((Delegate)TMLData.EventBackup.FieldValue, (Action)(() => LogManager.GetLogger("SQUID").Info("Hi!")));

            Main.NewText($"Menus lenght: {_menus.Count}");
            Main.NewText($"Events lenght: {((Delegate)TMLData.EventBackup.FieldValue).GetInvocationList().Length}");
            */
            //Delegate.Remove(TestEventNew, TestEventOld).DynamicInvoke();
        }
    }
}