using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using ModReloader.UI.Elements.ButtonElements;
using ModReloader.UI.Elements.DebugElements;

//using ModReloader.UI.Elements.DebugElements;
using ModReloader.UI.Elements.PanelElements;
using ModReloader.UI.Elements.PanelElements.ModElements;
using ReLogic.Content;
using Terraria.ID;
using Terraria.UI;

namespace ModReloader.Common.Systems
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
            offset = -ButtonSize * 2;

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

                string uiElementHoverDesc = LocalizationHelper.GetText("UIElementButton.HoverDescBase");
                if (Conf.C.RightClickToolOptions)
                    uiElementHoverDesc += "\n" + LocalizationHelper.GetText("UIElementButton.HoverDescRightClick");

                string logHoverDesc = LocalizationHelper.GetText("LogButton.HoverDescBase");
                if (Conf.C.RightClickToolOptions)
                    logHoverDesc += "\n" + LocalizationHelper.GetText("LogButton.HoverDescRightClick", Path.GetFileName(Logging.LogPath));

                modsButton = AddButton<ModsButton>(
                    Ass.ButtonMods,
                    LocalizationHelper.GetText("ModsButton.Text"),
                    LocalizationHelper.GetText("ModsButton.HoverText"),
                    hoverTextDescription: LocalizationHelper.GetText("ModsButton.HoverDesc"));

                uiElementButton = AddButton<UIElementButton>(
                    Ass.ButtonUIAnimation,
                    LocalizationHelper.GetText("UIElementButton.Text"),
                    LocalizationHelper.GetText("UIElementButton.HoverText"),
                    hoverTextDescription: uiElementHoverDesc);

                logButton = AddButton<LogButton>(
                    Ass.ButtonLogAnimation,
                    LocalizationHelper.GetText("LogButton.Text"),
                    LocalizationHelper.GetText("LogButton.HoverText"),
                    hoverTextDescription: logHoverDesc);

                // Panels
                modsButton.AssociatedPanel = modsPanel;
                modsPanel.AssociatedButton = modsButton;
                uiElementPanel.AssociatedButton = uiElementButton;
                uiElementButton.AssociatedPanel = uiElementPanel;
                logButton.AssociatedPanel = logPanel;
                logPanel.AssociatedButton = logButton;

                string reloadHoverMods = ReloadUtilities.IsModsToReloadEmpty
                    ? LocalizationHelper.GetText("ReloadButton.HoverDescNoMods")
                    : string.Join(",", Conf.C.ModsToReload);

                if (Conf.C.RightClickToolOptions)
                    reloadHoverMods += "\n" + LocalizationHelper.GetText("ReloadButton.HoverDescRightClick");

                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    reloadSPButton = AddButton<ReloadSPButton>(
                        Ass.ButtonReloadSPAnimation,
                        buttonText: LocalizationHelper.GetText("ReloadButton.Text"),
                        hoverText: LocalizationHelper.GetText("ReloadButton.Text"),
                        hoverTextDescription: reloadHoverMods);
                }
                else if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    reloadMPButton = AddButton<ReloadMPButton>(
                        Ass.ButtonReloadMPAnimation,
                        buttonText: LocalizationHelper.GetText("ReloadButton.Text"),
                        hoverText: LocalizationHelper.GetText("ReloadButton.Text"),
                        hoverTextDescription: reloadHoverMods);
                }
            }

            // Debug. 
            // Always add to MainState, but only show if its enabled (in Draw() and Update().. See the implementation in its class.
            DebugText debugText = new("");
            Append(debugText);
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

        private T AddButton<T>(Asset<Texture2D> spritesheet = null, string buttonText = null, string hoverText = null, string hoverTextDescription = "") where T : BaseButton
        {
            T button = (T)Activator.CreateInstance(typeof(T), spritesheet, buttonText, hoverText, hoverTextDescription);

            // offset
            button.Left.Set(offset, 0f);
            offset += ButtonSize;

            // add button
            AllButtons.Add(button);
            Append(button);

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

        public override void Update(GameTime gameTime)
        {
            if (!Active) return;
            base.Update(gameTime);

            // whoa hot reload resize buttons work!  
            UIScale = 0.85f;
            foreach (var b in AllButtons)
            {
                // Fix: Convert StyleDimension to float using its Pixels property  
                b.ButtonText.ResizeText();
            }
            LayoutButtons();
            collapse?.RecalculateSizeAndPosition();
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
