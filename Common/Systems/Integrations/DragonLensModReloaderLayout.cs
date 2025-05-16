using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DragonLens.Content.GUI;
using DragonLens.Core.Systems.ToolbarSystem;

namespace ModReloader.Common.Systems.Integrations
{
    /// Tries to add reload as a 'tool' to DragonLens
    [JITWhenModsEnabled("DragonLens")]
    [ExtendsFromMod("DragonLens")]
    public class DragonLensModReloaderLayout : ModSystem
    {
        public override void PostSetupContent()
        {
            AddLayout();
        }

        private void AddLayout()
        {
            // get filename from this mod folder
            string fileName = "DragonLensModReloaderLayout";

            // TODO get grid somehow
            // TODO get the file somehow
            //grid.Add(new LayoutPresetButton(this, "Simple", Path.Join(Main.SavePath, "DragonLensLayouts", "DragonLensModReloaderLayout")));
        }
    }
}