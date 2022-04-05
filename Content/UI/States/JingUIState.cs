using TerraBend.Content.UI.Elements;
using Terraria;
using Terraria.UI;

namespace TerraBend.Content.UI.States {
    /// <summary>
    /// Relatively simple UIState that handles the Jing "resource" bar.
    /// </summary>
    public class JingUIState : UIState {
        public JingResourceElement jingBar;

        public DraggableElement backgroundElement;

        public override void OnInitialize() {
            jingBar = new JingResourceElement() {
                HAlign = 0.5f,
                VAlign = 0.8f
            };

            backgroundElement = new DraggableElement() {
                Top = new StyleDimension(Main.screenWidth / 2f, 0f),
                Left = new StyleDimension(300f, 0f),
                Width = new StyleDimension(jingBar.Width.Pixels + 8f, 0f),
                Height = new StyleDimension(80f, 0f)
            };
            Append(backgroundElement);

            backgroundElement.Append(jingBar);
        }
    }
}