using System;
using System.Reflection;
using Microsoft.Xna.Framework.Input;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TerraBend.Common.MiscLoadables {
    /// <summary>
    /// Miscellaneous loadable class that holds miscellaneous keybinds without their own space.
    /// </summary>
    public class MiscellaneousKeybinds : ILoadable {
        public static ModKeybind jingPurgeKeybind;

        public void Load(Mod mod) {
            //Like in ElementKeybinds.cs, translations aren't loaded in this point of time, so reflection!
            MethodInfo getOrCreateTranslationMethodInfo = typeof(LocalizationLoader).GetMethod("GetOrCreateTranslation", BindingFlags.Static | BindingFlags.NonPublic, new Type[] { typeof(Mod), typeof(string), typeof(bool) });

            ModTranslation jingPurgeTranslation = (ModTranslation)getOrCreateTranslationMethodInfo.Invoke(null, new object[] { mod, "MiscKeybindInfo.JingPurge", false });

            jingPurgeKeybind = KeybindLoader.RegisterKeybind(mod, jingPurgeTranslation.GetTranslation(Language.ActiveCulture), Keys.N);
        }

        public void Unload() {
            jingPurgeKeybind = null;
        }
    }
}