using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using TerraBend.Common.Players;
using TerraBend.Custom.Utils;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace TerraBend.Content.UI.Elements {
    /// <summary>
    /// Highly specific element that handles the Chi Frame/Bar.
    /// Handles coloration, updating, all that fun stuff.
    /// </summary>
    public class ChiResourceElement : UIElement {
        private Asset<Texture2D> _frameEdge;
        private Asset<Texture2D> _frameMiddle;
        private Asset<Texture2D> _frameInner;
        private Asset<Texture2D> _frameInnerBack;

        /// <summary>
        /// Denominator value that determines the size of each individual "bar" piece.
        /// Basically, how much each bar piece denotes in numbers.
        /// </summary>
        private readonly float _barSizeDenomination = 25f;

        public ChiResourceElement() {
            string chiPath = nameof(TerraBend) + "/Assets/Sprites/UI/ChiBar/";

            _frameEdge = ModContent.Request<Texture2D>(chiPath + "ChiFrameEdge", AssetRequestMode.ImmediateLoad);
            _frameMiddle = ModContent.Request<Texture2D>(chiPath + "ChiFrameMiddle", AssetRequestMode.ImmediateLoad);
            _frameInner = ModContent.Request<Texture2D>(chiPath + "ChiFrameInner");
            _frameInnerBack = ModContent.Request<Texture2D>(chiPath + "ChiFrameInnerBack");

            Width.Set(_frameEdge.Width() * 2 + _frameMiddle.Width() * 2, 0f);
            Height.Set(_frameEdge.Height(), 0f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch) {
            base.DrawSelf(spriteBatch);

            ChiPlayer chiPlayer = Main.LocalPlayer.GetModPlayer<ChiPlayer>();
            float middlePartCount = MathHelper.Max(chiPlayer.baseMaxChi, chiPlayer.displayedMaxChi) / _barSizeDenomination;

            CalculatedStyle dimensions = GetDimensions();
            Vector2 elementDrawPos = dimensions.Position();
            Vector2 edgeDims = new Vector2(_frameEdge.Width(), _frameEdge.Height());
            Vector2 middleDims = new Vector2(_frameMiddle.Width(), _frameMiddle.Height());

            //Draw middle bar frame
            DrawingUtils.DrawBar(spriteBatch, elementDrawPos + new Vector2(edgeDims.X, 0f), middlePartCount, _frameMiddle);

            //Draw bar edges
            spriteBatch.Draw(_frameEdge.Value, elementDrawPos, Color.White);
            spriteBatch.Draw(_frameEdge.Value,
                new Rectangle((int)(elementDrawPos.X + edgeDims.X + middleDims.X * middlePartCount), (int)elementDrawPos.Y, (int)edgeDims.X, (int)edgeDims.Y),
                null,
                Color.White,
                0f,
                default,
                SpriteEffects.FlipHorizontally,
                0f
            );

            //Draw inner back bar
            DrawingUtils.DrawBar(spriteBatch, elementDrawPos + new Vector2(edgeDims.X, 4f), middlePartCount, _frameInnerBack);

            //Draw actual chi value
            DrawingUtils.DrawBar(spriteBatch, elementDrawPos + new Vector2(edgeDims.X, 4f), chiPlayer.currentChi / _barSizeDenomination, _frameInner);

            //Draw "middle" line (to represent point where bending dies)
            //Note this line isn't always in the middle, it usually is unless displayedMaxChi > baseMaxChi
            spriteBatch.Draw(TextureAssets.MagicPixel.Value,
                new Rectangle((int)(elementDrawPos.X + edgeDims.X + middleDims.X * (chiPlayer.baseMaxChi / _barSizeDenomination / 2f)), (int)(elementDrawPos.Y + 2f), 2, (int)(middleDims.Y - 4)),
                null,
                Color.Red
            );

            //Draw "lowered" line if displayedMaxChi < baseMaxChi, which represents the upper bound for the current chi
            if (chiPlayer.displayedMaxChi < chiPlayer.baseMaxChi) {
                spriteBatch.Draw(TextureAssets.MagicPixel.Value,
                    new Rectangle((int)(elementDrawPos.X + edgeDims.X + middleDims.X * (chiPlayer.displayedMaxChi / _barSizeDenomination)), (int)(elementDrawPos.Y + 2f), 2, (int)(middleDims.Y - 4)),
                    null,
                    Color.Yellow
                );
            }

            //Update size
            Width.Set(edgeDims.X * 2 + middleDims.X * middlePartCount, 0f);
        }
    }
}