using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MajgEngine;
using System.Xml;

namespace Baine
{
    class Level
    {
        List<Sprite> sprites;
        List<MovingSprite> movingSprites;
        public Vector2 startPos = new Vector2(10, 300);
        public Vector2 goalPos = new Vector2(110, 300);

        public List<Sprite> Sprites { get { return sprites; } }
        public List<MovingSprite> MovingSprites { get { return movingSprites; } }

        public enum PlatformType // Each name of PlatformType must equal the key of the texture used by Library.textures
        {
            _8x8,
            _8x1,
            _10x1,
            _3x1,
            _16x16,
            COUNT
        }

        public enum TrapType // Each name of TrapType must equal the key of the texture used by Library.textures
        {
            Spikes,
            COUNT
        }

        public enum PotionType // Each name of PotionType must equal the key of the texture used by Librar.textures
        {
            HealthPotion,
            COUNT
        }

        public const float FarBack = 0.1f;
        public const float BaineLayer = 0.6f;
        public const float FarFront = 1f;

        public Level()
        {
            sprites = new List<Sprite>();
            movingSprites = new List<MovingSprite>();
            movingSprites.Add(new Enemy2(new Vector2(500, 0), "Enemy2Moving", Level.BaineLayer));
        }

        public void AddSprite(Sprite sprite)
        {
            sprites.Add(sprite);
        }

        public void RemoveSprite(Sprite sprite)
        {
            sprites.Remove(sprite);
        }

        public void AddSprite(MovingSprite sprite)
        {
            movingSprites.Add(sprite);
        }

        public void RemoveSprite(MovingSprite sprite)
        {
            movingSprites.Remove(sprite);
        }

        public void ClearLevel()
        {
            sprites.RemoveRange(0, sprites.Count);
            movingSprites.RemoveRange(0, movingSprites.Count);
        }

        public void Update(GameTime gameTime)
        {
            foreach (MovingSprite sprite in movingSprites)
            {
                sprite.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            // Draw every sprite inside camera bounds
            for (int i = 0; i < sprites.Count; i++)
            {
                if (sprites[i].IsDead)
                {
                    sprites.RemoveAt(i--);
                    continue;
                }
                if (sprites[i].Bounds.Intersects(camera.Bounds))
                    sprites[i].Draw(spriteBatch);
            }
            // Draw every moving sprite inside camera bounds
            for (int i = 0; i < movingSprites.Count; i++)
            {
                if (movingSprites[i].IsDead)
                {
                    movingSprites.RemoveAt(i--);
                    continue;
                }
                if (movingSprites[i].Bounds.Intersects(camera.Bounds))
                    movingSprites[i].Draw(spriteBatch);
            }
            spriteBatch.Draw(Library.textures["GoalFlag"], goalPos, null, Color.White, 0, new Vector2(5, 40), 1, SpriteEffects.None, 1);
        }

        public void GetSaveData(System.IO.BinaryWriter w)
        {
            w.Write((float)startPos.X);
            w.Write((float)startPos.Y);
            w.Write((float)goalPos.X);
            w.Write((float)goalPos.Y);
            for (int i = 0; i < sprites.Count; i++)
            {
                sprites[i].GetSaveData(w);
            }
            for (int i = 0; i < movingSprites.Count; i++)
            {
                movingSprites[i].GetSaveData(w);
            }
            Console.WriteLine("Save successful");
        }
    }
}
