using System.Collections.Generic;
using Terraria.ModLoader.Config.UI;

namespace ModReloader.UI.Elements.ConfigElements
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
