using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.Default;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ModReloader.UI.Elements.ConfigElements
{
    public class PlayerDefinition : EntityDefinition
    {
        public override int Type => Utilities.FindPlayerId(Name);

        public override bool IsUnloaded
        => Type <= -1 || Name == null;

        [JsonIgnore]
        private PlayerFileData PlayerFile;

        public PlayerDefinition() : base() { }
        public PlayerDefinition(int type) : base(Utilities.FindPlayer(type).Path)
        {
            UpdatePlayerFile();
        }

        private void UpdatePlayerFile()
        {
            if (PlayerFile == null || PlayerFile.Path != Name)
            {
                PlayerFile = Utilities.FindPlayer(Name);
            }
        }
        public PlayerDefinition(string path)
        {
            Mod = "Terraria";
            Name = path;
            UpdatePlayerFile();
        }
        public override string ToString()
        {
            UpdatePlayerFile();
            return $"{(PlayerFile != null ? PlayerFile.Name : "null")}";
        }
    }

    public class PlayerDefinitionElement : DefinitionElement<PlayerDefinition>
    {
        protected override DefinitionOptionElement<PlayerDefinition> CreateDefinitionOptionElement() => new PlayerDefinitionOptionElement(Value);

        protected override List<DefinitionOptionElement<PlayerDefinition>> CreateDefinitionOptionElementList()
        {
            var options = new List<DefinitionOptionElement<PlayerDefinition>>();

            Main.LoadPlayers();

            for (int i = -1; i < Main.PlayerList.Count; i++)
            {
                Log.Info("Creating PlayerDefinition for index: " + i);
                // The first Player from PlayerID is null, so it's better to create an empty PlayerDefinition.
                var PlayerDefinition = i == -1 ? new PlayerDefinition() : new PlayerDefinition(i);
                var optionElement = new PlayerDefinitionOptionElement(PlayerDefinition, OptionScale);
                optionElement.OnLeftClick += (a, b) =>
                {
                    Value = optionElement.Definition;
                    UpdateNeeded = true;
                    SelectionExpanded = false;
                };
                options.Add(optionElement);
            }

            return options;
        }

        protected override List<DefinitionOptionElement<PlayerDefinition>> GetPassedOptionElements()
        {
            var passed = new List<DefinitionOptionElement<PlayerDefinition>>();

            foreach (var option in Options)
            {
                // Should this be the localized Player name?
                if (!Utilities.FindPlayer(option.Type).Name.Contains(ChooserFilter.CurrentString, StringComparison.OrdinalIgnoreCase))
                    continue;

                string modname = "Terraria";

                if (modname.IndexOf(ChooserFilterMod.CurrentString, StringComparison.OrdinalIgnoreCase) == -1)
                    continue;

                passed.Add(option);
            }
            return passed;
        }

        public override void OnBind()
        {
            base.OnBind();
            RemoveChild(ChooserFilterMod.Parent);
        }
    }

    public class PlayerDefinitionOptionElement : DefinitionOptionElement<PlayerDefinition>
    {
        public PlayerDefinitionOptionElement(PlayerDefinition definition, float scale = 0.75f) : base(definition, scale)
        {
        }

        public override void SetItem(PlayerDefinition definition)
        {
            base.SetItem(definition);
            Tooltip = definition.ToString();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetInnerDimensions();

            spriteBatch.Draw(BackgroundTexture.Value, dimensions.Position(), null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);

            if (Definition != null)
            {
                int type = Unloaded ? -1 : Type;

                Texture2D PlayerTexture;

                if (type == -1)
                {
                    // Use ItemID.None as the empty Player texture.
                    PlayerTexture = TextureAssets.Item[ItemID.None].Value;
                }
                else
                {
                    PlayerTexture = TextureAssets.Item[type + 1].Value;
                }


                int height = PlayerTexture.Height;
                int width = PlayerTexture.Width;
                int y = height;
                var rectangle2 = new Rectangle(0, 0, width, height);

                float drawScale = 1f;
                float availableWidth = (float)DefaultBackgroundTexture.Width() * Scale;

                if (width > availableWidth || height > availableWidth)
                {
                    if (width > height)
                    {
                        drawScale = availableWidth / width;
                    }
                    else
                    {
                        drawScale = availableWidth / height;
                    }
                }

                Vector2 vector = BackgroundTexture.Size() * Scale;
                Vector2 position2 = dimensions.Position() + vector / 2f - rectangle2.Size() * drawScale / 2f;
                Vector2 origin = rectangle2.Size() * 0/* * (pulseScale / 2f - 0.5f)*/;

                spriteBatch.Draw(PlayerTexture, position2, rectangle2, Color.White, 0f, origin, drawScale, SpriteEffects.None, 0f);
            }

            if (IsMouseHovering)
                UIModConfig.Tooltip = Tooltip;
        }
    }
}
