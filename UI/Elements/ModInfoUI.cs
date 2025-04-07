using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ModHelper.UI.Elements
{
    /// <summary>
    /// This is an example of an in-game fullscreen UI.
    /// This UI is shown and managed using the IngameFancyUI class. Since we are using IngameFancyUI, we do not need to write code to Update or Draw a UserInterface, unlike other UI. Since IngameFancyUI is used for non-gameplay fullscreen UI, it prevents later interface layers from drawing. Vanilla examples of this sort of UI include the bestiary, emote menu, and settings menus.
    /// </summary>
    public class ModInfoUI : UIState, ILoadable
    {
        public static ModInfoUI instance;
        public string CurrentModDescription = "";
        private UIText descriptionText;

        public void Load(Mod mod)
        {
            instance = this;
        }

        public void Unload()
        {
        }

        public override void OnInitialize()
        {
            var panel = new UIPanel()
            {
                HAlign = 0.5f,
                VAlign = 0.5f,
                Width = new(600, 0f),
                Height = new(440, 0f)
            };
            Append(panel);

            descriptionText = new UIText(CurrentModDescription, 0.8f, false)
            {
                Top = new(40f, 0f),
                IsWrapped = true,
                Width = StyleDimension.Fill
            };
            panel.Append(descriptionText);

            var backButton = new UITextPanel<LocalizedText>(Language.GetText("UI.Back"), 0.7f)
            {
                // TextColor = Color.Red,
                VAlign = 1f,
                Width = new(-10f, 1 / 3f),
                Height = new(30f, 0f)
            };
            backButton.WithFadedMouseOver();
            backButton.OnLeftClick += BackButton_OnLeftClick;
            panel.Append(backButton);

            var openConfigButton = new UITextPanel<LocalizedText>(Language.GetText("tModLoader.ModsOpenConfig"), 0.7f);
            openConfigButton.CopyStyle(backButton);
            openConfigButton.HAlign = 0.5f;
            openConfigButton.WithFadedMouseOver();
            // openConfigButton.OnLeftClick += OpenConfigButton_OnLeftClick;
            panel.Append(openConfigButton);

            // TODO: Use this for a ModConfig.Save example later
            // var otherButton = new UITextPanel<string>("TBD", 0.7f);
            // otherButton.CopyStyle(backButton);
            // otherButton.HAlign = 1f;
            // otherButton.BackgroundColor = Color.Purple * 0.7f;
            /*
			otherButton.WithFadedMouseOver(Color.Purple, Color.Purple * 0.7f);
			otherButton.OnLeftClick += OtherButton_OnLeftClick;
			*/
            // panel.Append(otherButton);
        }

        public override void OnActivate()
        {
            if (descriptionText != null)
                descriptionText.SetText(CurrentModDescription);
        }

        private void BackButton_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        {
            IngameFancyUI.Close();
        }

        // private void OpenConfigButton_OnLeftClick(UIMouseEvent evt, UIElement listeningElement)
        // {
        //     // We can use ModContent.GetInstance<ModConfigClassHere>().Open() to open a specific ModConfig UI.
        //     // This example, however, scrolls to a specific item in the ModConfig and also runs code after the ModConfig UI is closed.
        //     ModContent.GetInstance<ModConfigShowcaseDataTypes>().Open(onClose: () =>
        //     {
        //         // Re-open this UI when the user exits the ModConfig menu.
        //         IngameFancyUI.OpenUIState(this);
        //     }, scrollToOption: nameof(ModConfigShowcaseDataTypes.itemDefinitionExample), centerScrolledOption: true);

        //     // If we want to scroll to the header of an option instead, prepend "Header:"
        //     // ModContent.GetInstance<ModConfigShowcaseLabels>().Open(scrollToOption: $"Header:{nameof(ModConfigShowcaseLabels.TypicalHeader)}");
        // }

        // private void UpdateItemDefinitionMessageText()
        // {
        //     itemDefinitionMessage.SetText(GetItemDefinitionMessageText());
        // }

        // private string GetItemDefinitionMessageText()
        // {
        //     ModConfigShowcaseDataTypes config = ModContent.GetInstance<ModConfigShowcaseDataTypes>();
        //     var itemDefinition = config.itemDefinitionExample;
        //     string configEntryLabel = Language.GetTextValue(config.GetLocalizationKey("itemDefinitionExample.Label"));
        //     if (itemDefinition.Type == ItemID.None || itemDefinition.IsUnloaded)
        //     {
        //         return NothingText.Format(configEntryLabel);
        //     }
        //     else
        //     {
        //         return IsSetText.Format(configEntryLabel, itemDefinition.DisplayName, itemDefinition.Type);
        //     }
        // }
    }
}