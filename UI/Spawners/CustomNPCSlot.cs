using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace SquidTestingMod.UI.Spawners
{
    // A minimal NPCSlot that displays an NPC in a shop-style slot.
    public class CustomNPCSlot : UIElement
    {
        private int slotContext;
        private NPC displayNPC;

        // Animation fields
        private float bgScale = 0.6f; // Scale for the background slot
        private const int desiredSize = 40; // Target size for the NPC sprite drawing
        private int frameCounter = 0;
        private int frameTimer = 0;
        private const int frameDelay = 7; // Adjust as needed

        // Constructor: takes an NPC and a slot context.
        public CustomNPCSlot(NPC npc, int slotContext)
        {
            this.slotContext = slotContext;
            displayNPC = npc;
            // Set desired clickable area (adjust as needed)
            Width.Set(40, 0f);
            Height.Set(40, 0f);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Draw background inventory slot (using InventoryBack)
            CalculatedStyle dimensions = GetInnerDimensions();
            float bgOpacity = IsMouseHovering ? 0.9f : 0.6f;
            Texture2D bg = TextureAssets.InventoryBack.Value;
            spriteBatch.Draw(bg, dimensions.ToRectangle(), Color.White * bgOpacity);

            // Load the NPC texture and update animation frames.
            Main.instance.LoadNPC(displayNPC.type);
            Texture2D npcTexture = TextureAssets.Npc[displayNPC.type].Value;
            int totalFrames = Main.npcFrameCount[displayNPC.type];
            if (totalFrames <= 0)
                totalFrames = 1;

            // Update frame timer and counter.
            frameTimer++;
            if (frameTimer > frameDelay)
            {
                frameCounter = (frameCounter + 1) % totalFrames;
                frameTimer = 0;
            }

            // Calculate the source rectangle for the current frame.
            int frameHeight = npcTexture.Height / totalFrames;
            Rectangle npcDrawRectangle = new Rectangle(0, frameCounter * frameHeight, npcTexture.Width, frameHeight);
            float scale = desiredSize / (float)Math.Max(npcTexture.Width, frameHeight);

            // Center the NPC sprite within the background slot.
            Vector2 drawPos = dimensions.Position();
            drawPos.X += (dimensions.Width - npcTexture.Width * scale) / 2f;
            drawPos.Y += (dimensions.Height - frameHeight * scale) / 2f;

            spriteBatch.Draw(npcTexture, drawPos, npcDrawRectangle, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            // If hovering, display the NPC's name.
            if (IsMouseHovering)
            {
                string hoverText = "";

                if (displayNPC.FullName != null)
                    hoverText += $"Name: {displayNPC.FullName}\n";

                if (displayNPC.type != NPCID.None)
                    hoverText += $"ID: {displayNPC.type}\n";

                if (displayNPC.lifeMax != 0)
                    hoverText += $"Health: {displayNPC.lifeMax}\n";

                if (displayNPC.defense != 0)
                    hoverText += $"Defense: {displayNPC.defense}\n";

                if (displayNPC.damage != 0)
                    hoverText += $"Damage: {displayNPC.damage}\n";

                UICommon.TooltipMouseText(hoverText);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Check if the right mouse button is being held down
            if (IsMouseHovering && Main.mouseRight)
            {
                MainSystem sys = ModContent.GetInstance<MainSystem>();
                if (sys.mainState.npcSpawnerPanel.IsDraggingNPCPanel || sys.mainState.npcSpawnerPanel.GetNPCPanelActive() == false)
                {
                    Log.Info("Dont spawn NPC, panel is hidden");
                    return;
                }

                // Spawn the NPC 200 tiles above and 200 tiles to the left of players pos
                // Only singleplayer
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    return;
                }

                float playerX = Main.LocalPlayer.position.X;
                float playerY = Main.LocalPlayer.position.Y;

                int desiredX = (int)(playerX + Conf.NPCSpawnLocation.X);
                int desiredY = (int)(playerY + Conf.NPCSpawnLocation.Y);
                NPC.NewNPC(new MyCustomNPCSource("CustomData"), desiredX, desiredY, displayNPC.type);
            }
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            // Spawn the NPC 200 tiles above and 200 tiles to the left of players pos
            // Only singleplayer
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return;
            }

            MainSystem sys = ModContent.GetInstance<MainSystem>();
            if (sys.mainState.npcSpawnerPanel.IsDraggingNPCPanel || sys.mainState.npcSpawnerPanel.GetNPCPanelActive() == false)
            {
                Log.Info("Dont spawn NPC, panel is hidden");
                return;
            }

            float playerX = Main.LocalPlayer.position.X;
            float playerY = Main.LocalPlayer.position.Y;

            int desiredX = (int)(playerX + Conf.NPCSpawnLocation.X);
            int desiredY = (int)(playerY + Conf.NPCSpawnLocation.Y);
            NPC.NewNPC(new MyCustomNPCSource("CustomData"), desiredX, desiredY, displayNPC.type);
            Log.Info("Spawned NPC " + displayNPC.FullName + " at " + desiredX + ", " + desiredY);
        }
    }

    public class MyCustomNPCSource(string customData) : IEntitySource
    {
        public string RandomStringForNoReason = customData;

        public string Context => "NPCSpawnFromNPCSlotClass";
    }

}


