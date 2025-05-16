using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using ModReloader.Helpers;
using ModReloader.UI.Elements.PanelElements.ModElements;
using ReLogic.Content;
using Terraria.GameContent.UI.Elements;

namespace ModReloader.UI.Elements.ConfigElements
{
    // Contains:
    // Icon image
    // Mod name
    // Checkbox to set this mod to current and all others to unselected
    // Build and reload icon
    // Some padding
    // A folder icon to open this mod's folder
    // A .csproj icon which opens the project in Visual Studio
    public class ModSourcesElementConfig : UIPanel
    {
        public string cleanModName;
        public string modPath;
        public ModCheckboxConfig checkbox;
        public ModSourcesIcon modIcon;
        public ModSourcesConfig parentConfig;

        public ModSourcesElementConfig(ModSourcesConfig parent, string fullModPath, string cleanName = "")
        {

            parentConfig = parent;
            // size and position
            Width.Set(-35f, 1f);
            Height.Set(30 * 2, 0);
            Left.Set(5, 0);

            // last modified
            DateTime lastModified = File.GetLastWriteTime(fullModPath);

            // mod path
            modPath = fullModPath;

            // mod icon
            string iconPath = Path.Combine(fullModPath, "icon.png");
            if (File.Exists(iconPath))
            {
                // Defer texture creation to the main thread:
                Main.QueueMainThreadAction(() =>
                {
                    using var stream = File.OpenRead(iconPath);
                    Texture2D texture = Texture2D.FromStream(
                        graphicsDevice: Main.graphics.GraphicsDevice,
                        stream: stream);
                    modIcon = new ModSourcesIcon(texture, lastModified: lastModified);
                    Append(modIcon);
                });
            }
            else
            {
                Log.Info("No icon found. Substituting default icon for " + fullModPath);

                Asset<Texture2D> defaultIcon = Main.Assets.Request<Texture2D>("Images/UI/DefaultResourcePackIcon", AssetRequestMode.ImmediateLoad);

                Main.QueueMainThreadAction(() =>
                {
                    modIcon = new ModSourcesIcon(defaultIcon.Value, lastModified: lastModified);
                    Append(modIcon);
                });
            }

            string internalNameFolderName = Path.GetFileName(fullModPath);

            // check if the mod is enabled, we supply a "open config" option.
            bool isModEnabled = false;

            foreach (var mod in ModLoader.Mods.Skip(1))
            {
                if (mod.Name == internalNameFolderName)
                {
                    isModEnabled = true;
                    break;
                }
            }

            // distances for icons
            float def = -22f;
            float dist = 27f;

            // mod name
            // modSourcePathString = Path.GetFileName(modPath);
            cleanModName = cleanName;
            if (cleanModName.Length > 20)
                cleanModName = string.Concat(cleanModName.AsSpan(0, 20), "...");

            string hoverText = internalNameFolderName;
            if (isModEnabled)
            {
                // hoverText = $"Open {internalNameFolderName} config";
                ModConfigIcon modConfigIcon = new(texture: Ass.ConfigOpen, modPath: internalNameFolderName, hover: $"Open config", cleanModName: cleanModName);
                float size = 22f;
                modConfigIcon.MaxHeight.Set(size, 0f);
                modConfigIcon.MaxWidth.Set(size, 0f);
                modConfigIcon.Width.Set(size, 0f);
                modConfigIcon.Height.Set(size, 0f);
                modConfigIcon.Top.Set(-22, 0); // custom top
                modConfigIcon.Left.Set(def, 1f);
                Append(modConfigIcon);
            }

            ModTitleText modNameText = new(text: cleanModName, hover: hoverText, internalModName: internalNameFolderName, textSize: 0.45f, large: true);
            modNameText.Left.Set(60, 0);
            modNameText.Top.Set(-3, 0);
            modNameText.VAlign = 0f;
            Append(modNameText);

            TimeSpan timeAgo = DateTime.Now - lastModified;
            Color timeColor = timeAgo.TotalSeconds < 60 ? new Color(5, 230, 55) :
                                          timeAgo.TotalMinutes < 60 ? new Color(5, 230, 55) :
                                          timeAgo.TotalHours < 24 ? Color.Orange :
                                          Color.Red;

            // add last modified text
            ModSourceLastBuiltText lastModifiedText = new(lastModified);
            lastModifiedText.Left.Set(60, 0);
            lastModifiedText.VAlign = 1f;
            Append(lastModifiedText);

            // checkbox icon
            checkbox = new(parentConfig, Ass.ModUncheck.Value, modSourcePathString: internalNameFolderName, $"Add {internalNameFolderName} to reload");
            checkbox.Left.Set(def - dist * 3, 1f);
            Append(checkbox);

            // reload icon
            ModReloadIcon modReloadIcon = new(Ass.ModReload.Value, internalNameFolderName, "Reload " + internalNameFolderName);
            modReloadIcon.Left.Set(def - dist * 2, 1f);
            Append(modReloadIcon);

            // folder icon
            ModFolderIcon folderIcon = new(Ass.ModOpenFolder.Value, fullModPath, "Open Folder");
            folderIcon.Left.Set(def - dist * 1, 1f);
            Append(folderIcon);

            // cs proj icon
            ModProjectIcon projectIcon = new(Ass.ModOpenProject.Value, fullModPath, "Open .csproj");
            projectIcon.Left.Set(def, 1f);
            Append(projectIcon);
        }
    }
}