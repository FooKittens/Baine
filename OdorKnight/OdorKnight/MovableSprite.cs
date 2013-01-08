using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Baine
{
    abstract class MovingSprite : Sprite
    {
        // Movement
        protected Vector2 speed;
        protected float movementSpeed;
        protected float maxHorizontalSpeed = 7;
        protected float friction;
        protected bool touchesGround;

        protected Vector2 previousPosition;
        protected Vector2 previousSpeed;

        // Animation
        protected SpriteEffects previousFlip;

        public MovingSprite(Vector2 position, string textureKey, float layer)
            : base(position, textureKey, layer)
        {
            touchesGround = false;
        }

        public MovingSprite(System.IO.BinaryReader r)
            :base(r)
        {
            // Get Movable-data
            touchesGround = r.ReadBoolean();
        }

        public virtual void Update(GameTime gameTime)
        {
            MovementUpdate();

            InputUpdate();

            AnimationUpdate(gameTime);
        }

        public override void GetSaveData(System.IO.BinaryWriter w)
        {
            base.GetSaveData(w);

            w.Write((bool)touchesGround);
        }

        protected abstract void MovementUpdate();

        protected abstract void InputUpdate();

        protected abstract void AnimationUpdate(GameTime gameTime);
    }
}
