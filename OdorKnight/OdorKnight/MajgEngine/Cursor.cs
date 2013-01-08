using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace MajgEngine
{
    public class Cursor
    {
        private Texture2D cursorTexture = Library.textures["Cursor"];
        private Vector2 dotOffset = new Vector2(3);
        private bool showCursor = true;
        private float cursorRotation = 0;
        private Color cursorColor = Color.White;
        private bool canXboxControllerMoveMe = false;
        private Vector2 mousePos;

        public void Update()
        {
            mousePos = Input.Mouse_Position();

            if (showCursor)
            {
                Vector2 direction = mousePos - Input.Mouse_PreviousPosition();
                Face(direction);
            }

            // Move cursor with xbox controller
            if (canXboxControllerMoveMe)
            {
                List<PlayerIndex> indices = new List<PlayerIndex> { PlayerIndex.One, PlayerIndex.Two, PlayerIndex.Three, PlayerIndex.Four };
                Vector2 movement = Vector2.Zero;
                foreach (PlayerIndex pi in indices)
                {
                    movement += Input.GP_LeftThumbstick(pi) * 5;
                }
                Mouse.SetPosition(Convert.ToInt32(mousePos.X + movement.X), Convert.ToInt32(mousePos.Y - movement.Y));
            }
        }

        public void Face(Vector2 direction)
        {
            if (direction != Vector2.Zero)
            {
                float goalRotation = (float)Math.Atan2(direction.Y, direction.X) + 0.63f * MathHelper.Pi;
                PhysicsHelper.Face(ref cursorRotation, goalRotation, 0.2f);
            }
        }

        public void SetCursorColor(Color color)
        {
            cursorColor = color;
        }

        public void Draw(SpriteBatch sb)
        {
            if (showCursor)
                sb.Draw(cursorTexture, mousePos, null, cursorColor, cursorRotation, Vector2.Zero, Input.Mouse_LeftDown() ? 0.8f : 1, SpriteEffects.None, 1);
        }
    }
}
