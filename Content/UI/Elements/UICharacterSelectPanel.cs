using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.UI;

namespace TerraBend.Content.UI.Elements {
    /// <summary>
    /// Special panel used within the character selection screen.
    /// </summary>
    public class UICharacterSelectPanel : UIElement {
        private readonly Asset<Texture2D> _backgroundPanelAsset;

        public UICharacterSelectPanel(Vector2 drawPos, float width) {
            Left.Set(drawPos.X, 0f);
            Top.Set(drawPos.Y, 0f);
            _backgroundPanelAsset = Main.Assets.Request<Texture2D>("Images/UI/InnerPanelBackground", AssetRequestMode.ImmediateLoad);
            Width.Set(width, 0f);
            Height.Set(_backgroundPanelAsset.Height(), 0f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch) {
            base.DrawSelf(spriteBatch);
            Vector2 drawPos = GetDimensions().Position();
            Vector2 elementSize = new Vector2(GetDimensions().Width, GetDimensions().Height);

            //Adapted vanilla code
            spriteBatch.Draw(_backgroundPanelAsset.Value, drawPos, new Rectangle(0, 0, 8, (int)elementSize.Y), Color.White);
            spriteBatch.Draw(_backgroundPanelAsset.Value, new Vector2(drawPos.X + 8f, drawPos.Y), new Rectangle(8, 0, 8, (int)elementSize.Y), Color.White, 0f, Vector2.Zero, new Vector2((elementSize.X - 16f) / 8f, 1f), SpriteEffects.None,
                0f);
            spriteBatch.Draw(_backgroundPanelAsset.Value, new Vector2(drawPos.X + elementSize.X - 8f, drawPos.Y), new Rectangle(16, 0, 8, (int)elementSize.Y), Color.White);
        }
    }
}