using Terraria.Localization;

namespace ModReloader.Helpers
{
    public static class LocalizationHelper
    {
        /// <summary>
        /// Gets the text for the given key from the ModReloader localization file.
        /// If no localization is found, the key itself is returned.
        /// Reference:
        /// https://github.com/ScalarVector1/DragonLens/blob/master/Helpers/LocalizationHelper.cs
        /// </summary>
        public static string GetText(string key, params object[] args)
        {
            return Language.Exists($"Mods.ModReloader.{key}") ? Language.GetTextValue($"Mods.ModReloader.{key}", args) : key;
        }
    }
}
