using System.Collections.Generic;
using System.Linq;
using TerraBend.Content.DamageClasses;
using TerraBend.Custom.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TerraBend.Content.BendingMoves.FireMoves {
    /// <summary>
    /// Mastery Firebender move. Generates an insta-kill lightning bolt that can kill the bender if their
    /// Jing is not within balance.
    /// </summary>
    public class LightningMove : BendingMove {
        public override string Texture => "Terraria/Images/Item_" + ItemID.LightningBug;

        public override BendingType BendingMoveType => BendingType.Fire;

        public override void SetDefaults() {
            Item.DamageType = ModContent.GetInstance<FireDamageClass>();
            Item.rare = ItemRarityID.Master;
            Item.damage = 1; //Since we are dealing with an instakill item, damage number doesn't actually matter
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) {
            //Keeping base, cause that auto-handling is nice
            base.ModifyTooltips(tooltips);

            //Edit damage line to show an infinity sign for damage, for thematic purposes
            TooltipLine vanillaDamageLine = tooltips.FirstOrDefault(tooltip => tooltip.mod == "Terraria" && tooltip.Name == "Damage");

            if (vanillaDamageLine is not null) {
                vanillaDamageLine.text = $"\u221E {Item.DamageType.ClassName.GetTranslation(Language.ActiveCulture)}";
            }
        }
    }
}