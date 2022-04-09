using Terraria;

namespace TerraBend.Content.StatusEffects.Buffs {
    public class PositiveJingPurgeBuff : BaseStatusEffect {
        public override void SetStaticDefaults() {
            Main.buffNoSave[Type] = true;
        }
    }
}