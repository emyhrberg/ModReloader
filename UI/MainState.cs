using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        public ReloadSingleplayerButton reloadSingleplayerButton;
        public ReloadMultiplayerButton reloadMultiplayerButton;

        // List of all buttons
        public HashSet<BaseButton> AllButtons;

        // MainState Constructor
        // This is where we create all the buttons and set up their positions.
        public MainState()
        {
            // Initialize config reference
            Config c = ModContent.GetInstance<Config>();

            // Create all the buttons first.
            toggleButton = CreateButton<ToggleButton>(Assets.ButtonOn, "Toggle all buttons");
            configButton = CreateButton<ConfigButton>(Assets.ButtonConfig, "Open config");
            refreshButton = CreateButton<RefreshButton>(Assets.ButtonReload, "Reload the selected mod");
            itemButton = CreateButton<ItemsButton>(Assets.ButtonItems, "Open item browser");
            npcButton = CreateButton<NPCsButton>(Assets.ButtonNPC, "Open NPC browser");
            godButton = CreateButton<GodButton>(Assets.ButtonGodOn, "Toggle player god mode");
            fastButton = CreateButton<FastButton>(Assets.ButtonFastOn, "Toggle player fast mode");
            hitboxButton = CreateButton<HitboxButton>(Assets.ButtonHitboxOn, "Show player, enemy, and projectile hitboxes");
            uiDebugButton = CreateButton<UIDebugButton>(Assets.ButtonUIDebug, "Show all UIElements from mods");
            logButton = CreateButton<LogButton>(Assets.ButtonLog, "Open client.log");
            reloadSingleplayerButton = CreateButton<ReloadSingleplayerButton>(Assets.ButtonReloadSingleplayer, "Reload singleplayer");
            reloadMultiplayerButton = CreateButton<ReloadMultiplayerButton>(Assets.ButtonReloadMultiplayer, "Reload multiplayer");

            // Append buttons conditionally based on disable flags.
            // Build list of buttons to append.
            var buttons = new List<BaseButton> { toggleButton }; // always include toggleButton

            if (!c.DisableButton.DisableConfig) buttons.Add(configButton);
            if (!c.DisableButton.DisableReload) buttons.Add(refreshButton);
            if (!c.DisableButton.DisableItemBrowser) buttons.Add(itemButton);
            if (!c.DisableButton.DisableNPCBrowser) buttons.Add(npcButton);
            if (!c.DisableButton.DisableGod) buttons.Add(godButton);
            if (!c.DisableButton.DisableFast) buttons.Add(fastButton);
            if (!c.DisableButton.DisableHitboxes) buttons.Add(hitboxButton);
            if (!c.DisableButton.DisableUIElements) buttons.Add(uiDebugButton);
            if (!c.DisableButton.DisableLog) buttons.Add(logButton);

            // Append reload button to on multiplayer/singleplayer
            buttons.Add(reloadSingleplayerButton);
            buttons.Add(reloadMultiplayerButton); //TODO maybe only in multiplayer client

            // Append all buttons and set AllButtons.
            buttons.ForEach(Append);
            AllButtons = new HashSet<BaseButton>(buttons);

            // Append all enabled buttons to the UI
            foreach (var button in AllButtons)
            {
                Append(button);
            }

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

            // setup center position
            Log.Info($"Screen width: {Main.screenWidth}, Button count: 10, Button size: {ButtonSize}");
            // 1600 - 70 * 10 = 900 (center)
            // 900 / 2 = 450
            // convert to percent 450 / 1600 = 0.28125

            // Set up positions, alignment, etc.
            button.Width.Set(ButtonSize, 0f);
            button.Height.Set(ButtonSize, 0f);
            button.VAlign = 0.9f; // 90% from top
            button.HAlign = 0.28f;


            // set min and max, width and height
            button.MinWidth = new StyleDimension(ButtonSize, 0f);
            button.MaxWidth = new StyleDimension(ButtonSize, 0f);
            button.MinHeight = new StyleDimension(ButtonSize, 0f);
            button.MaxHeight = new StyleDimension(ButtonSize, 0f);

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
