using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI.Buttons
{
    public class ReloadSPButton(Asset<Texture2D> spritesheet, string buttonText, string hoverText) : ReloadButton(spritesheet, buttonText, hoverText)
    {
        // Set custom animation dimensions
        protected override int MaxFrames => 5;
        protected override int FrameSpeed => 12;
        protected override int FrameWidth => 65;
        protected override int FrameHeight => 65;

        public async override void LeftClick(UIMouseEvent evt)
        {
            // 1 Clear logs if needed
            if (Conf.ClearClientLogOnReload)
                Log.ClearClientLog();

            // 2 Prepare client data
            ReloadUtilities.PrepareClient(ClientMode.SinglePlayer);

            // 3 Exit server or world
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                await ReloadUtilities.ExitWorldOrServer();
            }
            else if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                await ReloadUtilities.ExitAndKillServer();
            }

            // 3 Reload
            ReloadUtilities.BuildAndReloadMod();
        }

        public override void RightClick(UIMouseEvent evt)
        {
            // Retrieve the MainSystem instance and ensure it and its mainState are not null.
            var sys = ModContent.GetInstance<MainSystem>();
            if (sys?.mainState == null)
                return;

            // Retrieve the panels and check for null.
            var allPanels = sys.mainState.AllPanels;
            var modsPanel = sys.mainState.modsPanel;
            if (allPanels == null || modsPanel == null)
            {
                Log.Error("ReloadSPButton.RightClick: allPanels or modsPanel is null.");
                return;
            }

            // Close all panels except the modsPanel.
            foreach (var panel in allPanels.Except([modsPanel]))
            {
                if (panel != null && panel.GetActive())
                {
                    panel.SetActive(false);
                }
            }

            // Toggle the modsPanel's active state.
            if (modsPanel.GetActive())
                modsPanel.SetActive(false);
            else
                modsPanel.SetActive(true);
        }
    }
}