using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using ModReloader.Common.Configs;
using ModReloader.Helpers;
using ReLogic.OS;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace ModReloader.Common.Systems.Hooks
{
    /// <summary>
    /// Try to get a "retry" and avoid disabling mods when not needed.
    /// Also, todo: highlight the line in the error message and provide clickable link to the file.
    /// </summary>
    public class ExceptionHook : ModSystem
    {
        #region hooks
        public override void Load()
        {
            if (Conf.C != null && !Conf.C.ShowCopyToClipboardButton)
            {
                Log.Info("ExceptionHook: ImproveExceptionMenu is set to false. Not hooking into Error Menu.");
                return;
            }
            On_Main.DrawVersionNumber += DrawCopyToClipboard;
        }
        public override void Unload()
        {
            if (Conf.C != null && !Conf.C.ShowCopyToClipboardButton)
            {
                Log.Info("ExceptionHook: ImproveExceptionMenu is set to false. Not unloading the hook.");
                return;
            }
            On_Main.DrawVersionNumber -= DrawCopyToClipboard;
        }
        #endregion

        private static void DrawCopyToClipboard(On_Main.orig_DrawVersionNumber orig, Color menucolor, float upbump)
        {
            // Draw vanilla stuff first
            orig(menucolor, upbump);

            if (IsInErrorUI())
            {
                DrawErrorUIOptions();
            }
        }

        private static bool IsInErrorUI()
        {
            // Check if the feature is enabled in config
            if (Conf.C == null || !Conf.C.ShowErrorMenuInfo)
                return false;

            // Check if we're in an error screen
            Type uiErrorMessageType = typeof(Main).Assembly.GetType("Terraria.ModLoader.UI.UIErrorMessage");
            if (uiErrorMessageType == null)
                return false;

            // Check if the current UI state is an error message
            var state = Main.MenuUI.CurrentState;
            if (state != null && uiErrorMessageType.IsInstanceOfType(state))
            {
                return true;
            }
            return false;
        }

        private static bool WebHelpButtonExists()
        {
            // Get the state
            var state = Main.MenuUI.CurrentState;

            // tracker variables
            bool webHelpFound = false;
            int childrenCount = 0;

            // get area field
            FieldInfo areaField = state.GetType().GetField("area", BindingFlags.NonPublic | BindingFlags.Instance);
            if (areaField.GetValue(state) is UIElement area)
            {
                foreach (var child in area.Children)
                {
                    childrenCount++;
                    if (child is UITextPanel<string> textPanel && textPanel.Text == "Open Web Help")
                    {
                        Log.SlowInfo("Found Web Help button. Moving our button.");
                        webHelpFound = true;
                    }
                }
            }
            //Log.Info("Children count: " + childrenCount);
            return webHelpFound;
        }

        private static void CopyErrorMessage(string errorMessage)
        {
            Platform.Get<IClipboard>().Value = errorMessage;
            timeOnCopyOptionPressed = DateTime.Now;

        }

        private static DateTime timeOnCopyOptionPressed;

        private static void DrawErrorUIOptions()
        {
            // Get the state
            var state = Main.MenuUI.CurrentState;

            // Get the error message
            Type uiErrorMessageType = typeof(Main).Assembly.GetType("Terraria.ModLoader.UI.UIErrorMessage");
            FieldInfo messageField = uiErrorMessageType.GetField("message", BindingFlags.NonPublic | BindingFlags.Instance);
            string errorMessage = messageField?.GetValue(state) as string;

            if (WebHelpButtonExists())
            {
                Log.SlowInfo("Ugh, web help.", seconds: 5);
            }

            // Start at top-left corner
            var drawPos = new Vector2(15, 15);

            // Get names and tooltips for menu options
            string fileName = Path.GetFileName(Logging.LogPath);
            string reloadHoverMods = ReloadUtilities.IsModsToReloadEmpty ? "No mods selected" : string.Join(",", Conf.C.ModsToReload);

            Mod mod = ModReloader.Instance;

            // Copy tooltip will change based on last time pressed
            string copyTooltip;
            if (timeOnCopyOptionPressed == DateTime.MinValue)
            {
                copyTooltip = "Copy error message to clipboard";
            }
            else
            {
                TimeSpan timeSinceCopy = DateTime.Now - timeOnCopyOptionPressed;
                copyTooltip = timeSinceCopy.TotalSeconds <= 1 ? "Copied!" : "Copy error message to clipboard";
            }

            // Menu options with corresponding actions
            var menuOptions = new (string Text, Action Action, float scale, string tooltip)[]
            {
                ($"{mod.DisplayNameClean} v{mod.Version}", null, 1.15f, "Welcome to Mod Reloaders UIError menu!"),
                 ("Reload", async () => await ReloadUtilities.SinglePlayerReload(), 1.02f, $"Reloads {reloadHoverMods}"),
                (" ", null, 1.15f, ""), // empty line
                ("Open Log", Conf.C.OpenLogType == "File" ? Log.OpenClientLog : Log.OpenLogFolder, 1.02f, $"Click to open the {fileName} of this client"),
                ("Clear Log", Log.ClearClientLog, 1.02f, $"Click to clear the {fileName} of this client"),
                ($"Copy", () => CopyErrorMessage(errorMessage), 1.02f, copyTooltip),
                ($"Go to file", () => OpenFileWithException(errorMessage), 1.02f, "Open VS with the file with the exception"),
            };

            foreach (var (text, action, scale, tooltip) in menuOptions)
            {
                // Measure text
                Vector2 size = FontAssets.MouseText.Value.MeasureString(text) * 0.9f;
                size.Y *= 0.9f; // Increase the Y size by 50%
                Vector2 hoverSize = new Vector2(size.X, size.Y * 1.26f);
                // Check if mouse is hovering it
                bool hovered = Main.MouseScreen.Between(drawPos, drawPos + hoverSize);

                string tooltipToShow = tooltip;

                if (hovered)
                {
                    // Draw tooltip
                    DrawHelper.DrawMainMenuTooltipPanel(tooltip, extraXOffset: 130, extraYOffset: -430, extraHeight: 65, extraWidth: -150);

                    Main.LocalPlayer.mouseInterface = true;
                    // Click
                    if (Main.mouseLeft && Main.mouseLeftRelease && action != null)
                    {
                        SoundEngine.PlaySound(SoundID.MenuOpen);
                        Main.mouseLeftRelease = false;
                        action?.Invoke(); // Call the corresponding action
                    }
                }

                // Choose color/alpha
                Color textColor = hovered ? new Color(237, 246, 255) : new Color(173, 173, 198);
                float alpha = hovered ? 1f : 0.76f;
                if (action == null)
                {
                    alpha = 1f;
                    textColor = new Color(237, 246, 255);
                }

                // Draw with an outline
                DrawHelper.DrawOutlinedStringOnMenu(Main.spriteBatch, FontAssets.MouseText.Value, text, drawPos, textColor,
                    rotation: 0f, origin: Vector2.Zero, scale: scale, effects: SpriteEffects.None, layerDepth: 0f,
                    alphaMult: alpha);

                // Draw debug
                //Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value,
                //    new Rectangle((int)drawPos.X, (int)drawPos.Y, (int)hoverSize.X, (int)hoverSize.Y),
                //    Color.Red * 0.5f // Semi-transparent red.
                //);

                // Move down for the next line
                drawPos.Y += size.Y + 6f;
            }
        }

        #region open IDE

        private static void OpenFileWithException(string errorMessage)
        {
            Log.Info("► ExtractAndLogFileLine: starting");
            string file = "";
            int line = 0;

            try
            {
                // --------------------------------------------------------------------
                // 1) tModLoader / Roslyn compile error: C:\path\Foo.cs(42,10): error…
                // --------------------------------------------------------------------
                var compileMatch = Regex.Match(
                    errorMessage,
                    @"^\s*(?:Error|Warning)?:?\s*(?<file>[^\(\r\n]+?)\((?<line>\d+),\d+\):",
                    RegexOptions.Multiline | RegexOptions.IgnoreCase);

                if (compileMatch.Success)
                {
                    file = compileMatch.Groups["file"].Value.Trim();
                    line = int.Parse(compileMatch.Groups["line"].Value);
                    Log.Info($"✔ Matched compile-time pattern   →  file=\"{file}\", line={line}");
                }

                bool opened = false;

                string[] vsVersions = ["2022", "2019"];
                string[] vsEditions = ["Community", "Professional", "Enterprise"];
                string[] progRoots =
                [
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
                ];

                foreach (var root in progRoots.Where(r => !string.IsNullOrEmpty(r) && Directory.Exists(r)))
                {
                    foreach (var ver in vsVersions)
                    {
                        foreach (var ed in vsEditions)
                        {
                            var vsExe = Path.Combine(root, "Microsoft Visual Studio", ver, ed,
                                                     "Common7", "IDE", "devenv.exe");

                            /* NEW: skip missing executables instead of trying to start them */
                            if (!File.Exists(vsExe))
                            {
                                Log.Info($"VS {ver} {ed} not found — skipping.");
                                continue;
                            }

                            try
                            {
                                Log.Info($"→ Opening Visual Studio {ver} {ed}");
                                Process.Start(vsExe,
                                    $"/edit \"{file}\" /command \"Edit.GoTo {line}\"");
                                opened = true;
                                break;                    // success
                            }
                            catch (Win32Exception w32)
                            {
                                Log.Warn($"Win32 launch error: {w32.Message}");
                            }
                            catch (Exception ex)
                            {
                                Log.Warn($"Launch failed: {ex.Message}");
                            }
                        }
                        if (opened) break;
                    }

                }

                // VS Code
                if (!opened)
                {
                    try
                    {
                        Log.Info("→ Trying VS Code");
                        Process.Start("code", $"--goto \"{file}:{line}\"");
                        opened = true;
                    }
                    catch { /* ignore */ }
                }

                // Notepad++
                if (!opened)
                {
                    try
                    {
                        Log.Info("→ Trying Notepad++");
                        Process.Start("notepad++", $"-n{line} \"{file}\"");
                        opened = true;
                    }
                    catch { /* ignore */ }
                }

                // default shell
                if (!opened)
                {
                    try
                    {
                        Log.Info("→ Falling back to OS default editor");
                        Process.Start(new ProcessStartInfo { FileName = file, UseShellExecute = true });
                    }
                    catch (Exception ex)
                    {
                        Log.Error($"✖ Final fallback failed: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"✖ Extraction threw: {ex.Message}");
            }
        }

        #endregion

    }
}
