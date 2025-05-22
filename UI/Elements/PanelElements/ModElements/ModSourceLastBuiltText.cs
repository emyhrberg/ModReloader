using System;
using Microsoft.Xna.Framework.Graphics;
using ModReloader.Helpers;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI;

namespace ModReloader.UI.Elements.PanelElements.ModElements
{
    public class ModSourceLastBuiltText : UIText
    {
        private readonly DateTime lastModified;

        public ModSourceLastBuiltText(DateTime lastModified, float textScale = 1.0f, bool large = false)
            : base(FormatTimeAgoText(lastModified), textScale, large)
        {
            this.lastModified = lastModified;

            // Set the color based on the time
            TimeSpan timeAgo = DateTime.Now - lastModified;
            Color timeColor = GetTimeColor(timeAgo);
            TextColor = timeColor;
        }

        public override void Update(GameTime gameTime)
        {
            // update the text
            // if (Conf.C.AlwaysUpdateBuiltAgo)
            // {
            TimeSpan timeAgo = DateTime.Now - lastModified;
            string text = FormatTimeAgoText(lastModified);
            TextColor = GetTimeColor(timeAgo);
            SetText(text, 0.38f, true);
            // }

            base.Update(gameTime);
        }

        private static Color GetTimeColor(TimeSpan timeAgo)
        {
            if (timeAgo.TotalSeconds < 60)
            {
                return Color.Green;
            }
            else if (timeAgo.TotalMinutes < 10)
            {
                return Color.Yellow;
            }
            else if (timeAgo.TotalMinutes < 60)
            {
                return Color.Yellow;
            }
            else if (timeAgo.TotalHours < 24)
            {
                return Color.Orange; // Orange
            }
            else
            {
                return ColorHelper.CalamityRed; // Red
            }
        }

        private static string FormatTimeAgoText(DateTime lastModified)
        {
            TimeSpan timeAgo = DateTime.Now - lastModified;
            if (timeAgo.TotalSeconds < 60)
            {
                return Loc.Get("ModSourceLastBuilt.Seconds", timeAgo.Seconds);
            }
            else if (timeAgo.TotalMinutes < 2)
            {
                return Loc.Get("ModSourceLastBuilt.Minute", timeAgo.Minutes);
            }
            else if (timeAgo.TotalMinutes < 60)
            {
                return Loc.Get("ModSourceLastBuilt.Minutes", timeAgo.Minutes);
            }
            else if (timeAgo.TotalHours < 2)
            {
                return Loc.Get("ModSourceLastBuilt.Hour", timeAgo.Hours);
            }
            else if (timeAgo.TotalHours < 24)
            {
                return Loc.Get("ModSourceLastBuilt.Hours", timeAgo.Hours);
            }
            else if (timeAgo.TotalDays < 2)
            {
                return Loc.Get("ModSourceLastBuilt.Day", timeAgo.Days);
            }
            else
            {
                return Loc.Get("ModSourceLastBuilt.Days", timeAgo.Days);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            // Show tooltip with the full date and time when hovering over the text
            if (IsMouseHovering)
            {
                TimeSpan timeAgo = DateTime.Now - lastModified;

                string tooltipText = FormatTimeAgoText(lastModified);
                UICommon.TooltipMouseText(Loc.Get("ModSourceLastBuilt.LastBuilt") + ": " + tooltipText);
            }
        }
    }
}