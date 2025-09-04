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
        public PlayerDefinitionOptionElement(PlayerDefinition definition, float scale = 0.5f) : base(definition, scale)
        {
        }

        public override void SetItem(PlayerDefinition definition)
        {
            base.SetItem(definition);
            Tooltip = definition?.ToString();
        }

        private bool _animated;
        private int _animationCounter;

        public override void Update(GameTime gameTime)
        {
            OverflowHidden = true;
            base.Update(gameTime);

            if (Definition?.Name is string name && Utilities.FindPlayer(name)?.Player is Player player)
            {
                using (new Main.CurrentPlayerOverride(player))
                {
                    _animated = IsMouseHovering;

                    if (_animated)
                    {
                        _animationCounter++;
                        int frame = (int)(Main.GlobalTimeWrappedHourly / 0.07f) % 14 + 6;
                        int y = frame * 56;
                        player.bodyFrame.Y = player.legFrame.Y = player.headFrame.Y = y;
                        player.WingFrame(wingFlap: false);
                    }
                    else
                    {
                        player.bodyFrame.Y = player.legFrame.Y = player.headFrame.Y = 0;
                    }

                    player.PlayerFrame();
                }
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            // Positions
            CalculatedStyle dimensions = GetInnerDimensions();
            Vector2 position = dimensions.Position();
            Vector2 size = BackgroundTexture.Size() * Scale;
            Rectangle destination = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);

            // Draw background
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp,
                              DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);
            spriteBatch.Draw(BackgroundTexture.Value, destination, Color.White);
            spriteBatch.End();

            // Restart spritebatch
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
                              DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);

            // Draw player
            if (Definition?.Name is string name && Utilities.FindPlayer(name)?.Player is Player player)
            {
                player.direction = 1; // facing right
                //player.mount.SetMount(0, player);
                //player.heldProj = -1;

                using (new Main.CurrentPlayerOverride(player))
                {
                    Vector2 drawPos = position + size / 2f + Main.screenPosition;
                    Main.PlayerRenderer.DrawPlayer(Main.Camera, player, drawPos, 0f, Vector2.Zero);
                }
            }

            // Restart spritebatch
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
                              DepthStencilState.Default, RasterizerState.CullNone, null, Main.UIScaleMatrix);

            if (IsMouseHovering)
                UIModConfig.Tooltip = Tooltip;
        }
    }
}
