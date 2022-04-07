using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace TerraBend.Common.Players {
    /// <summary>
    /// ModPlayer that exclusively handles the "Chi" resource, which essentially determines the threshold
    /// by which the player can attack; i.e, certain moves require that your "Chi" be high enough.
    /// </summary>
    /// <remarks>
    /// "Chi", as explained by the ATLA universe, is a "metaphysical energy that flows throughout the human body... [which] serves as the root
    /// of bending and an array of other skills." (from the ATLA wiki)
    /// <para></para>
    /// In (technical) detail, Chi isn't quite like Mana for mages: there are some distinct differences. Each of the three "parts" of Chi have unique
    /// purposes. The player's current Chi value (<seealso cref="currentChi"/>) under normal circumstances is going to be whatever the Max Chi
    /// value is. However, the current Chi value, unlike Mana, will not be decreased by using normal bending moves; it can only be decreased by
    /// certain (enemy) attacks or one-time circumstances. The Current chi value regenerates VERY slowly, and has no "instant regen" method like
    /// mana/health potions. As for the Base Max Chi value (<seealso cref="baseMaxChi"/>) is the simplest of the three "parts", it's the saved,
    /// base max value of Chi. If the player's current Chi value falls below 50% of the base Max Chi, the player will be unable to use ANY bending
    /// moves until that threshold is passed again. Finally, the last "part" is the DISPLAYED max value (<seealso cref="displayedMaxChi"/>) of Chi;
    /// this value is reset to the base max Chi every tick, the current Chi value cannot pass this value. Some bending moves, as a side effect, will
    /// decrease this value (for example, summoning a persistent ball of flame). If the current Chi value falls below 70% of the DISPLAYED max chi
    /// value, then bending moves will lose 2% damage (additive) for each % below that threshold. For example, if the current Chi value is at 50% of
    /// the DISPLAYED max value, then bending moves will lose 40% damage.
    /// </remarks>
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

        public override void OnRespawn(Player player) {
            currentChi = displayedMaxChi;
        }

        public override void Initialize() {
            currentChi = baseMaxChi = displayedMaxChi = DefaultMaxChi;
        }

        public override void SaveData(TagCompound tag) {
            tag["maxChi"] = baseMaxChi;
        }

        public override void LoadData(TagCompound tag) {
            displayedMaxChi = baseMaxChi = tag.ContainsKey("maxChi") ? tag.GetInt("maxChi") : DefaultMaxChi;
            currentChi = displayedMaxChi;
        }
    }
}