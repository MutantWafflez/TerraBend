using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using TerraBend.Common.MiscLoadables;
using TerraBend.Common.Players;
using TerraBend.Content.UI.Elements;
using TerraBend.Custom.Utils;
using Terraria.GameContent.UI.Elements;
using Terraria.IO;
using Terraria.ModLoader;

namespace TerraBend.Common.Patches {
    /// <summary>
    /// Patches class that deals with IL/On edits in the space of the character selection screen.
    /// </summary>
    public class CharacterSelectionPatches : ILoadable {
        public void Load(Mod mod) {
            IL.Terraria.GameContent.UI.Elements.UICharacterListItem.ctor += PostConstructorAdditionPatch;
        }

        private void PostConstructorAdditionPatch(ILContext il) {
            //This patch is relatively simple; we move to the very end of the constructor, and add our own UIElement class that allows for drawing on the Character Selection screen

            ILCursor c = new ILCursor(il);

            //Move to end, then add our UIElement with a delegate
            c.Index = c.Instrs.Count - 1;
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldarg_0);
            c.Emit(OpCodes.Ldfld, typeof(UICharacterListItem).GetField("_playerPanel", BindingFlags.NonPublic | BindingFlags.Instance));
            c.Emit(OpCodes.Ldarg_1);
            c.EmitDelegate<Action<UICharacterListItem, UICharacter, PlayerFileData>>((listItem, playerPanel, currentPlayerData) => {
                //This is all vanilla magic numbers, sorry!
                Vector2 panelDrawPos = new Vector2(playerPanel.Left.Pixels + playerPanel.Width.Pixels + 211f, 29f * 2f);

                BendingStancePlayer bendingPlayer = currentPlayerData.Player.GetModPlayer<BendingStancePlayer>();
                string displayedText = bendingPlayer.elementAlignment is not null
                    ? LocalizationUtils.GetModTextValue($"ElementNames.{bendingPlayer.elementAlignment}") + (bendingPlayer.isAnAvatar ? $" ({LocalizationUtils.GetModTextValue("Common.Avatar")})" : "")
                    : LocalizationUtils.GetModTextValue("Common.UndecidedAlignment");
                Color displayedColor = bendingPlayer.elementAlignment is not null ? ElementColors.colors[bendingPlayer.elementAlignment.Value] : Color.White;

                UICharacterSelectPanel backgroundPanel = new UICharacterSelectPanel(panelDrawPos, 140f);
                backgroundPanel.Append(new UIText(displayedText) {
                    TextColor = displayedColor,
                    HAlign = 0.5f,
                    VAlign = 0.5f
                });
                listItem.Append(backgroundPanel);
            });
        }

        public void Unload() { }
    }
}