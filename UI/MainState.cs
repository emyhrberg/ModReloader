using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class MainState : UIState
    {
        // State
        public bool AreButtonsShowing = true;
        public bool IsDrawingTextOnButtons = true;
        public float ButtonScale = 1.0f;
        private Config c;

        // Buttons
        public ItemsButton itemButton;
        public RefreshButton refreshButton;
        public ConfigButton configButton;
        public ToggleButton toggleButton;
        public NPCsButton npcButton;
        public GodButton godButton;
        public TeleportButton teleportButton;
        public LogButton logButton;

        // List of all buttons
        public BaseButton[] AllButtons => [toggleButton, itemButton, refreshButton, configButton, npcButton, godButton, teleportButton, logButton];

        public override void OnInitialize()
        {
            // Init some stuff
            c = ModContent.GetInstance<Config>();

            // Create all the buttons
            toggleButton = CreateButton<ToggleButton>(Assets.ButtonOn, Assets.ButtonOnNoText, "Toggle buttons\nRight click to hide", 0f);
            configButton = CreateButton<ConfigButton>(Assets.ButtonConfig, Assets.ButtonConfigNoText, "Open config", 100f);
            itemButton = CreateButton<ItemsButton>(Assets.ButtonItems, Assets.ButtonItemsNoText, "Browse all items", 200f);
            npcButton = CreateButton<NPCsButton>(Assets.ButtonNPC, Assets.ButtonNPCNoText, "Browse all NPCs", 300f);
            godButton = CreateButton<GodButton>(Assets.ButtonGod, Assets.ButtonGodNoText, "God mode", 400f);
            teleportButton = CreateButton<TeleportButton>(Assets.ButtonTeleport, Assets.ButtonTeleportNoText, "Open map and click to teleport", 500f);
            logButton = CreateButton<LogButton>(Assets.ButtonLog, Assets.ButtonLogNoText, "Open log", 600f);
            refreshButton = CreateButton<RefreshButton>(Assets.ButtonReload, Assets.ButtonReloadNoText, "Reload mod (see Config) \nRight click to go to mods", 700f);

            // Add all the buttons to the state
            Append(toggleButton);
            Append(itemButton);
            Append(refreshButton);
            Append(configButton);
            Append(npcButton);
            Append(godButton);
            Append(teleportButton);

            // Initialize the setting of whether to show text on the buttons or not
            UpdateAllButtonsTexture();
        }

        // Utility to create & position any T : BaseButton
        private static T CreateButton<T>(Asset<Texture2D> buttonImgText, Asset<Texture2D> buttonImgNoText, string hoverText, float leftOffset)
            where T : BaseButton
        {
            // We create the button via reflection
            T button = (T)Activator.CreateInstance(typeof(T), buttonImgText, buttonImgNoText, hoverText);

            // Wire up all buttons OnLeftClick to call their own HandleClick
            // button.OnLeftClick += (evt, element) => button.HandleClick();

            // Set up positions, alignment, etc.
            button.Width.Set(100f, 0f);
            button.Height.Set(100f, 0f);
            button.HAlign = 0.3f; // start 30% from the left
            button.VAlign = 0.9f; // buttons at bottom
            button.Left.Set(leftOffset, 0f);
            button.RelativeLeftOffset = leftOffset;

            return button;
        }

        // updates image only: for example no text or text, scale, and on/off and god on/off, etc
        public void UpdateAllButtonsTexture()
        {
            foreach (BaseButton btn in AllButtons)
                btn.UpdateTexture();
        }

        // updates position only
        public void UpdateButtonsPositions(Vector2 anchorPosition)
        {
            foreach (BaseButton btn in AllButtons)
            {
                btn.Left.Set(anchorPosition.X + btn.RelativeLeftOffset * ButtonScale, 0f);
                btn.Top.Set(anchorPosition.Y, 0f);
            }
            Recalculate(); // Refresh layout after moving buttons.
        }

        public void ToggleOnOff()
        {
            AreButtonsShowing = !AreButtonsShowing;
            Log.Info($"Buttons visibility toggled to: {AreButtonsShowing}");
            UpdateAllButtonsTexture();
        }

        public void AddAllExceptToggle()
        {
            foreach (BaseButton btn in AllButtons.Where(b => b != toggleButton))
                if (!Children.Contains(btn))
                    Append(btn);
        }

        public void RemoveAll()
        {
            foreach (BaseButton btn in AllButtons)
                if (Children.Contains(btn))
                    RemoveChild(btn);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Check if we should show the buttons
            bool showOnly = c.General.OnlyShowWhenInventoryOpen;
            bool invOpen = Main.playerInventory;

            RemoveAll(); // Remove all buttons to build them again

            // Show only buttons when inventory is open
            if (showOnly)
            {
                if (invOpen)
                {
                    Append(toggleButton);
                    if (AreButtonsShowing)
                        AddAllExceptToggle();
                }
            }
            else
            {
                Append(toggleButton);
                if (AreButtonsShowing)
                    AddAllExceptToggle();
            }
        }
    }
}
