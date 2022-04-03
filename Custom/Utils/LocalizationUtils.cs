using Terraria.Localization;

namespace TerraBend.Custom.Utils {
    public static class LocalizationUtils {
        /// <summary>
        /// Shortcut for getting TerraBend localization strings, where the "key" is the
        /// direction within the Mods.TerraBend.
        /// </summary>
        /// <param name="key">
        /// The key for the specified localization string starting at the TerraBend base directory.
        /// </param>
        public static string GetModTextValue(string key, params object[] args) => Language.GetTextValue("Mods.TerraBend." + key, args);
    }
}