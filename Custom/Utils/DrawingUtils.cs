using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using TerraBend.Content.UI.Elements;
using Terraria;

namespace TerraBend.Custom.Utils {
    /// <summary>
    /// Utilities class that has shortcuts or general utilities for drawing stuff.
    /// </summary>
    public static class DrawingUtils {
        /// <summary>
        /// Draws a bar of specified bar size <paramref name="partCount"/> with the specified texture.
        /// See <seealso cref="JingResourceElement"/> for example usage.
        /// </summary>
        /// <param name="spriteBatch"> The spritebatch that will draw the bars. </param>
        /// <param name="elementDrawPos"> The position of the far-left most bar. </param>
        /// <param name="partCount"> How many parts needing to be drawn. </param>
        /// <param name="textureToDraw"> The singular texture of a bar piece. </param>
        /// <param name="barColor"> If applicable, the color to draw this bar in. </param>
        public static void DrawBar(SpriteBatch spriteBatch, Vector2 elementDrawPos, float partCount, Asset<Texture2D> textureToDraw, Color? barColor = null) {
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