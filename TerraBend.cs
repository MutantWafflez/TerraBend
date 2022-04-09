using Terraria.ModLoader;

namespace TerraBend {
    public class TerraBend : Mod {
        /// <summary>
        /// Whether or not the mod is in Debug, which is determined by if you are building from some
        /// IDE as Debug.
        /// </summary>
        public static bool IsDebug {
            get {
                #if DEBUG
                return true;
                #else
                return false;
                #endif
            }
        }

        /// <summary>
        /// Directory of the Sprites for TerraBend.
        /// </summary>
        public static string SpritePath => nameof(TerraBend) + "/Assets/Sprites/";
    }
}