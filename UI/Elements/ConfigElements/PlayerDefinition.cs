using Microsoft.Extensions.Primitives;
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


        public PlayerDefinition() : base()
        {

        }

        public PlayerDefinition(int type) : base(Utilities.FindPlayer(type).Path)
        {

        }

        public PlayerDefinition(string path) : base()
        {
            Name = path;
        }
        public override string ToString()
        {
            return $"{(Utilities.FindPlayer(Name) != null ? Utilities.FindPlayer(Name).Name : "null")}";
        }
    }

    public class PlayerDefinitionElement : DefinitionElement<PlayerDefinition>
    {
        protected override DefinitionOptionElement<PlayerDefinition> CreateDefinitionOptionElement() => new PlayerDefinitionOptionElement(Value);

        protected override List<DefinitionOptionElement<PlayerDefinition>> CreateDefinitionOptionElementList()
        {
            var options = new List<DefinitionOptionElement<PlayerDefinition>>();

            Main.LoadPlayers();

            for (int i = 0; i < Main.PlayerList.Count; i++)
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
            ChooserFilterMod.Parent.Remove();
            ChooserFilterMod.Parent.Deactivate();
        }
    }

    public class PlayerDefinitionOptionElement : DefinitionOptionElement<PlayerDefinition>
    {
        public PlayerDefinitionOptionElement(PlayerDefinition definition, float scale = 0.5f) : base(definition, scale)
        {
            OverflowHidden = true;
        }

        public override void SetItem(PlayerDefinition definition)
        {
            base.SetItem(definition);
            Tooltip = definition?.ToString();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetInnerDimensions();

            var rectangle = dimensions.ToRectangle();
            Vector2 position = dimensions.Position() + rectangle.Size() / 2f;
            // Draw background texture (end and begin to reset the sprite batch)

            spriteBatch.Draw(BackgroundTexture.Value, dimensions.Position(), null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(default, default, default, default, default, default, default);

            // Draw player head!
            if (Definition != null && !Unloaded)
            {
                var playerFile = Utilities.FindPlayer(Definition.Name);
                if (playerFile?.Player != null)
                {
                    playerFile.Player.PlayerFrame();
                    
                    Main.PlayerRenderer.DrawPlayerHead(Main.Camera, playerFile.Player, position, scale: 1);//scale: Scale
                }
                else
                {
                    // Fallback
                    Texture2D fallbackTexture = TextureAssets.Item[ItemID.None].Value;
                    Vector2 texSize = fallbackTexture.Size();
                    Vector2 drawPos = position + rectangle.Size() * Scale / 2f - texSize / 2f;
                    spriteBatch.Draw(fallbackTexture, drawPos, Color.Gray);
                }
            }

            if (IsMouseHovering)
                UIModConfig.Tooltip = Tooltip;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
                              DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);
        }
    }
}
