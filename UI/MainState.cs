using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class MainState : UIState
    {
        // State
        public bool AreButtonsShowing = true;
        public float ButtonSize = 70f;

        // Buttons
        public ItemsButton itemButton;
        public RefreshButton refreshButton;
        public ConfigButton configButton;
        public ToggleButton toggleButton;
        public NPCsButton npcButton;
        public GodButton godButton;
        public FastButton fastButton;
        public LogButton logButton;
        public HitboxButton hitboxButton;
        public UIDebugButton uiDebugButton;

        // List of all buttons
        public BaseButton[] AllButtons = [];

        // MainState Constructor
        // This is where we create all the buttons and set up their positions.
        public MainState()
        {
            // Initialize config reference
            Config c = ModContent.GetInstance<Config>();

            // Create all the buttons first.
            toggleButton = CreateButton<ToggleButton>(Assets.ButtonOn, "Toggle all buttons");
            configButton = CreateButton<ConfigButton>(Assets.ButtonConfig, "Open config");
            refreshButton = CreateButton<RefreshButton>(Assets.ButtonReload, "Reload");
            itemButton = CreateButton<ItemsButton>(Assets.ButtonItems, "Open item browser");
            npcButton = CreateButton<NPCsButton>(Assets.ButtonNPC, "Open NPC browser");
            godButton = CreateButton<GodButton>(Assets.ButtonGodOn, "God mode");
            fastButton = CreateButton<FastButton>(Assets.ButtonFastOn, "Fast mode");
            hitboxButton = CreateButton<HitboxButton>(Assets.ButtonHitbox, "Hitboxes");
            uiDebugButton = CreateButton<UIDebugButton>(Assets.ButtonUIDebug, "UIElements");
            logButton = CreateButton<LogButton>(Assets.ButtonLog, "Open client.log");

            // Always add toggle button
            Append(toggleButton);

            // Append buttons conditionally based on disable flags.
            if (!c.DisableButton.DisableConfig) Append(configButton);
            if (!c.DisableButton.DisableReload) Append(refreshButton);
            if (!c.DisableButton.DisableItemBrowser) Append(itemButton);
            if (!c.DisableButton.DisableNPCBrowser) Append(npcButton);
            if (!c.DisableButton.DisableGod) Append(godButton);
            if (!c.DisableButton.DisableFast) Append(fastButton);
            if (!c.DisableButton.DisableHitboxes) Append(hitboxButton);
            if (!c.DisableButton.DisableUIElements) Append(uiDebugButton);
            if (!c.DisableButton.DisableLog) Append(logButton);

            // Add all buttons to the AllButtons array.
            AllButtons = [toggleButton, configButton, refreshButton, itemButton, npcButton, godButton, fastButton, hitboxButton, uiDebugButton, logButton];

            // Adjust left offsets for the appended buttons (skip the toggle button).
            int index = 0;
            foreach (BaseButton btn in AllButtons)
            {
                if (btn == toggleButton)
                    btn.RelativeLeftOffset = 0;
                else
                    btn.RelativeLeftOffset = ButtonSize * (++index);
            }

            // Initialize textures and positions.
            UpdateButtonsPositions(toggleButton.anchorPos);
        }

        // Utility to create & position any T : BaseButton
        private T CreateButton<T>(Asset<Texture2D> buttonImgText, string hoverText)
            where T : BaseButton
        {
            // We create the button via reflection
            T button = (T)Activator.CreateInstance(typeof(T), buttonImgText, hoverText);

            // Set up positions, alignment, etc.
            button.Width.Set(ButtonSize, 0f);
            button.Height.Set(ButtonSize, 0f);
            button.HAlign = 0.15f; // start 30% from the left
            button.VAlign = 0.9f; // buttons at bottom

            button.Recalculate();
            return button;
        }

        // updates position only
        public void UpdateButtonsPositions(Vector2 anchorPosition)
        {
            foreach (BaseButton btn in AllButtons)
            {
                btn.Left.Set(anchorPosition.X + btn.RelativeLeftOffset, 0f);
                btn.Top.Set(anchorPosition.Y, 0f);
                btn.Width.Set(ButtonSize, 0f);
                btn.Height.Set(ButtonSize, 0f);
                btn.MinWidth = new StyleDimension(ButtonSize, 0f);
                btn.MinHeight = new StyleDimension(ButtonSize, 0f);
                btn.MaxWidth = new StyleDimension(ButtonSize, 0f);
                btn.MaxHeight = new StyleDimension(ButtonSize, 0f);
                btn.SetVisibility(1f, 0f); // active = 1f, inactive = 0f
            }
            Recalculate(); // Refresh layout after moving buttons.
        }

        public void ToggleOnOff()
        {
            AreButtonsShowing = !AreButtonsShowing;
            toggleButton.UpdateTexture(); // on/off texture

            if (!AreButtonsShowing)
            {
                // Deactivate and hide all buttons
                foreach (BaseButton btn in AllButtons.Where(b => b != toggleButton))
                {
                    btn.Active = false;
                }
            }
            else
            {
                // Activate and show all buttons
                foreach (BaseButton btn in AllButtons.Where(b => b != toggleButton))
                {
                    btn.Active = true;
                }
            }
        }
    }
}
