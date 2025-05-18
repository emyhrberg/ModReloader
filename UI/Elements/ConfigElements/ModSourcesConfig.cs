using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

namespace ModReloader.UI.Elements.ConfigElements
{
    /// <summary>
    /// A panel to display the Mod Sources
    /// </summary>
    public class ModSourcesConfig : ConfigElement
    {
        public IList<List<string>> ListList { get; set; }

        public ModSourcesPanelConfig modSourcesPanelConfig;

        public override void OnBind()
        {
            base.OnBind();

            DrawLabel = false;

            Height.Set(280, 0f);

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

        public override void Update(GameTime gameTime)
        {
            // hot reload testing/fixing

            base.Update(gameTime);
            //TooltipFunction = () => GetDynamicTooltip();
        }

        public override void Draw(SpriteBatch sb)
        {
            //base.Draw(sb);
            Height.Set(280, 0);

            Top.Set(5, 0);

            // Draw children only
            for (int i = 0; i < Elements.Count; i++)
            {
                Elements[i].Draw(sb);
            }
        }

        public virtual void SetValue(List<string> value)
        {
            SetObject(value);
        }
    }
}
