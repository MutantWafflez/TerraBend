using Microsoft.Xna.Framework;
using Terraria;
using Terraria.UI;

namespace TerraBend.Content.UI.Elements {
    /// <summary>
    /// UIElement that can be dragged.
    /// </summary>
    public class DraggableElement : UIElement {
        public Vector2 elementOffset;
        private bool _isDragging;

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);

            elementOffset = new Vector2(Left.Pixels, Top.Pixels);

            //Separate check here so that the first click doesn't trigger an item usage
            if (ContainsPoint(Main.MouseScreen)) {
                Main.LocalPlayer.mouseInterface = true;
            }

            if (Main.mouseLeft) {
                //Begin the dragging process if the mouse is down and is clicking on the UI
                if (ContainsPoint(Main.MouseScreen)) {
                    _isDragging = true;
                }
                //Continue to drag if isDragging is true, even if the mouse deviates from the UI (which it can by accident if the mouse is moved too fast)
                else if (!_isDragging) {
                    _isDragging = false;
                }
            }
            else {
                _isDragging = false;
            }

            if (_isDragging) {
                //Minus half the size of the element so the mouse is in the center of the element  when dragging
                elementOffset = new Vector2(Main.mouseX - Width.Pixels / 2, Main.mouseY - Height.Pixels / 2);

                Left.Set(elementOffset.X, 0f);
                Top.Set(elementOffset.Y, 0f);
            }

            CheckAndCorrectBounds();
        }

        //Quick method that checks if the element is on the screen and corrects it if off the screen
        private void CheckAndCorrectBounds() {
            CalculatedStyle dimensions = GetDimensions();
            Vector2 position = dimensions.Position();

            //Top check
            if (position.Y < 0f) {
                Top.Set(0f, 0f);
            }
            else if (position.Y + dimensions.Height > Main.screenHeight) {
                Top.Set(Main.screenHeight - dimensions.Height, 0f);
            }

            //Left Check
            if (position.X < 0f) {
                Left.Set(0f, 0f);
            }
            else if (position.X + dimensions.Width > Main.screenWidth) {
                Left.Set(Main.screenWidth - dimensions.Width, 0f);
            }

            Recalculate();
        }
    }
}