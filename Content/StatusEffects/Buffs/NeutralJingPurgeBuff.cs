using Terraria;
using Terraria.ID;

namespace TerraBend.Content.StatusEffects.Buffs {
    public class NeutralJingPurgeBuff : BaseStatusEffect {
        public override void SetStaticDefaults() {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;

            BuffID.Sets.TimeLeftDoesNotDecrease[Type] = true;
        }
    }
}