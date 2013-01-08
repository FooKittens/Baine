using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MajgEngine;

namespace Baine
{
    class ParallaxBackground
    {
        Vector2 offset;
        Texture2D texture;
        float layer;
        public ParallaxBackground(Vector2 offset, Texture2D texture, float layer)
        {
            this.offset = offset;
            this.texture = texture;
            this.layer = layer;
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera, bool wrap)
        {
            Rectangle? rect = null;
            int screenWidth = Game1.graphics.PreferredBackBufferWidth;
            if (wrap)
            {
                if (camera.Position.X / (1 / layer) - offset.X > texture.Bounds.Width)
                    offset.X += texture.Bounds.Width;
                if (camera.Position.X / (1 / layer) - offset.X < 0)
                    offset.X -= texture.Bounds.Width;
                rect = new Rectangle(0, 0, screenWidth + texture.Bounds.Width, Game1.graphics.PreferredBackBufferHeight);
            }
            spriteBatch.Draw(texture, new Vector2(-camera.Position.X / (1 / layer), 0) + offset, rect, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, layer);
        }
    }
}
