using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using TerraBend.Common.Configs;
using TerraBend.Common.MiscLoadables;
using TerraBend.Common.Players;
using TerraBend.Custom.Enums;
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

        /// <summary>
        /// Denominator value that determines the size of each individual "bar" piece.
        /// Basically, how much each bar piece denotes in numbers.
        /// </summary>
        private readonly float _barSizeDenomination = 20f;

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
            float middlePartCount = jingPlayer.MaxJing / _barSizeDenomination;

            CalculatedStyle dimensions = GetDimensions();
            Vector2 elementDrawPos = dimensions.Position();
            Vector2 edgeDims = new Vector2(_panelEdge.Width(), _panelEdge.Height());
            Vector2 middleDims = new Vector2(_panelMiddle.Width(), _panelMiddle.Height());

            //Draw middle bar outlines
            DrawBar(spriteBatch, elementDrawPos + new Vector2(edgeDims.X, 0f), middlePartCount, _panelMiddle);

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

            //Draw actual Jing Value bars
            float positiveJingBars = jingPlayer.PositiveJing / _barSizeDenomination;
            float neutralJingBars = jingPlayer.NeutralJing / _barSizeDenomination;
            float negativeJingBars = jingPlayer.NegativeJing / _barSizeDenomination;
            float unalignedJingBars = jingPlayer.UnalignedJing / _barSizeDenomination;
            ClientConfig clientConfig = ModContent.GetInstance<ClientConfig>();
            Color[] jingColors = new Color[] { clientConfig.positiveJingColor, clientConfig.neutralJingColor, clientConfig.negativeJingColor, clientConfig.unalignedJingColor };

            for (int i = 0; i < 4; i++) {
                float[] jingBarCounts = new float[] { positiveJingBars, neutralJingBars, negativeJingBars, unalignedJingBars };
                float totalBarCount = jingBarCounts.Take(i).Sum();

                Vector2 innerBarDrawPos = elementDrawPos + new Vector2(edgeDims.X, 0f) + new Vector2(_pannelInner.Width() * totalBarCount, 6f);
                DrawBar(spriteBatch, innerBarDrawPos, jingBarCounts[i], _pannelInner, jingColors[i]);
            }

            //Update size
            Width.Set(edgeDims.X * 2 + middleDims.X * middlePartCount, 0f);
        }

        /// <summary>
        /// Draws a bar of specified bar size <paramref name="partCount"/> with the specified texture.
        /// </summary>
        /// <param name="spriteBatch"> The spritebatch that will draw the bars. </param>
        /// <param name="elementDrawPos"> The position of the far-left most bar. </param>
        /// <param name="partCount"> How many parts needing to be drawn. </param>
        /// <param name="textureToDraw"> The singular texture of a bar piece. </param>
        /// <param name="barColor"> If applicable, the color to draw this bar in. </param>
        private void DrawBar(SpriteBatch spriteBatch, Vector2 elementDrawPos, float partCount, Asset<Texture2D> textureToDraw, Color? barColor = null) {
            Vector2 textureDimensions = new Vector2(textureToDraw.Width(), textureToDraw.Height());

            //Draw full bar parts
            for (int i = 0; i < (int)partCount; i++) {
                spriteBatch.Draw(textureToDraw.Value,
                    new Rectangle((int)(elementDrawPos.X + textureDimensions.X * i), (int)elementDrawPos.Y, (int)textureDimensions.X, (int)textureDimensions.Y),
                    null,
                    barColor ?? Color.White
                );
            }

            //Draw fractional part, if applicable
            if (partCount > (int)partCount) {
                int fractionalScale = (int)(textureDimensions.X * (partCount - (int)partCount));

                spriteBatch.Draw(textureToDraw.Value,
                    new Rectangle((int)(elementDrawPos.X + textureDimensions.X * (int)partCount), (int)elementDrawPos.Y, fractionalScale, (int)textureDimensions.Y),
                    new Rectangle(0, 0, fractionalScale, (int)textureDimensions.Y),
                    barColor ?? Color.White
                );
            }
        }
    }
}