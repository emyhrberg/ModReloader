using System;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SquidTestingMod.Common.Configs;
using SquidTestingMod.Helpers;
using Terraria.ModLoader;
using Terraria.UI;


namespace SquidTestingMod.UI
{
    public class SecondClientButton(Asset<Texture2D> buttonImgText, Asset<Texture2D> buttonImgNoText, string hoverText) : BaseButton(buttonImgText, buttonImgNoText, hoverText)
    {
        public override void LeftClick(UIMouseEvent evt)
        {
            try
            {
                string default_file = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\tModLoader\\start-tModLoader.bat";

                // string my_file = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\tModLoader\\_START_CLIENT.bat";

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo($@"{default_file}") { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Log.Error("Error opening client.log: " + ex.Message);
            }
        }
    }
}