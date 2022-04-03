using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace TerraBend.Common.Players {
    /// <summary>
    /// ModPlayer that exclusively handles the "Chi" resource, which essentially determines the threshold
    /// by which the player can attack; i.e, certain moves require that your "Chi" be high enough.
    /// </summary>
    public class ChiPlayer : ModPlayer {
        /// <summary>
        /// The current chi value, out of the max.
        /// </summary>
        public int currentChi;

        /// <summary>
        /// This is the base chi value that is not modified, and is the value
        /// saved to player, similar to Player.statLifeMax or Player.statManaMax.
        /// Should NOT be changed unless its intended to be permanent; change displayedMaxChi
        /// instead.
        /// </summary>
        public int baseMaxChi;

        /// <summary>
        /// This value is reset to baseMaxChi every tick, and is the actual chi value displayed.
        /// This should be used for most things/calculations. Chi equivalent to Player.statLifeMax2 or Player.statManaMax2
        /// </summary>
        public int displayedMaxChi;

        public static readonly int DefaultMaxChi = 50;

        public override void ResetEffects() {
            displayedMaxChi = baseMaxChi;
            currentChi = (int)MathHelper.Clamp(currentChi, 0f, displayedMaxChi);
        }

        public override void Initialize() {
            baseMaxChi = displayedMaxChi = DefaultMaxChi;
        }

        public override void SaveData(TagCompound tag) {
            tag["maxChi"] = baseMaxChi;
        }

        public override void LoadData(TagCompound tag) {
            baseMaxChi = tag.ContainsKey("maxChi") ? tag.GetInt("maxChi") : DefaultMaxChi;
        }
    }
}