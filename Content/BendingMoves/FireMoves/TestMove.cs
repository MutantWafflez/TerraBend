using TerraBend.Content.DamageClasses;
using TerraBend.Custom.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace TerraBend.Content.BendingMoves.FireMoves {
    public class TestMove : BendingMove {
        public override string Texture => "Terraria/Images/Item_" + ItemID.Flamelash;

        public override BendingType BendingMoveType => BendingType.Fire;

        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.Flamelash);
            Item.DamageType = ModContent.GetInstance<FireDamageClass>();
            jingCreationType = JingType.Positive;
            jingCreationAmount = 4;
        }
    }
}