using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SquidTestingMod.Common.Configs;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace SquidTestingMod.Common.Systems
{
    public class PlayerGlowEffect : ModPlayer
    {
        // The thing to draw to. Since it extends Texture2D, you
        // would need one of these for each player drawn on
        // screen
        public static RenderTarget2D playerDrawingTarget = null;
        // How big the draw area for the player should be.
        // This should be as small as possible while still being
        // big enough to fit the player. 16 is the size of a tile
        public static readonly Vector2 renderSize = new Vector2(16 * 5, 16 * 5);
        // prevents the glow from drawing before its actually made
        public static bool ready = false;
        // the draw data to use in the player draw layer
        public static DrawData sData;

        public override void Load()
        {
            //playerDrawingTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, (int)renderSize.X, (int)renderSize.Y);
        }

        public override void Unload()
        {
            //if(playerDrawingTarget != null && !playerDrawingTarget.IsDisposed)
            //	playerDrawingTarget.Dispose();
            playerDrawingTarget = null;
        }

        public override void PostUpdate()
        {
            // reset just to be sure
            ready = false;

            // create it if it doesnt already exist.
            // You get an exception if you try to do this in
            // Load() for some reason
            if (!Main.dedServ && playerDrawingTarget == null)
                playerDrawingTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, (int)PlayerGlowEffect.renderSize.X, (int)PlayerGlowEffect.renderSize.Y);

            // since just testing, its only drawn for the local player
            if (Player.whoAmI != Main.myPlayer || playerDrawingTarget == null)
                return;

            // set up the player draw layer stuff and
            // then go collect/run all of the layers
            PlayerDrawSet drawInfo = new PlayerDrawSet();
            List<DrawData> data = new List<DrawData>(16);
            List<int> useless = new List<int>(0);
            drawInfo.BoringSetup(Main.LocalPlayer, data, useless, useless, Player.position, 0, 0, Vector2.Zero);
            DrawPlayerOnlyDrawLayers(ref drawInfo);

            // define the stuff neeeded for drawing
            GraphicsDevice device = Main.instance.GraphicsDevice;
            SpriteBatch spritebatch = new SpriteBatch(device);

            // make it draw to the RenderTarget2D and clear it
            device.SetRenderTarget(playerDrawingTarget);
            device.Clear(Color.Transparent);

            // draw all of the draw datas
            spritebatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
            for (int i = 0; i < data.Count; i++)
            {
                DrawData d = data[i];
                d.position += renderSize / 2 - Player.Size / 2 - (Player.position - Main.screenPosition);
                d.Draw(spritebatch);
            }
            spritebatch.End();

            // reset the graphics thing back to the default and 
            // get rid of the sprite batch
            device.SetRenderTarget(null);
            spritebatch.Dispose();

            // calculate the draw data
            DrawData glowDraw = new DrawData(
                playerDrawingTarget,
                Vector2.Zero, // place it nowhere, for now
                null, // use the entire image
                Color.LightPink, // color it light purple/pink
                0, // rotation
                (renderSize / 2).Floor(), // use the center as the origin
                1f, // scale
                0 // flip (0 means none)
            );

            // set the shader of the draw data to the glow effect thing.
            // This can be changed to any other shader
            int shaderId = GameShaders.Armor.GetShaderIdFromItemId(ModContent.ItemType<BorderShaderDye>());
            if (shaderId != -1)
            {
                glowDraw.shader = shaderId;
            }

            // set it to the thing to be used in
            // the draw layer
            sData = glowDraw;
            ready = true;
        }

        /// <summary>
        /// Does a bunch of the player draw layer things. I
        /// dont think it does mod layers. Some of them are
        /// commented out since they are either effects
        /// or things that are not actually part of the player,
        /// like mounts. You can just uncomment them if you think
        /// they are needed.
        /// DrawPlayer_extra_TorsoMinus()
        /// and 
        /// DrawPlayer_extra_TorsoPlus()
        /// cannot be removed since they change the positioning
        /// of things that are drawn instead of drawing anything
        /// themselves
        /// </summary>
        public static void DrawPlayerOnlyDrawLayers(ref PlayerDrawSet drawInfo)
        {
            PlayerDrawLayers.DrawPlayer_extra_TorsoPlus(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_01_2_JimsCloak(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_extra_TorsoMinus(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_02_MountBehindPlayer(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_03_Carpet(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_03_PortableStool(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_extra_TorsoPlus(ref drawInfo);
            //PlayerDrawLayers.DrawPlayer_04_ElectrifiedDebuffBack(ref drawInfo);
            //PlayerDrawLayers.DrawPlayer_05_ForbiddenSetRing(ref drawInfo);
            //PlayerDrawLayers.DrawPlayer_05_2_SafemanSun(ref drawInfo);
            //PlayerDrawLayers.DrawPlayer_06_WebbedDebuffBack(ref drawInfo);
            //PlayerDrawLayers.DrawPlayer_07_LeinforsHairShampoo(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_extra_TorsoMinus(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_08_Backpacks(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_extra_TorsoPlus(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_08_1_Tails(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_extra_TorsoMinus(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_09_Wings(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_extra_TorsoPlus(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_01_BackHair(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_10_BackAcc(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_01_3_BackHead(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_extra_TorsoMinus(ref drawInfo);
            //PlayerDrawLayers.DrawPlayer_11_Balloons(ref drawInfo);
            //if (drawInfo.weaponDrawOrder == WeaponDrawOrder.BehindBackArm)
            //{
            //	PlayerDrawLayers.DrawPlayer_27_HeldItem(ref drawInfo);
            //}
            PlayerDrawLayers.DrawPlayer_12_Skin(ref drawInfo);
            if (drawInfo.drawPlayer.wearsRobe && drawInfo.drawPlayer.body != 166)
            {
                PlayerDrawLayers.DrawPlayer_14_Shoes(ref drawInfo);
                PlayerDrawLayers.DrawPlayer_13_Leggings(ref drawInfo);
            }
            else
            {
                PlayerDrawLayers.DrawPlayer_13_Leggings(ref drawInfo);
                PlayerDrawLayers.DrawPlayer_14_Shoes(ref drawInfo);
            }
            PlayerDrawLayers.DrawPlayer_extra_TorsoPlus(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_15_SkinLongCoat(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_16_ArmorLongCoat(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_17_Torso(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_18_OffhandAcc(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_19_WaistAcc(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_20_NeckAcc(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_21_Head(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_21_1_Magiluminescence(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_22_FaceAcc(ref drawInfo);
            if (drawInfo.drawFrontAccInNeckAccLayer)
            {
                PlayerDrawLayers.DrawPlayer_extra_TorsoMinus(ref drawInfo);
                PlayerDrawLayers.DrawPlayer_32_FrontAcc_FrontPart(ref drawInfo);
                PlayerDrawLayers.DrawPlayer_extra_TorsoPlus(ref drawInfo);
            }
            //PlayerDrawLayers.DrawPlayer_23_MountFront(ref drawInfo);
            //PlayerDrawLayers.DrawPlayer_24_Pulley(ref drawInfo);
            //PlayerDrawLayers.DrawPlayer_JimsDroneRadio(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_32_FrontAcc_BackPart(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_25_Shield(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_extra_MountPlus(ref drawInfo);
            //PlayerDrawLayers.DrawPlayer_26_SolarShield(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_extra_MountMinus(ref drawInfo);
            //if (drawInfo.weaponDrawOrder == WeaponDrawOrder.BehindFrontArm)
            //{
            //	PlayerDrawLayers.DrawPlayer_27_HeldItem(ref drawInfo);
            //}
            PlayerDrawLayers.DrawPlayer_28_ArmOverItem(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_29_OnhandAcc(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_30_BladedGlove(ref drawInfo);
            if (!drawInfo.drawFrontAccInNeckAccLayer)
            {
                PlayerDrawLayers.DrawPlayer_32_FrontAcc_FrontPart(ref drawInfo);
            }
            PlayerDrawLayers.DrawPlayer_extra_TorsoMinus(ref drawInfo);
            //if (drawInfo.weaponDrawOrder == WeaponDrawOrder.OverFrontArm)
            //{
            //	PlayerDrawLayers.DrawPlayer_27_HeldItem(ref drawInfo);
            //}
            //PlayerDrawLayers.DrawPlayer_31_ProjectileOverArm(ref drawInfo);
            //PlayerDrawLayers.DrawPlayer_33_FrozenOrWebbedDebuff(ref drawInfo);
            //PlayerDrawLayers.DrawPlayer_34_ElectrifiedDebuffFront(ref drawInfo);
            //PlayerDrawLayers.DrawPlayer_35_IceBarrier(ref drawInfo);
            //PlayerDrawLayers.DrawPlayer_36_CTG(ref drawInfo);
            //PlayerDrawLayers.DrawPlayer_37_BeetleBuff(ref drawInfo);
            //PlayerDrawLayers.DrawPlayer_38_EyebrellaCloud(ref drawInfo);
            //PlayerDrawLayers.DrawPlayer_MakeIntoFirstFractalAfterImage(ref drawInfo);
        }
    }

    /// <summary>
    /// Draws/adds the glow image thing behind the player
    /// </summary>
    public class PlayerGlowEffectLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition() => PlayerDrawLayers.BeforeFirstVanillaLayer;
        public override bool IsHeadLayer => false;

        protected override void Draw(ref PlayerDrawSet _set)
        {
            // get godmodeplayer
            if (!GodModePlayer.GodMode)
            {
                return;
            }

            if (PlayerGlowEffect.ready)
            {
                // draw the draw data 4 times, but shifted in different
                // directions for each. This gives it a softer
                // gradiant and fills in more holes
                PlayerGlowEffect.sData.position = _set.Position - Main.screenPosition + (_set.drawPlayer.Size / 2).Floor();
                DrawData d0, d1, d2, d3;
                d0 = d1 = d2 = d3 = PlayerGlowEffect.sData;
                d0.position += Vector2.UnitY * 3;
                d1.position += Vector2.UnitY * -3;
                d2.position += Vector2.UnitX * 3;
                d3.position += Vector2.UnitY * -3;
                _set.DrawDataCache.Add(d0);
                _set.DrawDataCache.Add(d1);
                _set.DrawDataCache.Add(d2);
                _set.DrawDataCache.Add(d3);
            }
        }
    }

    /// <summary>
    /// An empty item created only for its
    /// ItemID. All armor shaders require themselves
    /// to be bound to a specific item type.
    /// </summary>
    public class BorderShaderDye : ModItem
    {
        public override string Texture => "Terraria/Images/Item_" + ItemID.ColorOnlyDye;

        public override void SetStaticDefaults()
        {
            if (!Main.dedServ)
            {
                Config c = ModContent.GetInstance<Config>();
                if (c != null)
                {
                    if (c.GodModeOutlineSize == "Small")
                    {
                        GameShaders.Armor.BindShader(Type, new ArmorShaderData(Mod.Assets.Request<Effect>("Effects/LessOutlineEffect"), "Pass0"));
                    }
                    else if (c.GodModeOutlineSize == "Big")
                    {
                        GameShaders.Armor.BindShader(Type, new ArmorShaderData(Mod.Assets.Request<Effect>("Effects/OutlineEffect"), "Pass0"));
                    }
                }
            }
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.ColorOnlyDye);
        }
    }
}