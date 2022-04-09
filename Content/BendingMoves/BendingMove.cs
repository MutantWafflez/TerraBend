using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TerraBend.Common.MiscLoadables;
using TerraBend.Common.Players;
using TerraBend.Content.DamageClasses;
using TerraBend.Custom.Enums;
using TerraBend.Custom.Utils;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace TerraBend.Content.BendingMoves {
    /// <summary>
    /// "Item" class used for all of the different types of Bending Moves. 
    /// </summary>
    public abstract class BendingMove : ModItem {
        /// <summary>
        /// The TYPE of Jing to create. If you do not want this move to create
        /// any Jing, set this to JingType.Unaligned.
        /// </summary>
        public JingType jingCreationType;

        /// <summary>
        /// The AMOUNT of Jing to create of the specified type in the <seealso cref="jingCreationType"/>
        /// field. Does nothing if a valid type is not inputted.
        /// </summary>
        public int jingCreationAmount;

        /// <summary>
        /// This move's bending type. Must be denoted.
        /// </summary>
        public abstract BendingType BendingMoveType {
            get;
        }

        //Not allowed to use bending moves if:
        //1. Somehow the item is within the normal inventory
        //2. Current chi is <= displayedMaxChi / 2
        public override bool CanUseItem(Player player) {
            ChiPlayer chiPlayer = player.GetModPlayer<ChiPlayer>();

            return !player.inventory.Contains(Item) || chiPlayer.currentChi <= chiPlayer.displayedMaxChi / 2;
        }

        public override bool? UseItem(Player player) {
            //Handle actual Jing creation
            JingPlayer jingPlayer = player.GetModPlayer<JingPlayer>();
            switch (jingCreationType) {
                case JingType.Positive:
                    jingPlayer.ChangePositiveJing(jingCreationAmount);
                    break;
                case JingType.Neutral:
                    jingPlayer.ChangeNeutralJing(jingCreationAmount);
                    break;
                case JingType.Negative:
                    jingPlayer.ChangeNegativeJing(jingCreationAmount);
                    break;
            }

            return null;
        }

        public override bool CanRightClick() => TerraBend.IsDebug;

        public override void RightClick(Player player) {
            //Add item to each applicable inventory when right clicked in a default inventory
            player.GetModPlayer<BendingStancePlayer>().learnedMoves[BendingMoveType].Add(Item.Clone());
        }

        public override void AutoDefaults() {
            base.AutoDefaults();
            Item.DamageType = ModContent.GetInstance<BendingDamageClass>();
            Item.crit = -100;
            jingCreationType = JingType.Unaligned;
            jingCreationAmount = 0;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage, ref float flat) {
            ChiPlayer chiPlayer = player.GetModPlayer<ChiPlayer>();

            //For every % below 70% of the displayed max chi value, lose 2% damage
            damage -= MathHelper.Clamp(0.02f * ((1f - chiPlayer.currentChi / (float)chiPlayer.displayedMaxChi) * 100f - 30f), 0f, 1f);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) {
            //Remove the critical strike % line from vanilla (since bending moves by default cannot crit)
            TooltipLine vanillaCritLine = tooltips.FirstOrDefault(tooltip => tooltip.mod == "Terraria" && tooltip.Name == "CritChance");
            if (vanillaCritLine is not null) {
                tooltips.Remove(vanillaCritLine);
            }

            //Add Jing Creation tooltip, if applicable
            TooltipLine vanillaDamageLine = tooltips.FirstOrDefault(tooltip => tooltip.mod == "Terraria" && tooltip.Name == "Damage");
            if (jingCreationType < JingType.Unaligned && jingCreationAmount > 0) {
                TooltipLine jingCreationLine = new TooltipLine(Mod,
                    "JingCreation",
                    LocalizationUtils.GetModTextValue("MiscTooltips.JingCreationTip", jingCreationAmount, LocalizationUtils.GetModTextValue($"FullJingName.{jingCreationType}"))
                );

                if (vanillaDamageLine is not null) {
                    tooltips.Insert(tooltips.IndexOf(vanillaDamageLine) + 1, jingCreationLine);
                }
                else {
                    tooltips.Add(jingCreationLine);
                }
            }

            //Add Item Lore, if applicable
            string itemLoreKey = $"Mods.TerraBend.ItemLore.{Name}";
            if (Language.GetTextValue(itemLoreKey) is { } returnedValue && returnedValue != itemLoreKey) {
                //If the player is not holding shift, then simply show a tooltip saying to do so if they wanna see lore
                bool pressingShift = Keyboard.GetState().PressingShift();

                TooltipLine lineToAdd = new TooltipLine(Mod, pressingShift ? "MoveLoreTip" : "OpenLoreTip", pressingShift ? returnedValue : LocalizationUtils.GetModTextValue("MiscTooltips.OpenLoreTip")) {
                    overrideColor = pressingShift ? null : Color.Gray,
                    isModifier = pressingShift,
                    isModifierBad = pressingShift
                };

                tooltips.Add(lineToAdd);
            }
        }
    }
}