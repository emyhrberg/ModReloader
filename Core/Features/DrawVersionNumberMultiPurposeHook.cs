using ModReloader.Core.Features.ExceptionFeatures;
using ModReloader.Core.Features.MainMenuFeatures;

namespace ModReloader.Core.Features;

/// <summary>
/// Multi purposes:
/// Draws exception menu, 
/// copy to clipboard button,
/// 
/// </summary>
internal class DrawVersionNumberMultiPurposeHook : ModSystem
{
    public override void Load()
    {
        On_Main.DrawVersionNumber += OnDrawVersionNumber;
        On_Main.DrawSocialMediaButtons += OnDrawSocialMediaButtons;
        On_Main.DrawtModLoaderSocialMediaButtons += OnDrawtModLoaderSocialMediaButtons;
    }
    public override void Unload()
    {
        On_Main.DrawVersionNumber -= OnDrawVersionNumber;
        On_Main.DrawSocialMediaButtons -= OnDrawSocialMediaButtons;
        On_Main.DrawtModLoaderSocialMediaButtons -= OnDrawtModLoaderSocialMediaButtons;
    }

    private static void OnDrawVersionNumber(On_Main.orig_DrawVersionNumber orig, Color menucolor, float upbump)
    {
        Config config = Conf.C;

        if (config == null)
        {
            orig(menucolor, upbump);
            return;
        }

        if (Main.menuMode != 0 || !config.ShowModsSection)
        {
            orig(menucolor, upbump);
        }

        if (config.ShowBackToMainMenu)
        {
            BackToMainMenuDrawer.DrawBackToMainMenu();
        }

        if (config.ShowCopyToClipboardButton)
        {
            CopyToClipboardDrawer.DrawCopyToClipboard();
        }

        if (config.ShowErrorMenuInfo)
        {
            ExceptionDrawer.DrawExceptionMenuUI();
        }
    }

    private static void OnDrawSocialMediaButtons(On_Main.orig_DrawSocialMediaButtons orig, Color menucolor, float upbump)
    {
        if (Main.menuMode == 0 && Conf.C.ShowModsSection)
        {
            return;
        }
        orig(menucolor, upbump);
    }

    private static void OnDrawtModLoaderSocialMediaButtons(On_Main.orig_DrawtModLoaderSocialMediaButtons orig, Color menucolor, float upbump)
    {
        if (Main.menuMode == 0 && Conf.C.ShowModsSection)
        {
            return;
        }
        orig(menucolor, upbump);
    }

}
