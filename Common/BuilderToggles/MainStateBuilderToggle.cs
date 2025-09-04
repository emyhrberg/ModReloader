using Microsoft.Xna.Framework.Graphics;
using ModReloader.Core;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;

namespace ModReloader.Common.BuilderToggles
{
    /// <summary>
    /// Reference:
    /// https://github.com/JavidPack/DPSExtreme/blob/1.4/BuilderToggles/ToggleUIBuildersToggle.cs
    /// </summary>
    public class MainStateBuilderToggle : BuilderToggle
    {
        public static LocalizedText OnText { get; private set; }
        public static LocalizedText OffText { get; private set; }

        public override bool Active() => true;

        public override int NumberOfStates => 2;

        public override void SetStaticDefaults()
        {
            OnText = this.GetLocalization(nameof(OnText));
            OffText = this.GetLocalization(nameof(OffText));
        }

        public override string DisplayValue()
        {
            if (CurrentState == 0)
                return OnText.Value;
            else
                return OffText.Value;
        }

        public override bool OnLeftClick(ref SoundStyle? sound)
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();

            // Returning true will flip CurrentState after this method.
            // If CurrentState == 0 (On), it will become Off; otherwise it will become On.
            bool willBeActive = CurrentState != 0;

            sys.mainState.Active = willBeActive;

            if (willBeActive)
            {
                sys.userInterface?.SetState(sys.mainState);
            }
            else
            {
                sys.userInterface?.SetState(null);
            }

            sound = SoundID.MenuClose;
            return true;
        }

        public override bool Draw(SpriteBatch spriteBatch, ref BuilderToggleDrawParams drawParams)
        {
            drawParams.Frame = drawParams.Texture.Frame(1, 2, 0, CurrentState);
            return base.Draw(spriteBatch, ref drawParams);
        }
    }
}
