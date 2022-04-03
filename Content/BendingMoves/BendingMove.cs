using System.Collections.Generic;
using System.Linq;
using TerraBend.Common.MiscLoadables;
using TerraBend.Common.Players;
using TerraBend.Content.DamageClasses;
using TerraBend.Custom.Enums;
using Terraria;
using Terraria.ModLoader;

namespace TerraBend.Content.BendingMoves {
    /// <summary>
    /// "Item" class used for all of the different types of Bending Moves. 
    /// </summary>
    public abstract class BendingMove : ModItem {
        /// <summary>
        /// This move's bending type. Must be denoted.
        /// </summary>
        public abstract BendingType BendingMoveType {
            get;
        }

        //Not allowed to use a Bending Move unless it is in an element inventory, or in debug mode
        public override bool CanUseItem(Player player) => !player.inventory.Contains(Item);

        public override void AutoDefaults() {
            base.AutoDefaults();
            Item.DamageType = ModContent.GetInstance<BendingDamageClass>();
            Item.crit = 0;
        }

        public override bool CanRightClick() => TerraBend.IsDebug;

        public override void RightClick(Player player) {
            //Add item to each applicable inventory when right clicked in a default inventory
            player.GetModPlayer<BendingStancePlayer>().learnedMoves[BendingMoveType].Add(Item.Clone());
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) {
            //Paint damage line to be the color of the element
            TooltipLine vanillaDamageLine = tooltips.FirstOrDefault(tooltip => tooltip.mod == "Terraria" && tooltip.Name == "Damage");

            if (vanillaDamageLine is not null) {
                vanillaDamageLine.overrideColor = ElementColors.colors[BendingMoveType];
            }
            //Remove the critical strike % line from vanilla (since bending moves by default cannot crit)
            TooltipLine vanillaCritLine = tooltips.FirstOrDefault(tooltip => tooltip.mod == "Terraria" && tooltip.Name == "CritChance");
            if (vanillaCritLine is not null) {
                tooltips.Remove(vanillaCritLine);
            }
        }
    }
}