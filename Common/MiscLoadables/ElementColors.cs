using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TerraBend.Custom.Enums;
using Terraria.ModLoader;

namespace TerraBend.Common.MiscLoadables {
    /// <summary>
    /// Small loadable class that initializes color data for the elements.
    /// </summary>
    public class ElementColors : ILoadable {
        /// <summary>
        /// Dictionary of colors for each element.
        /// </summary>
        public static Dictionary<BendingType, Color> colors;

        public void Load(Mod mod) {
            colors = new Dictionary<BendingType, Color>() {
                { BendingType.Fire, Color.Red },
                { BendingType.Air, Color.Gray },
                { BendingType.Water, Color.DeepSkyBlue },
                { BendingType.Earth, Color.Green }
            };
        }

        public void Unload() {
            colors = null;
        }
    }
}