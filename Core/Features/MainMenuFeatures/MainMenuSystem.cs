using Terraria.UI;

namespace ModReloader.Core.Features.MainMenuFeatures;

[Autoload(Side = ModSide.Client)]
internal sealed class MainMenuSystem : ModSystem
{
    private UserInterface ui;
    public MainMenuState state;

    public override void PostSetupContent()
    {
        if (Main.dedServ) return;

        ui = new UserInterface();
        state = new MainMenuState();
        ui.SetState(state);

        On_Main.DrawMenu += PreDrawMenu;
        On_Main.UpdateUIStates += PostUpdateUIStates;
    }

    private void PreDrawMenu(On_Main.orig_DrawMenu orig, Main self, GameTime gameTime)
    {
        if (Main.gameMenu && Main.menuMode == 0 && ui?.CurrentState != null)
        {
            ui.Draw(Main.spriteBatch, new GameTime());
        }

        orig(self, gameTime);
    }

    private void PostUpdateUIStates(On_Main.orig_UpdateUIStates orig, GameTime gameTime)
    {
        if (Main.gameMenu && Main.menuMode == 0)
        {
            if (ui.CurrentState == null)
                ui.SetState(state);

            ui.Update(gameTime);
        }
        else if (ui.CurrentState != null)
        {
            ui.SetState(null);
        }

        orig(gameTime);
    }

    public override void Unload()
    {
        On_Main.DrawMenu -= PreDrawMenu;
        On_Main.UpdateUIStates -= PostUpdateUIStates;
        ui = null;
        state = null;
    }
}
