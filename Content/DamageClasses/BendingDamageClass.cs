using Terraria;
using Terraria.ModLoader;

namespace TerraBend.Content.DamageClasses {
    /// <summary>
    /// The "base" bending damage class, that is essentially the "generic"
    /// version of bending damage. Should be extended for each of the elements.
    /// </summary>
    public class BendingDamageClass : DamageClass {
        //"General"/"Normal" bending bonuses will apply to element-specific damage classes as well
        protected override float GetBenefitFrom(DamageClass damageClass) => damageClass == Generic || damageClass == this ? 1f : 0f;

        public override void SetDefaultStats(Player player) {
            //Bending moves cannot random crit by default
            player.GetCritChance(this) = 0;
        }
    }
}