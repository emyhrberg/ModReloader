using Terraria.UI;
using Terraria.ModLoader;
using log4net;
using DPSPanel.Helpers;

namespace SquidTestingMod.UI
{
    public class ButtonState : UIState
    {
        private RefreshButton refreshButton;
        private ItemsButton itemBrowserButton;
        ILog logger;

        public override void OnInitialize()
        {
            logger = ModContent.GetInstance<SquidTestingMod>().Logger;

            // Initialize Item Browser Button
            itemBrowserButton = new ItemsButton(Assets.ButtonItems, "Open Item Browser");
            itemBrowserButton.Width.Set(100f, 0f);
            itemBrowserButton.Height.Set(100f, 0f);
            itemBrowserButton.HAlign = 0.4f;
            itemBrowserButton.VAlign = 0.02f;
            itemBrowserButton.OnLeftClick += itemBrowserButton.HandleClick;

            // Initialize Refresh Button
            refreshButton = new RefreshButton(Assets.ButtonRefresh, "Exit World And Go To Mod Sources");
            refreshButton.Width.Set(100f, 0f);
            refreshButton.Height.Set(100f, 0f);
            refreshButton.HAlign = 0.4f;
            refreshButton.Left.Set(100f, 0f);
            refreshButton.VAlign = 0.02f;
            refreshButton.OnLeftClick += refreshButton.HandleClick;

            Append(refreshButton);
            Append(itemBrowserButton);
        }


    }
}