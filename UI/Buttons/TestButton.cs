using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.CustomReload;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI.Buttons
{
    public class TestButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText, float textSize) : BaseButton(spritesheet, buttonText, hoverText, textSize)
    {
        // Set the button icon size
        private float _scale = 1f;
        protected override float Scale => _scale;
        protected override int FrameWidth => 37;
        protected override int FrameHeight => 15;

        private List<ModMenu> _menus => (List<ModMenu>)TMLData.MenusListBackup.FieldValue;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Update Scale dynamically based on the size of the button
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            MainState mainState = sys?.mainState;
            float buttonSize = mainState?.ButtonSize ?? 70f;
            _scale = 1f + (buttonSize - 70f) * 0.005f;
        }

        private int count = 1;

        public async override void LeftClick(UIMouseEvent evt)
        {
            Main.NewText("TestButton LeftClick. Count: " + count++);

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

        public static void CustomUnload()
        {

        }
    }
}