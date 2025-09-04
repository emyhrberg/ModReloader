// ──────────────────────────────────────────────────────────────────────────────
//  Shared helper to signal the player when the Builder‑toggle is OFF.
// ──────────────────────────────────────────────────────────────────────────────

// ──────────────────────────────────────────────────────────────────────────────
//  Shared helper to signal the player when the Builder‑toggle is OFF.
// ──────────────────────────────────────────────────────────────────────────────

// ──────────────────────────────────────────────────────────────────────────────
//  Shared helper to signal the player when the Builder‑toggle is OFF.
// ──────────────────────────────────────────────────────────────────────────────

// ──────────────────────────────────────────────────────────────────────────────
//  Shared helper to signal the player when the Builder‑toggle is OFF.
// ──────────────────────────────────────────────────────────────────────────────
using ModReloader.Common.BuilderToggles;

namespace ModReloader.Common.Integrations
{
    /// <summary>
    /// Displays a localized chat hint when the Mod Reloader toolset is disabled via
    /// the Builder toggle.  All calls become no‑ops while the toggle is ON, so you
    /// can safely sprinkle <see cref="Notify"/> calls anywhere you need feedback.
    /// </summary>
    public static class LeftClickHelper
    {
        private const string DefaultMessageKey = "LeftClickHelper.ModReloaderOffTooltip"; // "Mod Reloader is off…"

        /// <summary>
        /// Shows <paramref name="locKey"/> in chat (orange‑red) if the Builder‑toggle
        /// is OFF. When ON, the UI is visible and no chat spam is produced.
        /// </summary>
        public static void Notify(string locKey = DefaultMessageKey, params object[] args)
        {
            if (BuilderToggleHelper.GetActive())
                return; // toggle ON – nothing to say

            Main.NewText(Loc.Get(locKey, args), Color.OrangeRed);
        }
    }
}