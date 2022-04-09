using Terraria;

namespace TerraBend.Content.StatusEffects.Buffs {
    public class NegativeJingPurgeBuff : BaseStatusEffect {
        public override void SetStaticDefaults() {
            Main.buffNoSave[Type] = true;
        }
    }
}