using Microsoft.Xna.Framework;
using TerraBend.Common.Players;
using TerraBend.Content.UI.Elements;
using TerraBend.Custom.Utils;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace TerraBend.Content.UI.States {
    /// <summary>
    /// Relatively simple UIState that handles the Jing "resource" bar.
    /// </summary>
    public class JingUIState : UIState {
        public JingResourceElement jingBar;

        public DraggableElement backgroundElement;

        public UIText majorityJingElement;

        private readonly float _panelPadding = 8f;

        public override void OnInitialize() {
            PaddingLeft = _panelPadding;

            jingBar = new JingResourceElement() {
                HAlign = 0.5f,
                VAlign = 0.8f
            };

            backgroundElement = new DraggableElement() {
                Left = new StyleDimension(Main.screenWidth * 0.73f, 0f),
                Top = new StyleDimension(20f, 0f),
                Width = jingBar.Width,
                Height = new StyleDimension(100f, 0f)
            };
            Append(backgroundElement);

            majorityJingElement = new UIText("Majority:\nUnaligned") {
                HAlign = 0.5f,
                Top = new StyleDimension(_panelPadding, 0f)
            };

            backgroundElement.Append(jingBar);
            backgroundElement.Append(majorityJingElement);
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);

            backgroundElement.Width = jingBar.Width;

            majorityJingElement.SetText(
                $"{LocalizationUtils.GetModTextValue("Common.Majority")}:\n{LocalizationUtils.GetModTextValue($"JingStatus.{Main.LocalPlayer.GetModPlayer<JingPlayer>().GetMajorityJing()}")}");
        }
    }
}