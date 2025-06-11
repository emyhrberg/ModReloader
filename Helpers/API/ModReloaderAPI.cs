using System;
using Microsoft.Xna.Framework.Graphics;
using ModReloader.Common.Systems;
using ReLogic.Content;

namespace ModReloader.Helpers.API
{
    public static class ModReloaderAPI
    {
        public static bool AddButton(string name, Action action, Asset<Texture2D> asset = null, string tooltip = null, Func<bool> showHighlight = null)
        {
            try
            {
                var mainSystem = ModContent.GetInstance<MainSystem>();
                if (mainSystem?.mainState == null)
                {
                    Log.Error("MainState not available yet");
                    return false;
                }

                var button = mainSystem.mainState.AddButton<APIButton>(
                    spritesheet: asset,
                    buttonText: name,
                    hoverText: name,
                    hoverTextDescription: tooltip ?? name
                );

                button.SetAction(action);
                button.ShouldShowHighlight = showHighlight ?? (() => false);
                return true;
            }
            catch (Exception e)
            {
                Log.Error($"Failed to add button '{name}': {e.Message}");
                return false;
            }
        }
    }
}