using System.IO;
using Microsoft.Xna.Framework.Graphics;
using ModReloader.Common.Integrations.DragonLens;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;

namespace ModReloader.Core.Features.DebugTextInWorld
{
    public class DebugText : UIText
    {
        public DebugText(string text, float textScale = 0.9f, bool large = false) : base(text, textScale, large)
        {
            TextColor = Color.White;
            VAlign = 1.00f;
            HAlign = 0.005f;

            // start text at top left corner of its element
            TextOriginX = 0f;
            TextOriginY = 0f;

            // Arbitrary size, should use ChatManager.GetStringSize() instead
            Width.Set(150, 0);
            Top.Set(-5, 0);
            Height.Set(20 * 3 + 10, 0);
        }

        public override void MouseOver(UIMouseEvent evt)
        {
            //always call base! otherwise IsMouseHovering wont work
            base.MouseOver(evt);
            // TextColor = Color.Yellow;
        }

        public override void MouseOut(UIMouseEvent evt)
        {
            //always call base! otherwise IsMouseHovering wont work
            base.MouseOut(evt);
            // TextColor = Color.White;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);

            Conf.C.ShowDebugInfo = !Conf.C.ShowDebugInfo;
            Conf.Save();

            // Open client log
            // Log.OpenClientLog();
        }

        public override void RightClick(UIMouseEvent evt)
        {
            if (!Conf.C.ShowDebugInfo)
            {
                // if debug info is not active, do nothing
                Log.Info("Debug info is not active, ignoring right click.");
                return;
            }

            base.RightClick(evt);

            if (IsConfigOpenAnywhere())
            {
                // close any open config UI
                Main.menuMode = 0;
                //Main.InGameUI.SetState(null);
                IngameFancyUI.Close();
            }
            else
            {
                // open the config
                Conf.C.Open();
            }
        }

        // Helper: is any tML config UI currently open?
        private bool IsConfigOpenAnywhere()
        {
            if (Main.InGameUI == null)
            {
                return false;
            }

            var currentState = Main.InGameUI.CurrentState;

            if (currentState == null)
            {
                //Log.SlowInfo("Config is not open (InGameUI state is null).");
                return false;
            }

            var modConfig = Terraria.ModLoader.UI.Interface.modConfig;

            bool isConfigOpen = currentState.GetType() == modConfig.GetType();
            //Log.SlowInfo("isConfigOpen: " + isConfigOpen);
            return isConfigOpen;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!Conf.C.ShowDebugInfo)
            {
                return;
            }

            // update the text to show playername, whoAmI, and FPS
            string playerName = Main.LocalPlayer.name;
            if (playerName.Length > 9)
                playerName = playerName.Substring(0, 7) + "...";

            string worldName = Main.worldName;
            if (worldName.Length > 9)
                worldName = worldName.Substring(0, 7) + "...";
            int whoAmI = Main.myPlayer;
            int fps = Main.frameRate;
            int ups = Main.updateRate;

            string netmode = Main.netMode switch
            {
                NetmodeID.SinglePlayer => "SP",
                NetmodeID.MultiplayerClient => "MP",
                _ => "Unknown"
            };

            string logFileName = Path.GetFileName(Logging.LogPath);

            string text = "";
            //text += $"\nName: {playerName}, ID: {whoAmI}, Mode: {netmode}";
            //text += $"\nDebugger: {Debugger.IsAttached}, PID: {System.Environment.ProcessId}";
            //text += $"\n{fps}fps {ups}ups ({Main.upTimerMax:0}ms)";

            if (Main.netMode == NetmodeID.MultiplayerClient)
                text += $"\nP: {playerName} ({Main.myPlayer})\nW: {worldName}";
            else if (Main.netMode == NetmodeID.SinglePlayer)
                text += $"\nP: {playerName} \nW: {worldName}";

            //Main.instance.Window.Title = " += PID HERE? FOR EASY DEBUG INFO";

            SetText(text, 0.9f, large: false);

            if (IsConfigOpenAnywhere())
            {
                if (!ModLoader.TryGetMod("DragonLens", out _))
                {
                    //Log.Info("DragonLens is not loaded, skipping icon addition.");
                    return;
                }
                // ensure dragonlens assets are added. if not, add them
                DragonLensIntegration.AddIcons();
            }
        }

        public override void Draw(SpriteBatch sb)
        {
            //Top.Set(-5, 0);
            //Height.Set(20 * 3+10, 0);

            //DrawHelper.DrawDebugHitbox(this, Color.Green);

            if (!Conf.C.ShowDebugInfo)
            {
                return;
            }

            // also, if chat is open, hide the text
            //if (Main.drawingPlayerChat)
            //{
            //return;
            //}

            base.Draw(sb);

            if (IsMouseHovering)
            {
                //Vector2 pos = new(Main.MouseScreen.X-16, Main.MouseScreen.Y-24);
                CalculatedStyle dims = GetDimensions();
                Vector2 posHigh = dims.Position() + new Vector2(0, -18);

                DrawHelper.DrawOutlinedStringOnMenu(sb, FontAssets.MouseText.Value, Loc.Get("DebugText.ClickToHide"), posHigh, Color.White,
                    rotation: 0f, origin: Vector2.Zero, scale: 0.8f, effects: SpriteEffects.None, layerDepth: 0f,
                    alphaMult: 0.8f);

                Vector2 posLow = dims.Position() + new Vector2(0, 3);

                string configOpenCloseTooltip;
                //if (IsConfigOpenAnywhere())
                //{
                //configOpenCloseTooltip = "Right click to close config";
                //}
                //else
                //{
                configOpenCloseTooltip = Loc.Get("DebugText.RightClickOpenConfig");
                //}

                DrawHelper.DrawOutlinedStringOnMenu(sb, FontAssets.MouseText.Value, configOpenCloseTooltip, posLow, Color.White,
                    rotation: 0f, origin: Vector2.Zero, scale: 0.8f, effects: SpriteEffects.None, layerDepth: 0f,
                    alphaMult: 0.8f);
            }
        }
    }
}