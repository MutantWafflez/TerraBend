using System;
using System.Collections.Generic;
using System.Linq;
using TerraBend.Common.MiscLoadables;
using TerraBend.Custom.Enums;
using TerraBend.Custom.Utils;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace TerraBend.Common.Players {
    /// <summary>
    /// Player class that handles the player-sided bending stance activation.
    /// </summary>
    public class BendingStancePlayer : ModPlayer {
        /// <summary>
        /// The type of element this player is currently in the stance of.
        /// If null, the player is currently not in a bending stance at all.
        /// </summary>
        public BendingType? currentElementStance;

        /// <summary>
        /// The element that this player chose when first creating this player.
        /// The player cannot use any other element other than this alignment
        /// if they are not an avatar, but if they are an avatar, they can unlock the other
        /// elements. Null if the player has yet to choose an element (freshly created).
        /// </summary>
        public BendingType? elementAlignment;

        /// <summary>
        /// Whether or not this player is an Avatar. 
        /// </summary>
        public bool isAnAvatar;

        /// <summary>
        /// Small dictionary for every defined element that denotes whether
        /// or not said element has been unlocked. Has no use for players
        /// who are not an avatar. Avatar players, upon creation, have only their
        /// original elemental alignment unlocked.
        /// </summary>
        public Dictionary<BendingType, bool> unlockedElements;

        /// <summary>
        /// Hotbars for each of the elements.
        /// </summary>
        public Dictionary<BendingType, Item[]> elementHotbars;

        /// <summary>
        /// List of learned moves for each of the elements.
        /// </summary>
        public Dictionary<BendingType, List<Item>> learnedMoves;

        public override void ProcessTriggers(TriggersSet triggersSet) {
            foreach (KeyValuePair<BendingType, ModKeybind> elementPair in ElementKeybinds.keybinds) {
                if (!elementPair.Value.JustPressed) {
                    continue;
                }

                //Prevent stance swapping if an item is currently being held that isn't on the hotbar.
                if (!Main.mouseItem.IsAir) {
                    Main.NewText(LocalizationUtils.GetModTextValue("InfoText.EnteringBendingStanceWithItem"), 185, 0, 0);
                    break;
                }
                //Prevent stance swapping if the player is not an avatar and is trying to swap to an element that they aren't aligned with
                else if (!isAnAvatar && elementPair.Key != elementAlignment) {
                    Main.NewText(LocalizationUtils.GetModTextValue("InfoText.TryingToEnterNonAlignedElement"), 185, 0, 0);
                    break;
                }
                //Prevent stance swapping to a locked element for the avatar
                else if (isAnAvatar && !unlockedElements[elementPair.Key]) {
                    Main.NewText(LocalizationUtils.GetModTextValue("InfoText.TryingToEnterLockedElement", LocalizationUtils.GetModTextValue($"ElementNames.{elementPair.Key}")), ElementColors.colors[elementPair.Key]);
                    break;
                }

                if (currentElementStance == elementPair.Key) {
                    currentElementStance = null;
                }
                else {
                    currentElementStance = elementPair.Key;
                }

                break;
            }
        }

        public override void Initialize() {
            currentElementStance = null;
            elementAlignment = null;
            isAnAvatar = false;

            unlockedElements = new Dictionary<BendingType, bool>();
            foreach (BendingType bendingType in Enum.GetValues(typeof(BendingType))) {
                unlockedElements[bendingType] = false;
            }

            elementHotbars = new Dictionary<BendingType, Item[]>();
            foreach (BendingType bendingType in Enum.GetValues(typeof(BendingType))) {
                Item[] emptyHotbar = new Item[10];
                Array.Fill(emptyHotbar, new Item());
                elementHotbars[bendingType] = emptyHotbar;
            }

            learnedMoves = new Dictionary<BendingType, List<Item>>();
            foreach (BendingType bendingType in Enum.GetValues(typeof(BendingType))) {
                learnedMoves[bendingType] = new List<Item>();
            }
        }

        public override void SaveData(TagCompound tag) {
            tag["isAvatar"] = isAnAvatar;
            tag["elementAlignment"] = elementAlignment is not null ? (int)elementAlignment : -1;

            TagCompound unlockedElementCompound = new TagCompound();
            foreach (BendingType bendingType in Enum.GetValues(typeof(BendingType))) {
                unlockedElementCompound[bendingType.ToString()] = unlockedElements[bendingType];
            }
            tag["unlockedElements"] = unlockedElementCompound;

            TagCompound elementHotbarCompound = new TagCompound();
            foreach (BendingType bendingType in Enum.GetValues(typeof(BendingType))) {
                elementHotbarCompound[bendingType.ToString()] = elementHotbars[bendingType].ToList();
            }
            tag["elementHotbars"] = elementHotbarCompound;

            TagCompound learnedMovesCompounds = new TagCompound();
            foreach (BendingType bendingType in Enum.GetValues(typeof(BendingType))) {
                learnedMovesCompounds[bendingType.ToString()] = learnedMoves[bendingType];
            }
            tag["learnedMoves"] = learnedMovesCompounds;
        }

        public override void LoadData(TagCompound tag) {
            if (tag.ContainsKey("isAvatar")) {
                isAnAvatar = tag.GetBool("isAvatar");
            }

            if (tag.ContainsKey("elementAlignment")) {
                elementAlignment = tag.GetInt("elementAlignment") is int value and >= 0 ? (BendingType?)value : null;
            }

            if (tag.ContainsKey("unlockedElements")) {
                TagCompound innerCompound = tag.GetCompound("unlockedElements");

                foreach (BendingType bendingType in Enum.GetValues(typeof(BendingType))) {
                    unlockedElements[bendingType] = innerCompound.GetBool(bendingType.ToString());
                }
            }

            if (tag.ContainsKey("elementHotbars")) {
                TagCompound innerCompound = tag.GetCompound("elementHotbars");

                foreach (BendingType bendingType in Enum.GetValues(typeof(BendingType))) {
                    elementHotbars[bendingType] = innerCompound.Get<List<Item>>(bendingType.ToString()).ToArray();
                }
            }

            if (tag.ContainsKey("learnedMoves")) {
                TagCompound innerCompound = tag.GetCompound("learnedMoves");

                foreach (BendingType bendingType in Enum.GetValues(typeof(BendingType))) {
                    learnedMoves[bendingType] = innerCompound.Get<List<Item>>(bendingType.ToString());
                }
            }

            //In case some kind of loading shenanigans happen, the player's aligned element is forcefully unlocked.
            if (elementAlignment is not null) {
                unlockedElements[elementAlignment.Value] = true;
            }
        }

        /// <summary>
        /// Unlocks the next element in the Avatar Cycle, depending on what elements have already been unlocked.
        /// For example, if an avatar player has only Water unlocked, this method will unlock Earth, then Fire, then finally Air.
        /// </summary>
        public void UnlockNextElementInCycle() {
            //Simply end the method if the player has unlocked everything or is not an avatar
            if (!isAnAvatar || unlockedElements.All(pair => pair.Value)) {
                return;
            }

            //Get first element that is currently false (locked), which is preceeded by an element that is true (unlocked)
            BendingType nextElement = unlockedElements.OrderBy(pair => (int)pair.Key).FirstOrDefault(pair => unlockedElements[pair.Key.PreviousEnum()] && !pair.Value).Key;
            unlockedElements[nextElement] = true;

            Main.NewText(LocalizationUtils.GetModTextValue("InfoText.UnlockedNewElement", LocalizationUtils.GetModTextValue($"ElementNames.{nextElement}")), ElementColors.colors[nextElement]);
        }
    }
}