using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using ReLogic.Content;
using ReLogic.Graphics;
using TerraBend.Common.GlobalItems;
using TerraBend.Common.MiscLoadables;
using TerraBend.Common.Players;
using TerraBend.Content.BendingMoves;
using Terraria.ModLoader;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.UI;
using Terraria.UI.Chat;

namespace TerraBend.Common.Patches {
    /// <summary>
    /// Patches Class that handles any patches to Vanilla's inventory UI.
    /// </summary>
    public class InventoryPatches : ILoadable {
        private Asset<Texture2D> _elementBackgroundAsset;

        public void Load(Mod mod) {
            IL.Terraria.Main.GUIHotbarDrawInner += DrawElementHotbarPatch;
            IL.Terraria.Main.DrawInventory += DrawElementInventoryPatch;
            IL.Terraria.Player.ItemCheck_Inner += UseBendingMovePatch;
            _elementBackgroundAsset = ModContent.Request<Texture2D>("TerraBend/Assets/Sprites/UI/Element_Inventory_Back");
        }

        public void Unload() { }

        /// <summary>
        /// Adapted vanilla code that draws very closely to how ItemSlot normally draws, but with control over color amongst other small optimizations.
        /// </summary>
        private void DrawElementItemSlot(SpriteBatch spriteBatch, Item[] elementInventory, int inventoryContext, int itemIndex, Vector2 drawPos, Color elementColor, float inventoryScale) {
            //Draw background
            spriteBatch.Draw(_elementBackgroundAsset.Value, drawPos, null, elementColor, 0f, default, inventoryScale, SpriteEffects.None, 0f);

            //Draw item
            Item slotItem = elementInventory[itemIndex];
            if (slotItem.type > ItemID.None) {
                Main.instance.LoadItem(slotItem.type);
                Texture2D itemTexture = TextureAssets.Item[slotItem.type].Value;
                Rectangle framingRectangle = Main.itemAnimations[slotItem.type] is null ? itemTexture.Frame() : Main.itemAnimations[slotItem.type].GetFrame(itemTexture);
                Color currentColor = Color.White;
                Color itemColor = slotItem.GetColor(currentColor);
                Color itemAlpha = slotItem.GetAlpha(currentColor);
                float originalScale = 1f;
                ItemSlot.GetItemLight(ref currentColor, ref originalScale, slotItem);
                float constrainedScale = 1f;

                if (framingRectangle.Width > 32 || framingRectangle.Height > 32) {
                    constrainedScale = framingRectangle.Width <= framingRectangle.Height ? 32f / framingRectangle.Height : 32f / framingRectangle.Width;
                }
                float finalScale = constrainedScale * inventoryScale;
                Vector2 itemDrawPos = drawPos + _elementBackgroundAsset.Size() * inventoryScale / 2f - framingRectangle.Size() * finalScale / 2f;
                Vector2 origin = framingRectangle.Size() * (originalScale / 2f - 0.5f);

                if (ItemLoader.PreDrawInInventory(slotItem, Main.spriteBatch, itemDrawPos, framingRectangle, itemAlpha, itemColor, origin, finalScale * originalScale)) {
                    spriteBatch.Draw(itemTexture, itemDrawPos, framingRectangle, itemAlpha, 0f, origin, finalScale * originalScale, SpriteEffects.None, 0f);

                    if (slotItem.stack > 1) {
                        ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, slotItem.stack.ToString(), drawPos + new Vector2(10f, 26f) * inventoryScale, itemAlpha, 0f, default, new Vector2(inventoryScale),
                            spread: inventoryScale);
                    }
                }
                ItemLoader.PostDrawInInventory(slotItem, Main.spriteBatch, itemDrawPos, framingRectangle, itemAlpha, itemColor, origin, finalScale * originalScale);
            }

            //Draw item number in top left corner if applicable to context
            if (inventoryContext is ItemSlot.Context.HotbarItem && slotItem.type > ItemID.None || inventoryContext is ItemSlot.Context.InventoryItem && itemIndex < 10) {
                ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, FontAssets.ItemStack.Value, itemIndex + 1 != 10 ? (itemIndex + 1).ToString() : "0", drawPos + new Vector2(8f, 4f) * inventoryScale, Color.White, 0f, Vector2.Zero,
                    new Vector2(inventoryScale), spread: inventoryScale);
            }
        }

        private void DrawElementHotbar() {
            Player locPlayer = Main.LocalPlayer;
            BendingStancePlayer locBendingPlayer = locPlayer.GetModPlayer<BendingStancePlayer>();
            Color elementColor = ElementColors.colors[locBendingPlayer.currentElementStance.Value];
            Item[] elementHotbar = locBendingPlayer.elementHotbars[locBendingPlayer.currentElementStance.Value];

            //Adapted Vanilla Code
            int xPos = 20;
            for (int i = 0; i < 10; i++) {
                if (i == locPlayer.selectedItem) {
                    if (Main.hotbarScale[i] < 1f) {
                        Main.hotbarScale[i] += 0.05f;
                    }
                }
                else if (Main.hotbarScale[i] > 0.75) {
                    Main.hotbarScale[i] -= 0.05f;
                }

                float inventoryScale = Main.hotbarScale[i];
                int yPos = (int)(20f + 22f * (1f - inventoryScale));
                elementColor.A = (byte)(75f + 150f * inventoryScale);

                if (!locPlayer.hbLocked && !PlayerInput.IgnoreMouseInterface && Main.mouseX >= xPos && Main.mouseX <= xPos + _elementBackgroundAsset.Width() * inventoryScale &&
                    Main.mouseY >= yPos && Main.mouseY <= yPos + _elementBackgroundAsset.Height() * inventoryScale && !locPlayer.channel) {
                    locPlayer.mouseInterface = true;
                    locPlayer.cursorItemIconEnabled = false;
                    if (Main.mouseLeft && !locPlayer.hbLocked && !Main.blockMouse) {
                        locPlayer.changeItem = i;
                    }
                    Main.rare = ItemRarityID.White;
                }
                //Draw background & item
                DrawElementItemSlot(Main.spriteBatch, elementHotbar, ItemSlot.Context.HotbarItem, i, new Vector2(xPos, yPos), i != locPlayer.selectedItem ? elementColor : Color.Lerp(elementColor, Color.Yellow, 0.75f), inventoryScale);

                //Draw item name (up top)
                string itemNameText = string.IsNullOrEmpty(elementHotbar[locPlayer.selectedItem].Name) ? Lang.inter[37].Value : elementHotbar[locPlayer.selectedItem].AffixName();
                Main.spriteBatch.DrawString(FontAssets.MouseText.Value, itemNameText, new Vector2(236f - (FontAssets.MouseText.Value.MeasureString(itemNameText) / 2f).X, 0f),
                    new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor), 0f, default, 1f, SpriteEffects.None, 0f);

                //Update position
                xPos += (int)(_elementBackgroundAsset.Width() * inventoryScale) + 4;
            }
        }

        private void DrawElementInventory() {
            Player locPlayer = Main.LocalPlayer;
            BendingStancePlayer locBendingPlayer = locPlayer.GetModPlayer<BendingStancePlayer>();
            Color elementColor = ElementColors.colors[locBendingPlayer.currentElementStance.Value];
            Item[] elementHotbar = locBendingPlayer.elementHotbars[locBendingPlayer.currentElementStance.Value];
            Item[] elementInventory = elementHotbar.Concat(locBendingPlayer.learnedMoves[locBendingPlayer.currentElementStance.Value]).ToArray();

            //Adapted Vanilla Code
            //With how the IL is formatted, we have to manually do this check if the player is within a bending stance, as otherwise it will be skipped over.
            if (Main.mouseX > 20 && Main.mouseX < (int)(20f + 560f * Main.inventoryScale) && Main.mouseY > 20 && Main.mouseY < (int)(20f + 280f * Main.inventoryScale) && !PlayerInput.IgnoreMouseInterface) {
                locPlayer.mouseInterface = true;
            }

            for (int i = 0; i < Math.Min(elementInventory.Length, Main.InventoryItemSlotsCount); i++) {
                int columnNum = i % 10;
                int rowNum = (int)Math.Floor(i / 10f);
                int xPos = (int)(20f + columnNum * 56f * Main.inventoryScale);
                int yPos = (int)(20f + rowNum * 56f * Main.inventoryScale);
                int itemSlot = columnNum + rowNum * 10;

                if (Main.mouseX >= xPos && Main.mouseX <= xPos + _elementBackgroundAsset.Width() * Main.inventoryScale && Main.mouseY >= yPos && Main.mouseY <= yPos + _elementBackgroundAsset.Height() * Main.inventoryScale &&
                    !PlayerInput.IgnoreMouseInterface) {
                    locPlayer.mouseInterface = true;
                    ItemSlot.OverrideHover(elementInventory, ItemSlot.Context.InventoryItem, itemSlot);
                    if (locPlayer.inventoryChestStack[itemSlot] && (elementInventory[itemSlot].type == ItemID.None || elementInventory[itemSlot].stack == 0)) {
                        locPlayer.inventoryChestStack[itemSlot] = false;
                    }

                    bool itemNotInHotbar = elementInventory.Take(10).All(invItem => invItem.type != elementInventory[itemSlot].type);
                    if (itemSlot >= 10
                        && itemNotInHotbar
                        && PlayerInput.Triggers.Current.KeyStatus.FirstOrDefault(keyPair => keyPair.Key.StartsWith("Hotbar") && keyPair.Key.Any(char.IsDigit) && keyPair.Value).Key is { } key) {
                        //Move item to selected hotbar slot when hovering over it and pressing a hotbar keybind
                        int hotbarValue = int.Parse(string.Join(null, key.Where(char.IsDigit))) - 1;
                        elementHotbar[hotbarValue] = elementInventory[itemSlot].Clone();
                    }
                    if (Main.mouseRightRelease && Main.mouseRight && itemSlot < 10) {
                        //If right clicking and on the hotbar, delete the item on that slot
                        elementHotbar[itemSlot].TurnToAir();
                    }

                    ItemSlot.MouseHover(elementInventory, ItemSlot.Context.InventoryItem, itemSlot);
                    //If hovering over the item, and the item is not in the hotbar already, show hotbar assignment tooltip
                    if (elementInventory[itemSlot].ModItem is BendingMove && itemSlot >= 10 && itemNotInHotbar) {
                        elementInventory[itemSlot].GetGlobalItem<BendingInventoryGlobalItem>().showHotbarTip = true;
                    }
                }

                DrawElementItemSlot(Main.spriteBatch, elementInventory, ItemSlot.Context.InventoryItem, itemSlot, new Vector2(xPos, yPos), itemSlot != locPlayer.selectedItem ? elementColor : Color.Lerp(elementColor, Color.Yellow, 0.75f),
                    Main.inventoryScale);
            }
        }

        private void DrawElementHotbarPatch(ILContext il) {
            //For this edit, depending on whether or not the local player is within the bending stance or not, we are going to draw the bending hotbar.

            ILCursor c = new ILCursor(il);

            //Jump to exit instruction at the end of the method and grab a label for it. If player is in the bending stance, skip over normal vanilla drawing and only do our own
            c.Index = c.Instrs.Count - 1;
            ILLabel exitLabel = c.MarkLabel();
            c.Index = 0;
            c.EmitDelegate<Func<bool>>(() => {
                bool isInBendingStance = Main.LocalPlayer.GetModPlayer<BendingStancePlayer>().currentElementStance is not null && !Main.playerInventory;
                if (isInBendingStance) {
                    DrawElementHotbar();
                }

                return isInBendingStance;
            });
            c.Emit(OpCodes.Brtrue_S, exitLabel);
        }

        private void DrawElementInventoryPatch(ILContext il) {
            //For this edit, like the hotbar edit, we check if the local player is within a given bending stance, and draw that element's inventory instead of the vanilla one.

            ILCursor c = new ILCursor(il);

            //Jump to right after the loop that draws the inventory and grab a label. If the player's inventory is open and they are in a bending stance, skip to this label and thus
            //skip vanilla inventory drawing.
            ILLabel postLoopLabel = null;
            if (c.TryGotoNext(i => i.MatchCall<Main>("get_LocalPlayer"))) {
                postLoopLabel = c.MarkLabel();
            }

            c.Index = 0;

            //Next, jump to right before the inventory draw loop, and add our own check
            if (postLoopLabel is not null && c.TryGotoNext(MoveType.After, i => i.MatchStsfld<Main>(nameof(Main.inventoryScale)))) {
                c.EmitDelegate<Func<bool>>(() => {
                    bool isInBendingStance = Main.LocalPlayer.GetModPlayer<BendingStancePlayer>().currentElementStance is not null && Main.playerInventory;
                    if (isInBendingStance) {
                        DrawElementInventory();
                    }

                    return isInBendingStance;
                });
                c.Emit(OpCodes.Brtrue_S, postLoopLabel);
            }
        }

        private void UseBendingMovePatch(ILContext il) {
            //This edit is just us swapping out what is the denoted "selected item" is if the player is currently in a bending stance.

            ILCursor c = new ILCursor(il);

            //Move to stack allocation of the local variable for the "selected item" and do our bending stance check
            if (c.TryGotoNext(i => i.MatchStloc(2))) {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<Item, Player, Item>>((originalItem, player) => {
                    BendingStancePlayer bendingStancePlayer = player.GetModPlayer<BendingStancePlayer>();
                    if (bendingStancePlayer.currentElementStance is { } stanceType) {
                        return bendingStancePlayer.elementHotbars[stanceType][player.selectedItem];
                    }

                    return originalItem;
                });
            }

            //Jump to end of method where a "deletion" method takes place, which we must properly swap inventories so that removing an item
            //from an element's inventory won't delete it from the main inventory
            c.Index = c.Instrs.Count - 1;
            if (c.TryGotoPrev(i => i.MatchLdfld<Player>(nameof(Player.inventory)))) {
                c.Remove();
                c.EmitDelegate<Func<Player, Item[]>>(player => {
                    BendingStancePlayer bendingStancePlayer = player.GetModPlayer<BendingStancePlayer>();
                    if (bendingStancePlayer.currentElementStance is { } stanceType) {
                        return bendingStancePlayer.elementHotbars[stanceType].Concat(bendingStancePlayer.learnedMoves[stanceType]).ToArray();
                    }

                    return player.inventory;
                });
            }
        }
    }
}