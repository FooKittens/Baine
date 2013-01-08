using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MajgEngine;

namespace Baine
{
    class Sprite
    {
        private Vector2 baseOrigin;
        protected Texture2D baseTexture, texture;
        protected Vector2 position;
        public Vector2 Position { get { return position; } }
        protected Vector2 origin;
        protected SpriteEffects flip;
        protected string textureKey;
        protected RenderTarget2D renderTarget;
        protected float rotation;
        public bool needsRedraw { get; set; }
        protected SaveFileManager.SaveTypeIdentifier identifier;
        public float layer { get; set; }
        protected Color color;
        public bool IsDead { get; set; }

        public Sprite(Vector2 position, string textureKey, float layer)
        {
            // Create copy of texture because this class may change it's own instance
            rotation = 0;
            this.textureKey = textureKey;
            texture = Repainter.GetTextureCopy(Library.textures[textureKey]);
            baseTexture = texture;
            baseOrigin = new Vector2(texture.Width / 2, texture.Height / 2);
            origin = baseOrigin;
            this.position = position;
            needsRedraw = false;
            identifier = SaveFileManager.SaveTypeIdentifier.Sprite;
            color = Color.White;
            IsDead = false;
        }

        public Sprite(System.IO.BinaryReader r)
        {
            // 2 Load texture
            //ushort width = r.ReadUInt16();
            //ushort height = r.ReadUInt16();
            //byte[] colorData = r.ReadBytes(width * height * 4);
            //texture = new Texture2D(Game1.graphics.GraphicsDevice, width, height);
            //texture.SetData(colorData);

            // 3 Load position
            position.X = r.ReadSingle();
            position.Y = r.ReadSingle();

            // 4 Load rotation
            rotation = r.ReadSingle();

            // 5 Load textureKey
            byte length = r.ReadByte();
            textureKey = new string(r.ReadChars(length));
            baseTexture = Repainter.GetTextureCopy(Library.textures[textureKey]);
            texture = baseTexture;

            // 6 Load layer
            layer = r.ReadSingle();

            // Set other data
            baseOrigin = new Vector2(baseTexture.Width / 2, baseTexture.Height / 2);
            origin = baseOrigin;
            needsRedraw = true;
            identifier = SaveFileManager.SaveTypeIdentifier.Sprite;
            color = Color.White;
            IsDead = false;

            Console.WriteLine(ToString() + " loaded at position: " + position.ToString());
        }

        public virtual void GetSaveData(System.IO.BinaryWriter w)
        {
            // 1 Save identifier
            w.Write((byte)identifier);

            // 2 Save texture
            //w.Write((ushort)texture.Width);
            //w.Write((ushort)texture.Height);
            //byte[] textureData = new byte[texture.Width * texture.Height * 4];
            //texture.GetData(textureData);
            //w.Write(textureData, 0, textureData.Length);

            // 3 Save pos
            w.Write((float)position.X);
            w.Write((float)position.Y);

            // 4 Save rotation
            w.Write((float)rotation);

            // 5 Save texture key
            w.Write((byte)textureKey.Length);
            w.Write(textureKey.ToCharArray());

            // 6 Save layer
            w.Write((float)layer);

            Console.WriteLine(ToString() + " ID:" + GetHashCode().ToString() + " saved!");
        }

        public void Rotate(float angle)
        {
            rotation += angle;
            needsRedraw = true;
        }

        /// <summary>
        /// Checks for collision with other sprite
        /// </summary>
        /// <param name="other">The sprite to check against</param>
        /// <returns>True on collision, else false</returns>
        public bool PerPixelHits(Sprite other)
        {
            if (Bounds.Intersects(other.Bounds))
            {
                Texture2D otherTexture = other.texture;

                // Create color arrays of textures
                Color[] myColors = new Color[texture.Width * texture.Height];
                texture.GetData(myColors);
                Color[] otherColors = new Color[otherTexture.Width * otherTexture.Height];
                otherTexture.GetData(otherColors);

                // Calculate intersection
                int left = Math.Max(Bounds.X, other.Bounds.X);
                int right = Math.Min(Bounds.X + Bounds.Width, other.Bounds.X + other.Bounds.Width);

                int top = Math.Max(Bounds.Y, other.Bounds.Y);
                int bottom = Math.Min(Bounds.Y + Bounds.Height, other.Bounds.Y + other.Bounds.Height);

                for (int y = top; y < bottom; y++)
                {
                    for (int x = left; x < right; x++)
                    {
                        Color myColor = myColors[(x - Bounds.X) + (y - Bounds.Y) * Bounds.Width];
                        Color otherColor = otherColors[(x - other.Bounds.X) + (y - other.Bounds.Y) * other.Bounds.Width];

                        if (myColor.A + otherColor.A > 255)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void SetPosition(Vector2 position)
        {
            this.position = position;
        }

        public bool BoundingBoxHits(Sprite other)
        {
            if (Bounds.Intersects(other.Bounds))
                return true;
            return false;
        }

        public virtual Rectangle Bounds
        {
            get
            {
                return new Rectangle(
                    (int)(position.X - origin.X),
                    (int)(position.Y - origin.Y),
                    texture.Width,
                    texture.Height);
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Color newColor = Color.Multiply(color, 1 - layer);
            spriteBatch.Draw(texture, position, null, new Color(newColor.R, newColor.G, newColor.B, 255), 0, origin, 1, SpriteEffects.None, layer);
        }

        public virtual void Redraw(SpriteBatch sb, GraphicsDevice GraphicsDevice)
        {
            baseOrigin = new Vector2(baseTexture.Width / 2, baseTexture.Height / 2);
            // Calculate new texture dimensions
            Vector2 topleft = position - baseOrigin;
            Vector2 topright = position + new Vector2(baseOrigin.X, -baseOrigin.Y);
            Vector2 bottomleft = position + new Vector2(-baseOrigin.X, baseOrigin.Y);
            Vector2 bottomright = position + baseOrigin;
            MajgEngine.PhysicsHelper.RotateVector2(ref topleft, rotation, baseOrigin);
            MajgEngine.PhysicsHelper.RotateVector2(ref topright, rotation, baseOrigin);
            MajgEngine.PhysicsHelper.RotateVector2(ref bottomleft, rotation, baseOrigin);
            MajgEngine.PhysicsHelper.RotateVector2(ref bottomright, rotation, baseOrigin);
            float minX = Math.Min(topleft.X, topright.X);
            float minX2 = Math.Min(bottomleft.X, bottomright.X);
            minX = Math.Min(minX, minX2);
            float minY = Math.Min(topleft.Y, topright.Y);
            float minY2 = Math.Min(bottomleft.Y, bottomright.Y);
            minY = Math.Min(minY, minY2);
            float maxX = Math.Max(topleft.X, topright.X);
            float maxX2 = Math.Max(bottomleft.X, bottomright.X);
            maxX = Math.Max(maxX, maxX2);
            float maxY = Math.Max(topleft.Y, topright.Y);
            float maxY2 = Math.Max(bottomleft.Y, bottomright.Y);
            maxY = Math.Max(maxY, maxY2);

            // Draw new texture
            renderTarget = new RenderTarget2D(GraphicsDevice, (int)(maxX - minX), (int)(maxY - minY));

            GraphicsDevice.SetRenderTarget(renderTarget);
            Vector2 basePos = new Vector2((maxX - minX) / 2, (maxY - minY) / 2);

            GraphicsDevice.Clear(Color.Transparent);
            sb.Begin();
            sb.Draw(baseTexture, basePos, null, color, rotation, baseOrigin, 1, flip, 0);
            sb.End();

            texture = renderTarget;
            //origin = new Vector2((renderTarget.Bounds.Width) / 2, (renderTarget.Bounds.Height) / 2);
            GraphicsDevice.SetRenderTarget(null);
            Console.WriteLine(ToString() + " ID#" + GetHashCode() +" redrew itself.");
            needsRedraw = false;
        }
    }
}
