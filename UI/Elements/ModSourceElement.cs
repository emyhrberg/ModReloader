using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using ModHelper.Common.Configs;
using ModHelper.Helpers;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace ModHelper.UI.Elements
{
    // Contains:
    // Icon image
    // Mod name
    // Checkbox to set this mod to current and all others to unselected
    // Build and reload icon
    // Some padding
    // A folder icon to open this mod's folder
    // A .csproj icon which opens the project in Visual Studio
    public class ModSourcesElement : UIPanel
    {
        public string cleanModName;
        public string modPath;
        public ModCheckbox checkbox;
        public ModSourcesIcon modIcon;

        public ModSourcesElement(string fullModPath, string cleanName = "", string builtAgo = "")
        {
            // size and position
            Width.Set(-35f, 1f);
            Height.Set(30, 0);
            Left.Set(5, 0);

            // last modified
            DateTime lastModified = File.GetLastWriteTime(fullModPath);

            // mod path
            this.modPath = fullModPath;

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

            // mod name
            // modSourcePathString = Path.GetFileName(modPath);
            cleanModName = cleanName;
            if (cleanModName.Length > 20)
                cleanModName = string.Concat(cleanModName.AsSpan(0, 20), "...");

            string hoverText = internalNameFolderName;
            if (isModEnabled)
            {
                hoverText = $"Open {internalNameFolderName} config";
            }

            ModTitleText modNameText = new(text: cleanModName, hover: hoverText, internalModName: internalNameFolderName, clickToOpenConfig: isModEnabled);
            modNameText.Left.Set(30, 0);
            modNameText.VAlign = 0.5f;
            Append(modNameText);

            // distances for icons
            float def = -22f;
            float dist = 27f;

            // checkbox icon
            checkbox = new(Ass.ModUncheck.Value, modSourcePathString: internalNameFolderName, $"Click to add {internalNameFolderName} to the list of mods to reload");
            checkbox.Left.Set(def - dist * 3, 1f);
            Append(checkbox);

            // if this is the current mod, add checkmark
            bool isCurrentModToReload = Conf.ModToReload == internalNameFolderName;
            if (isCurrentModToReload)
            {
                checkbox.SetCheckState(true);

                // add initial mod
                if (!ModsToReload.modsToReload.Contains(internalNameFolderName))
                {
                    Log.Info($"Added {internalNameFolderName} to ModsToReload.");
                    ModsToReload.modsToReload.Add(internalNameFolderName);
                }
            }


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