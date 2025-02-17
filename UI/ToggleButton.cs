using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class ToggleButton : BaseButton
    {
        // Constructor
        public ToggleButton(Asset<Texture2D> _image, string hoverText) : base(_image, hoverText)
        {
        }

        public override void UpdateTexture()
        {
            // First update the base (which sets ButtonScale and the default image asset).
            base.UpdateTexture();

            MainSystem sys = ModContent.GetInstance<MainSystem>();
            if (sys == null || sys.mainState == null)
            {
                Log.Info("MainSystem or MainState is null. Cannot update texture.");
                return;
            }

            // Now update the current image asset based on the toggle state.
            if (sys.mainState.AreButtonsShowing)
                _Texture = Assets.ButtonOn;
            else
                _Texture = Assets.ButtonOff;

            SetImage(_Texture);
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            sys?.mainState.ToggleOnOff();
        }

        #region dragging
        public bool dragging;
        private Vector2 dragOffset;
        private Vector2 mouseDownPos;
        public bool isDrag;
        private const float DragThreshold = 10f; // you can tweak the threshold
        public Vector2 anchorPos;

        public override void LeftMouseDown(UIMouseEvent evt)
        {
            base.LeftMouseDown(evt);

            dragging = true;
            isDrag = false;
            mouseDownPos = evt.MousePosition; // store mouse down location
            dragOffset = evt.MousePosition - new Vector2(Left.Pixels, Top.Pixels);
        }

        public override void LeftMouseUp(UIMouseEvent evt)
        {
            base.LeftMouseUp(evt);
            dragging = false;
            Recalculate();

            if (isDrag)
            {
                LeftClick(evt);
            }
        }
        #endregion

        #region update
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (dragging)
            {
                // Check if we moved the mouse more than the threshold
                var currentPos = new Vector2(Main.mouseX, Main.mouseY);
                if (Vector2.Distance(currentPos, mouseDownPos) > DragThreshold)
                {
                    isDrag = true;
                }

                // get anchor pos of this button based on what we have dragOffset it with
                anchorPos = new(Main.mouseX - dragOffset.X, Main.mouseY - dragOffset.Y);

                // update the position of all the buttons
                MainSystem sys = ModContent.GetInstance<MainSystem>();
                sys?.mainState.UpdateButtonsPositions(anchorPos);

                Recalculate();
            }
        }
        #endregion
    }
}