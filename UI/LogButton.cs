using System;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Helpers;
using Terraria.UI;

namespace SquidTestingMod.UI
{
    public class LogButton(Asset<Texture2D> _image, string hoverText) : BaseButton(_image, hoverText)
    {
        public override void LeftClick(UIMouseEvent evt)
        {
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