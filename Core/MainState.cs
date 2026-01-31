using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using ModReloader.Common.BuilderToggles;
using ModReloader.Core.Features.DebugTextInWorld;
using ModReloader.Core.Features.LogFeatures;
using ModReloader.Core.Features.ModToggler.UI;
using ModReloader.Core.Features.Reload;
using ModReloader.Core.Features.UIElementFeatures;
using ModReloader.UI.Shared;
using ReLogic.Content;
using Terraria.ID;
using Terraria.UI;

namespace ModReloader.Core
{
    public class MainState : UIState
    {
        // Buttons for standalone mod
        public Collapse collapse;
        public ModsButton modsButton;
        public ReloadSPButton reloadSPButton;
        public ReloadMPButton reloadMPButton;
        public UIElementButton uiElementButton;
        public LogButton logButton;

        // Panels
        public ModsPanel modsPanel;
        public UIElementPanel uiElementPanel;
        public LogPanel logPanel;

        // Settings
        public bool Active = true;
        public bool AreButtonsShowing = true;
        public float TextSize = 0.9f;
        public float offset = 0;
        public float UIScale = 1f;
        public const float BaseButtonSize = 70f; // ← the original 70px value
        public float ButtonSize => BaseButtonSize * UIScale;

        public List<BaseButton> AllButtons = [];
        public List<BasePanel> AllPanels = [];

        #region Constructor
        public MainState()
        {
            ModContent.GetInstance<MainSystem>().mainState = this;
            offset = -ButtonSize;

            // Force to on
            MainStateBuilderToggle toggle = ModContent.GetInstance<MainStateBuilderToggle>();
            toggle.CurrentState = 0; // Force to "On" (state 0)
            Log.Info("MainStateBuilderToggle.CurrentState: " + toggle);
            Active = true;  // Active when toggle is "On" (state 0)

            // Create new panels no matter what
            modsPanel = AddPanel<ModsPanel>();
            uiElementPanel = AddPanel<UIElementPanel>();
            logPanel = AddPanel<LogPanel>();

            AreButtonsShowing = false; // Default to false, so we can toggle it on or off

            // For standalone mod, add the collapse and its own hotbar with buttons to toggle the panels with everything
            if (!AnyCheatModEnabled())
            {
                collapse = new(Ass.CollapseDown, Ass.CollapseUp);
                Append(collapse);

                AreButtonsShowing = true; // Show our own collapse and show buttons by default

                string uiElementHoverDesc = Loc.Get("UIElementButton.HoverDescBase");
                uiElementHoverDesc += "\n" + Loc.Get("UIElementButton.HoverDescRightClick");

                string logHoverDesc = Loc.Get("LogButton.HoverDescBase");
                logHoverDesc += "\n" + Loc.Get("LogButton.HoverDescRightClick", Path.GetFileName(Logging.LogPath));

                modsButton = AddButton<ModsButton>(
                    Ass.ButtonMods,
                    Loc.Get("ModsButton.Text"),
                    Loc.Get("ModsButton.HoverText"),
                    hoverTextDescription: Loc.Get("ModsButton.HoverDesc"));

                uiElementButton = AddButton<UIElementButton>(
                    Ass.ButtonUIAnimation,
                    Loc.Get("UIElementButton.Text"),
                    Loc.Get("UIElementButton.HoverText"),
                    hoverTextDescription: uiElementHoverDesc);

                logButton = AddButton<LogButton>(
                    Ass.ButtonLogAnimation,
                    Loc.Get("LogButton.Text"),
                    Loc.Get("LogButton.HoverText"),
                    hoverTextDescription: logHoverDesc);

                // Panels
                modsButton.AssociatedPanel = modsPanel;
                modsPanel.AssociatedButton = modsButton;
                uiElementPanel.AssociatedButton = uiElementButton;
                uiElementButton.AssociatedPanel = uiElementPanel;
                logButton.AssociatedPanel = logPanel;
                logPanel.AssociatedButton = logButton;

                string reloadHoverMods = ReloadUtilities.IsModsToReloadEmpty
                    ? Loc.Get("ReloadButton.HoverDescNoMods")
                    : string.Join(",", Conf.C.ModsToReload);

                reloadHoverMods += "\n" + Loc.Get("ReloadButton.HoverDescRightClick");

                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    reloadSPButton = AddButton<ReloadSPButton>(
                        Ass.ButtonReloadSPAnimation,
                        buttonText: Loc.Get("ReloadButton.Text"),
                        hoverText: Loc.Get("ReloadButton.Text"),
                        hoverTextDescription: reloadHoverMods);
                }
                else if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    reloadMPButton = AddButton<ReloadMPButton>(
                        Ass.ButtonReloadMPAnimation,
                        buttonText: Loc.Get("ReloadButton.Text"),
                        hoverText: Loc.Get("ReloadButton.Text"),
                        hoverTextDescription: reloadHoverMods);
                }
            }

            // Debug. 
            // Always add to MainState, but only show if its enabled (in Draw() and Update().. See the implementation in its class.
            DebugText debugText = new("");
            Append(debugText);

            // initialize
            InitializeButtonAndCollapseUIScale();
        }
        #endregion

        private bool AnyCheatModEnabled() => ModLoader.TryGetMod("CheatSheet", out _) || ModLoader.TryGetMod("HEROsMod", out _) || ModLoader.TryGetMod("DragonLens", out _);

        private T AddPanel<T>() where T : BasePanel, new()
        {
            T panel = new();
            AllPanels.Add(panel);
            Append(panel);
            return panel;
        }

        public T AddButton<T>(Asset<Texture2D> spritesheet = null, string buttonText = null, string hoverText = null, string hoverTextDescription = "") where T : BaseButton
        {
            T button = (T)Activator.CreateInstance(typeof(T), spritesheet, buttonText, hoverText, hoverTextDescription);

            // add button
            AllButtons.Add(button);
            Append(button);

            LayoutButtons();

            return button;
        }

        private void LayoutButtons()
        {
            float bs = ButtonSize;                  // BaseButtonSize * UIScale
            float totalW = AllButtons.Count * bs;
            float startX = (Main.screenWidth - totalW) / 2f;

            for (int i = 0; i < AllButtons.Count; i++)
            {
                var b = AllButtons[i];

                // 1) resize the hit‐box
                b.Width.Set(bs, 0f);
                b.Height.Set(bs, 0f);

                // 2) absolute bottom‐snap via Top.Percent + Top.Pixels
                b.HAlign = 0f;                     // use absolute Left
                b.VAlign = 0f;                     // ignore VAlign
                b.Left.Set(startX + i * bs, 0f);
                b.Top.Set(-bs, 1f);              // Top = 1*screenHeight - bs

                // 3) finally re‐calculate its position
                b.Recalculate();
            }
        }

        private void InitializeButtonAndCollapseUIScale()
        {
            UIScale = 0.85f;
            foreach (var b in AllButtons)
            {
                // Fix: Convert StyleDimension to float using its Pixels property  
                b.ButtonText.ResizeText();
            }
            LayoutButtons();
            collapse?.RecalculateSizeAndPosition();
        }

        private int lastScreenWidth = Main.screenWidth;

        public override void Update(GameTime gameTime)
        {
            if (!Active) return;
            base.Update(gameTime);

            // Re-layout buttons if screen size changed
            if (Main.screenWidth != lastScreenWidth)
            {
                LayoutButtons();
                lastScreenWidth = Main.screenWidth;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!Active) return;
            base.Draw(spriteBatch);

            if (AreButtonsShowing)
            {
                foreach (var button in AllButtons)
                {
                    // hot reload testing. comment the below line out plz
                    //if (button is UIElementButton) button.DrawTooltipPanel(button.HoverText, button.HoverTextDescription);

                    if (button.IsMouseHovering && button.HoverText != null)
                    {
                        button.DrawTooltipPanel(button.HoverText, button.HoverTextDescription);
                    }
                }
            }
        }
    }
}
