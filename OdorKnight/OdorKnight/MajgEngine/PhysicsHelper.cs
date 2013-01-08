using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MajgEngine
{
    public static class PhysicsHelper
    {
        public const float Gravity = 0.982f;

        /// <summary>
        /// Modifies a rotation to gradually face a direction
        /// </summary>
        /// <param name="rotation">Initial rotation</param>
        /// <param name="dir">Direction to face</param>
        /// <param name="rotationSpeed">Increment / Decrement per iteration</param>
        /// <returns>New rotation</returns>
        public static void Face(ref float rotation, float goalRotation, float rotationSpeed)
        {
            rotation = MathHelper.WrapAngle(rotation);
            float goal = MathHelper.WrapAngle(goalRotation);
            if (rotation != goal)
            {
                float positiveRotLength, negativeRotLength;
                if (rotation - goal >= rotationSpeed)
                {
                    negativeRotLength = rotation - goal;
                    positiveRotLength = MathHelper.TwoPi - rotation + goal;
                }
                else if (rotation - goal <= -rotationSpeed)
                {
                    positiveRotLength = goal - rotation;
                    negativeRotLength = MathHelper.TwoPi - goal + rotation;
                }
                else
                {
                    rotation = goal;
                    return;
                }

                if (positiveRotLength < negativeRotLength)
                    rotation += rotationSpeed;
                else
                    rotation -= rotationSpeed;
            }
        }


        /// <summary>
        /// Moves a vector towards a goal position
        /// </summary>
        /// <param name="toMove">Reference vector moved</param>
        /// <param name="goalPos">The position to move towards</param>
        /// <param name="speed">How far the vector is moved, in pixels</param>
        public static void MoveTowards(ref Vector2 toMove, Vector2 goalPos, float speed)
        {
            Vector2 diff = goalPos - toMove;
            if (diff.Length() < speed)
                toMove = goalPos;
            else
            {
                diff.Normalize();
                toMove += diff * speed;
            }
        }

        /// <summary>
        /// Rotates a Vector2 with a given rotation around a point
        /// </summary>
        /// <param name="toRotate">The Vector2 to change</param>
        /// <param name="rotation">Rotation amount</param>
        /// <param name="origin">Point to rotate around</param>
        public static void RotateVector2(ref Vector2 toRotate, float rotation, Vector2 origin)
        {
            Vector2 vector = new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
            toRotate = new Vector2(vector.X * (toRotate.X - origin.X) - vector.Y * (toRotate.Y - origin.Y) + origin.X, vector.Y * (toRotate.X - origin.X) + vector.X * (toRotate.Y - origin.Y) + origin.Y);
        }
    }
}
