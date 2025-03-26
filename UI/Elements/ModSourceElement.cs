using System;
using System.Collections.Generic;
using System.IO;
using ErkysModdingUtilities.Common.Configs;
using ErkysModdingUtilities.Helpers;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace ErkysModdingUtilities.UI.Elements
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
        public string modSourcePathString;
        public ModCheckbox checkbox;
        public ModSourcesIcon modIcon;

        public ModSourcesElement(string modPath, string cleanName = "")
        {
            // size and position
            Width.Set(-35f, 1f);
            Height.Set(30, 0);
            Left.Set(5, 0);

            // mod icon
            string iconPath = Path.Combine(modPath, "icon.png");
            if (File.Exists(iconPath))
            {
                // Defer texture creation to the main thread:
                Main.QueueMainThreadAction(() =>
                {
                    using var stream = File.OpenRead(iconPath);
                    Texture2D texture = Texture2D.FromStream(
                        graphicsDevice: Main.graphics.GraphicsDevice,
                        stream: stream);
                    modIcon = new(texture);
                    Append(modIcon);
                });
            }
            else
            {
                Log.Info("No icon found. Substituting default icon for " + modPath);

                Asset<Texture2D> defaultIcon = Main.Assets.Request<Texture2D>("Images/UI/DefaultResourcePackIcon", AssetRequestMode.ImmediateLoad);

                Main.QueueMainThreadAction(() =>
                {
                    modIcon = new(defaultIcon.Value);
                    Append(modIcon);
                });
            }

            // mod name
            // modSourcePathString = Path.GetFileName(modPath);
            modSourcePathString = cleanName;
            if (modSourcePathString.Length > 20)
                modSourcePathString = string.Concat(modSourcePathString.AsSpan(0, 20), "...");
            OptionTitleText modNameText = new(text: modSourcePathString, hover: $"Open {modSourcePathString} config", internalModName: modSourcePathString);
            modNameText.Left.Set(30, 0);
            modNameText.VAlign = 0.5f;
            Append(modNameText);

            // distances for icons
            float def = -22f;
            float dist = 27f;

            // checkbox icon
            checkbox = new(Ass.ModUncheck.Value, modSourcePathString, $"Click to make {modSourcePathString} the mod to reload");
            checkbox.Left.Set(def - dist * 3, 1f);
            Append(checkbox);

            // if this is the current mod, add checkmark
            bool isCurrentModToReload = Conf.ModToReload == modSourcePathString;
            if (isCurrentModToReload)
            {
                checkbox.SetCheckState(true);

                // add initial mod
                if (!ModsToReload.modsToReload.Contains(modSourcePathString))
                {
                    ModsToReload.modsToReload.Add(modSourcePathString);
                }
            }


            // reload icon
            ModReloadIcon modReloadIcon = new(Ass.ModReload.Value, modSourcePathString, "Reload " + modSourcePathString);
            modReloadIcon.Left.Set(def - dist * 2, 1f);
            Append(modReloadIcon);

            // folder icon
            ModFolderIcon folderIcon = new(Ass.ModOpenFolder.Value, modPath, "Open Folder");
            folderIcon.Left.Set(def - dist * 1, 1f);
            Append(folderIcon);

            // cs proj icon
            ModProjectIcon projectIcon = new(Ass.ModOpenProject.Value, modPath, "Open .csproj");
            projectIcon.Left.Set(def, 1f);
            Append(projectIcon);
        }
    }
}