using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework.Input;
using TerraBend.Custom.Enums;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TerraBend.Common.MiscLoadables {
    /// <summary>
    /// Small loadable class that holds data for the keybinds of each of the elements.
    /// </summary>
    public class ElementKeybinds : ILoadable {
        public static Dictionary<BendingType, ModKeybind> keybinds;

        public void Load(Mod mod) {
            //First, translation shenanigans, since they can't be manually retrieved during this time, and the method that adds the translations is private, so reflection is required
            MethodInfo getOrCreateTranslationMethodInfo = typeof(LocalizationLoader).GetMethod("GetOrCreateTranslation", BindingFlags.Static | BindingFlags.NonPublic, new Type[] { typeof(Mod), typeof(string), typeof(bool) });

            Dictionary<BendingType, ModTranslation> translatedKeybindInfo = new Dictionary<BendingType, ModTranslation>();
            foreach (BendingType bendingType in Enum.GetValues(typeof(BendingType))) {
                translatedKeybindInfo[bendingType] = (ModTranslation)getOrCreateTranslationMethodInfo.Invoke(null, new object[] { mod, $"ElementKeybindInfo.{bendingType}", false });
            }

            keybinds = new Dictionary<BendingType, ModKeybind>() {
                { BendingType.Fire, KeybindLoader.RegisterKeybind(mod, translatedKeybindInfo[BendingType.Fire].GetTranslation(Language.ActiveCulture), Keys.Y) },
                { BendingType.Air, KeybindLoader.RegisterKeybind(mod, translatedKeybindInfo[BendingType.Air].GetTranslation(Language.ActiveCulture), Keys.U) },
                { BendingType.Water, KeybindLoader.RegisterKeybind(mod, translatedKeybindInfo[BendingType.Water].GetTranslation(Language.ActiveCulture), Keys.I) },
                { BendingType.Earth, KeybindLoader.RegisterKeybind(mod, translatedKeybindInfo[BendingType.Earth].GetTranslation(Language.ActiveCulture), Keys.O) }
            };
        }

        public void Unload() {
            keybinds = null;
        }
    }
}