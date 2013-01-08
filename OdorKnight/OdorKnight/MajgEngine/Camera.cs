using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MajgEngine;
using Baine;

namespace MajgEngine
{
    class Camera
    {
        Vector2 halfScreenSize;
        private Vector2 position;
        public Vector2 Position { get { return position; } }
        public Vector2 Origin { get; private set; }
        public float Rotation { get; private set; }
        public float Scale { get; private set; }
        public Matrix Transform { get; private set; }
        public Rectangle Bounds
        {
            get
            {
                Vector2 topLeft = Vector2.Zero;
                Vector2 btmRight = halfScreenSize * 2;
                topLeft = TranslatePositionByCamera(topLeft);
                btmRight = TranslatePositionByCamera(btmRight);
                Rectangle result = new Rectangle((int)topLeft.X, (int)topLeft.Y, (int)(btmRight.X - topLeft.X), (int)(btmRight.Y - topLeft.Y));
                return result;
            }
        }

        public Camera(Vector2 screenSize)
        {
            halfScreenSize = screenSize / 2;
            Scale = 1f;
            position = halfScreenSize / Scale;
            Origin = halfScreenSize / Scale;
            Rotation = 0;

            UpdateTransformMatrix();
        }

        public void Update()
        {
            float camSpeed = 5;
            float zoomSpeed = 0.05f;

            if (Game1.currentState == Game1.GameState.Editor)
            {
                if (Input.Key_Down(Keys.Add))
                    Scale += zoomSpeed;
                if (Input.Key_Down(Keys.Subtract))
                    Scale -= zoomSpeed;
                Scale = MathHelper.Clamp(Scale, 0.1f, 2.5f);

                if (Input.Key_Down(Keys.Y))
                    Rotation -= 0.05f;
                if (Input.Key_Down(Keys.U))
                    Rotation += 0.05f;

                if (Input.Key_Down(Keys.W))
                    position.Y -= camSpeed / Scale;
                if (Input.Key_Down(Keys.A))
                    position.X -= camSpeed / Scale;
                if (Input.Key_Down(Keys.S))
                    position.Y += camSpeed / Scale;
                if (Input.Key_Down(Keys.D))
                    position.X += camSpeed / Scale;
                Origin = halfScreenSize / Scale;
            }
            if (position.Y > Origin.Y)
                position.Y = Origin.Y;
            if (position.X < Origin.X)
                position.X = Origin.X;
            UpdateTransformMatrix();
        }

        public void CenterOn(Vector2 position)
        {
            this.position = position;
            if (this.position.Y > Origin.Y)
                this.position.Y = Origin.Y;
            if (this.position.X < Origin.X)
                this.position.X = Origin.X;
        }

        private void UpdateTransformMatrix()
        {
            Transform = Matrix.Identity *
                    Matrix.CreateRotationZ(Rotation) *
                    Matrix.CreateTranslation(Origin.X - position.X, Origin.Y - position.Y, -0.1f) *
                    Matrix.CreateScale(new Vector3(Scale, Scale, Scale));
        }

        public Vector2 TranslatePositionByCamera(Vector2 position)
        {
            Vector2 result = (position / Scale) - (Origin - Position);
            return result;
        }
    }
}
