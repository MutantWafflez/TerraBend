using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using TerraBend.Common.Players;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace TerraBend.Content.UI.Elements {
    /// <summary>
    /// Highly specific element that handles the Jing Panel.
    /// Handles coloration, updating, all that fun stuff.
    /// </summary>
    public class JingResourceElement : UIElement {
        private Asset<Texture2D> _panelEdge;
        private Asset<Texture2D> _panelMiddle;

        public JingResourceElement() {
            _panelEdge = ModContent.Request<Texture2D>(nameof(TerraBend) + "/Assets/Sprites/UI/JingPanel/JingPanelEdge", AssetRequestMode.ImmediateLoad);
            _panelMiddle = ModContent.Request<Texture2D>(nameof(TerraBend) + "/Assets/Sprites/UI/JingPanel/JingPanelMiddle", AssetRequestMode.ImmediateLoad);

            Width.Set(_panelEdge.Width() * 2 + _panelMiddle.Width() * 5, 0f);
            Height.Set(_panelEdge.Height(), 0f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch) {
            base.DrawSelf(spriteBatch);

            JingPlayer jingPlayer = Main.LocalPlayer.GetModPlayer<JingPlayer>();
            float middlePartCount = jingPlayer.MaxJing / 30f;

            CalculatedStyle dimensions = GetDimensions();
            Vector2 elementDrawPos = dimensions.Position();

            spriteBatch.Draw(_panelEdge.Value, elementDrawPos, Color.White);
            spriteBatch.Draw(_panelEdge.Value,
                new Rectangle((int)(elementDrawPos.X + _panelMiddle.Width() * middlePartCount), (int)elementDrawPos.Y, _panelEdge.Width(), _panelEdge.Height()),
                null,
                Color.White
            );

            for (int i = 0; i <= (int)middlePartCount; i++) {
                float scale = i == (int)middlePartCount ? middlePartCount - (int)middlePartCount : 1f;

                spriteBatch.Draw(_panelMiddle.Value,
                    new Rectangle((int)(elementDrawPos.X + _panelEdge.Width() + _panelMiddle.Width() * i), (int)elementDrawPos.Y, _panelMiddle.Width(), _panelMiddle.Height()),
                    new Rectangle(0, 0, (int)(_panelMiddle.Width() * scale), _panelMiddle.Height()),
                    Color.White
                );
            }
        }
    }
}