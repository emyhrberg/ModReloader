using System;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class LogButton(Asset<Texture2D> buttonImgText, Asset<Texture2D> buttonImgNoText, string hoverText) : BaseButton(buttonImgText, buttonImgNoText, hoverText)
    {
        public override void LeftClick(UIMouseEvent evt)
        {
            // open client.log at 
            // C:\Program Files (x86)\Steam\steamapps\common\tModLoader\tModLoader-Logs\client.log 
            // string file = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\tModLoader\\tModLoader-Logs\\client.log";
            // System.Diagnostics.Process.Start(file);

            try
            {
                string file = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\tModLoader\\tModLoader-Logs\\client.log";
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo($@"{file}") { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Log.Error("Error opening client.log: " + ex.Message);
            }

        }
    }
}