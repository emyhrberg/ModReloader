using log4net;
using SquidTestingMod.Helpers;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class ButtonState : UIState
    {
        private RefreshButton refreshButton;
        private ItemsButton itemBrowserButton;
        private ConfigButton configButton;

        public override void OnInitialize()
        {
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
            refreshButton.VAlign = 0.02f;
            refreshButton.OnLeftClick += refreshButton.HandleClick;
            refreshButton.Left.Set(100f, 0f); // lil offset

            // Initialize Config Button
            configButton = new ConfigButton(Assets.ButtonConfig, "Open Config");
            configButton.Width.Set(100f, 0f);
            configButton.Height.Set(100f, 0f);
            configButton.HAlign = 0.4f;
            configButton.VAlign = 0.02f;
            configButton.OnLeftClick += configButton.HandleClick;
            configButton.Left.Set(200f, 0f); // lil offset

            Append(refreshButton);
            Append(itemBrowserButton);
            Append(configButton);
        }
    }
}