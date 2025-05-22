using Microsoft.Xna.Framework.Graphics;
using ModReloader.Common.Systems;
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
            if (!Conf.C.ShowBuilderToggle) return false;

            MainSystem sys = ModContent.GetInstance<MainSystem>();
            sys.mainState.Active = !sys.mainState.Active; // Toggle the property

            sound = sys.mainState.Active ? SoundID.MenuClose : SoundID.MenuClose;
            return true; // Returning true will actually toggle the state.
                         // * Returning false will not toggle the state, but will still play the sound. */
        }

        public override bool Draw(SpriteBatch spriteBatch, ref BuilderToggleDrawParams drawParams)
        {
            if (!Conf.C.ShowBuilderToggle) return false;

            drawParams.Frame = drawParams.Texture.Frame(1, 2, 0, CurrentState);
            return base.Draw(spriteBatch, ref drawParams);
        }
    }
}