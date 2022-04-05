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
        private Asset<Texture2D> _pannelInner;

        public JingResourceElement() {
            string jingPath = nameof(TerraBend) + "/Assets/Sprites/UI/JingPanel/";

            _panelEdge = ModContent.Request<Texture2D>(jingPath + "JingPanelEdge", AssetRequestMode.ImmediateLoad);
            _panelMiddle = ModContent.Request<Texture2D>(jingPath + "JingPanelMiddle", AssetRequestMode.ImmediateLoad);
            _pannelInner = ModContent.Request<Texture2D>(jingPath + "JingPanelInner");

            Width.Set(_panelEdge.Width() * 2 + _panelMiddle.Width() * 5, 0f);
            Height.Set(_panelEdge.Height(), 0f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch) {
            base.DrawSelf(spriteBatch);

            JingPlayer jingPlayer = Main.LocalPlayer.GetModPlayer<JingPlayer>();
            float middlePartCount = jingPlayer.MaxJing / 20f;

            CalculatedStyle dimensions = GetDimensions();
            Vector2 elementDrawPos = dimensions.Position();
            Vector2 edgeDims = new Vector2(_panelEdge.Width(), _panelEdge.Height());
            Vector2 middleDims = new Vector2(_panelMiddle.Width(), _panelMiddle.Height());

            //Draw full middle bar parts
            for (int i = 0; i < (int)middlePartCount; i++) {
                spriteBatch.Draw(_panelMiddle.Value,
                    new Rectangle((int)(elementDrawPos.X + edgeDims.X + middleDims.X * i), (int)elementDrawPos.Y, (int)middleDims.X, (int)middleDims.Y),
                    null,
                    Color.White
                );
            }

            //Draw fractional part, if applicable
            if (middlePartCount > (int)middlePartCount) {
                int fractionalScale = (int)(middleDims.X * (middlePartCount - (int)middlePartCount));

                spriteBatch.Draw(_panelMiddle.Value,
                    new Rectangle((int)(elementDrawPos.X + edgeDims.X + middleDims.X * (int)middlePartCount), (int)elementDrawPos.Y, fractionalScale, (int)middleDims.Y),
                    new Rectangle(0, 0, fractionalScale, (int)middleDims.Y),
                    Color.White
                );
            }

            //Draw bar edges
            spriteBatch.Draw(_panelEdge.Value, elementDrawPos, Color.White);
            spriteBatch.Draw(_panelEdge.Value,
                new Rectangle((int)(elementDrawPos.X + edgeDims.X + middleDims.X * middlePartCount), (int)elementDrawPos.Y, (int)edgeDims.X, (int)edgeDims.Y),
                null,
                Color.White,
                0f,
                default,
                SpriteEffects.FlipHorizontally,
                0f
            );

            //TODO: Draw inner bar (Jing values)
            float positiveJingBars = jingPlayer.GetPositiveJing() / 20f;
            float neutralJingBars = jingPlayer.GetNeutralJing() / 20f;
            float negativeJingBars = jingPlayer.GetNegativeJing() / 20f;

            //Update size
            Width.Set(edgeDims.X * 2 + middleDims.X * middlePartCount, 0f);
        }
    }
}