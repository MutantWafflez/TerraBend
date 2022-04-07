using Microsoft.Xna.Framework;
using TerraBend.Common.Players;
using TerraBend.Content.UI.Elements;
using TerraBend.Custom.Utils;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace TerraBend.Content.UI.States {
    /// <summary>
    /// UIState that handles the Chi resource UI, which is just a visual representation
    /// of the player's Chi.
    /// </summary>
    public class ChiUIState : UIState {
        public ChiResourceElement chiBar;

        public DraggableElement backgroundElement;

        public UIText chiTextRep;

        public override void OnInitialize() {
            chiBar = new ChiResourceElement() {
                HAlign = 0.5f,
                VAlign = 0.8f
            };

            backgroundElement = new DraggableElement() {
                Left = new StyleDimension(Main.screenWidth * 0.64f, 0f),
                Top = new StyleDimension(26f, 0f),
                Width = chiBar.Width,
                Height = new StyleDimension(54f, 0f)
            };
            Append(backgroundElement);

            chiTextRep = new UIText("Chi: 50/50") {
                HAlign = 0.5f
            };

            backgroundElement.Append(chiBar);
            backgroundElement.Append(chiTextRep);
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);

            backgroundElement.Width = chiBar.Width;

            ChiPlayer chiPlayer = Main.LocalPlayer.GetModPlayer<ChiPlayer>();
            chiTextRep.SetText(
                $"{LocalizationUtils.GetModTextValue("Common.Chi")}: {chiPlayer.currentChi}/{chiPlayer.displayedMaxChi}"
            );
        }
    }
}