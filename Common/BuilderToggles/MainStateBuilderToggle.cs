using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ModHelper.Common.BuilderToggles
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
            return CurrentState == 0 ? OnText.Value : OffText.Value;
        }

        public override bool OnLeftClick(ref SoundStyle? sound)
        {
            MainSystem sys = ModContent.GetInstance<MainSystem>();
            sys.mainState.Active = !sys.mainState.Active; // Toggle the property

            // Play appropriate sound
            if (sys.mainState.Active)
            {
                sound = SoundID.MenuClose;
            }
            else
            {
                sound = SoundID.MenuClose;
            }

            return true; // Indicate that the state has changed
        }

        public override bool Draw(SpriteBatch spriteBatch, ref BuilderToggleDrawParams drawParams)
        {
            drawParams.Frame = drawParams.Texture.Frame(1, 2, 0, CurrentState);
            return base.Draw(spriteBatch, ref drawParams);
        }
    }
}