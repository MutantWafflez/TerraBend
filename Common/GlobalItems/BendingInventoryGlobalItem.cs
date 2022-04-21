using System.Collections.Generic;
using TerraBend.Common.Patches;
using TerraBend.Content.BendingMoves;
using TerraBend.Custom.Utils;
using Terraria;
using Terraria.ModLoader;

namespace TerraBend.Common.GlobalItems {
    /// <summary>
    /// Global Item that is shared for items within the bending inventory. For the time being, it's primarily used for
    /// tooltips.
    /// </summary>
    public class BendingInventoryGlobalItem : GlobalItem {
        /// <summary>
        /// Whether or not to show the hotbar tip on this given item's tooltip at any given moment in time. See 
        /// <seealso cref="InventoryPatches"/> for how it is set.
        /// </summary>
        public bool showHotbarTip;

        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.ModItem is BendingMove;

        public override GlobalItem Clone(Item item, Item itemClone) {
            BendingInventoryGlobalItem defaultClone = (BendingInventoryGlobalItem)base.Clone(item, itemClone);
            defaultClone.showHotbarTip = showHotbarTip;
            showHotbarTip = false;

            return defaultClone;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) {
            // This line is here to show the player how to properly assign this weapon to the hotbar, if they otherwise don't know how
            TooltipLine hotbarAssignmentLine = new TooltipLine(Mod, "HotbarAssignmentLine", LocalizationUtils.GetModTextValue("MiscTooltips.HotbarAssignmentTip")) {
                isModifier = true
            };

            if (showHotbarTip) {
                tooltips.Add(hotbarAssignmentLine);
            }
        }
    }
}