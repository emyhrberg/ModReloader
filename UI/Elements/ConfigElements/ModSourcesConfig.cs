using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ModHelper.UI.AbstractElements;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Config.UI;
using Terraria.ModLoader.UI;

namespace ModHelper.UI.ModElements
{
    /// <summary>
    /// A panel to display the contents of client.log.
    /// </summary>
    public class ModSourcesConfig : ConfigElement
    {
        public IList<List<string>> ListList { get; set; }

        public ModSourcesPanelConfig modSourcesPanelConfig;

        public override void OnBind()
        {
            base.OnBind();

            Height.Set(300, 0f);

            ListList = (IList<List<string>>)List;

            if (ListList != null)
            {
                TextDisplayFunction = () => Index + 1 + ": " + ListList[Index].ToString();
            }

            modSourcesPanelConfig = new ModSourcesPanelConfig(this);
            Append(modSourcesPanelConfig);

            Recalculate();
        }


        public virtual List<string> GetValue() => (List<string>)GetObject();

        public virtual void SetValue(List<string> value)
        {
            SetObject(value);
        }
    }
}
