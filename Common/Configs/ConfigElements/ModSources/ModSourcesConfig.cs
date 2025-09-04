using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader.Config.UI;
using Terraria.UI;

namespace ModReloader.Common.Configs.ConfigElements.ModSources
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

            // Set height based on number of mods
            int modCount = GetModSourcesCount();
            Log.Info("Found " + modCount + " ModSources to bind to config");
            bool needScrollbar = false;
            if (modCount == 0)
            {
                Height.Set(35, 0);
                DrawLabel = true;
                return;
            }
            else if (modCount == 1)
            {
                Height.Set(95, 0);
            }
            else if (modCount == 2)
            {
                Height.Set(170, 0);
            }
            else if (modCount == 3)
            {
                Height.Set(250, 0);
            }
            else if (modCount >= 4)
            {
                Height.Set(280, 0);
                needScrollbar = true;
            }

            Top.Set(5, 0);

            ListList = (IList<List<string>>)List;

            if (ListList != null)
            {
                TextDisplayFunction = () => Index + 1 + ": " + ListList[Index].ToString();
            }

            modSourcesPanelConfig = new ModSourcesPanelConfig(this, needScrollbar);
            Append(modSourcesPanelConfig);

            Recalculate();
        }

        private int GetModSourcesCount()
        {
            return Terraria.ModLoader.Core.ModCompile.FindModSources().Length;
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
            //Height.Set(170, 0);

            //Top.Set(5, 0);

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
