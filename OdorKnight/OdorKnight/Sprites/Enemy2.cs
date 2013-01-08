using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MajgEngine;

namespace Baine
{
    class Enemy2 : MovingSprite
    {
        // public float health { get; private set; }

        const float WalkDistance = 600;
        float distanceRemaining;
        bool movingRight;
        protected const float standardMovementSpeed = 1;
        public float Damage { get; protected set; }

        public Enemy2(Vector2 position, string textureKey, float layer)
            : base(position, textureKey, layer)
        {
            touchesGround = false;
            //health = 100;

            identifier = SaveFileManager.SaveTypeIdentifier.Enemy2;

            movingRight = false;
            distanceRemaining = WalkDistance;
            movementSpeed = standardMovementSpeed;
            Damage = 1f;
        }

        public Enemy2(System.IO.BinaryReader r)
            :base(r)
        {
            // Other
            identifier = SaveFileManager.SaveTypeIdentifier.Enemy2;
            movingRight = false;
            distanceRemaining = WalkDistance;
            movementSpeed = standardMovementSpeed;
            Damage = 1f;
        }

        public override void Update(GameTime gameTime)
        {
            MovementUpdate();

            InputUpdate();

            AnimationUpdate(gameTime);
        }

        public override void GetSaveData(System.IO.BinaryWriter w)
        {
            // Base data
            base.GetSaveData(w);
        }

        protected override void MovementUpdate()
        {
            previousPosition = position;

            position += speed;

            distanceRemaining -= Math.Abs(position.X - previousPosition.X);
            if (distanceRemaining <= 0)
            {
                movingRight = !movingRight;
                distanceRemaining = WalkDistance;
            }

            speed.Y += PhysicsHelper.Gravity;
            if (position.X < origin.X)
                position.X = origin.X;

            foreach (Sprite sprite in Game1.level.Sprites)
            {
                Platform platform = sprite as Platform;
                if (platform != null)
                {
                    if (PerPixelHits(platform))
                    {
                        if (platform.layer == Level.BaineLayer)
                        {
                            friction = platform.material.friction;

                            #region Multisample
                            Vector2 diff = position - previousPosition;
                            int iterations = (int)speed.Length() + 5;
                            bool done = false;

                            for (int i = 0; i < iterations; i++)
                            {
                                position -= diff / iterations;
                                if (done)
                                    break;
                                if (!PerPixelHits(platform))
                                    done = true;
                            }
                            speed.X *= (1 - friction);
                            speed.Y = 0;
                            #endregion

                            ////Rubber
                            //if (platform.material.ToString() == Material.Preset.Rubber.ToString())
                            //{
                            //    speed.Y = -baseJumpStrength * jumpModifier * Math.Sign(speed.Y);
                            //    speed.X += friction * Math.Sign(speed.X);
                            //    hasDoubleJumped = false;
                            //    Library.sounds["Jump"].Play(1, (float)Game1.random.NextDouble(), 0);
                            //    touchesGround = false;
                            //    stance = Stance.Jumping;
                            //}

                            #region Unstuck
                            if (PerPixelHits(platform)) // If still hitting then stuck so move out
                            {
                                Console.WriteLine("Stuck!");
                                diff = position - platform.Position;
                                float pixelsToCenterOfObject = diff.Length();
                                diff.Normalize();
                                diff *= 2; // Move max 2 pixels per frame
                                iterations = 5;
                                done = false;
                                for (int i = 0; i < iterations; i++)
                                {
                                    position += diff / iterations;
                                    if (done)
                                        break;
                                    if (!PerPixelHits(platform))
                                        done = true;
                                }
                                speed = Vector2.Zero;
                            }
                            #endregion
                        }
                    }
                }

                previousSpeed = speed;

                if ((speed.Y == 0 && previousSpeed.Y == 0))
                {
                    touchesGround = true;
                }
                if (Math.Abs(speed.Y) > 2)
                {
                    touchesGround = false;
                }
            }
        }

        protected override void InputUpdate()
        {
            if (touchesGround)
            {
                if (movingRight)
                    speed.X += ((movementSpeed / 2) + (movementSpeed * friction));
                else
                    speed.X -= ((movementSpeed / 2) + (movementSpeed * friction));
            //    if (Input.Key_Down(leftKey)) // Ground control left
            //    {
            //        if (speed.X >= -maxHorizontalSpeed)
            //            speed.X -= (movementSpeed / 2) + (movementSpeed * friction);
            //        stance = Stance.Walking;
            //        flip = SpriteEffects.FlipHorizontally;
            //    }
            //    if (Input.Key_Down(rightKey)) // Ground control right
            //    {
            //        if (speed.X <= maxHorizontalSpeed)
            //            speed.X += (movementSpeed / 2) + (movementSpeed * friction);
            //        stance = Stance.Walking;
            //        flip = SpriteEffects.None;
            //    }
            //    if (Input.Key_Down(crouchKey))
            //    {
            //        if (isOnLadder) // Climb down
            //        {
            //            speed.Y = 4;
            //        }
            //        else // Crouch
            //        {
            //            stance = Stance.Crouching;
            //            if (previousStance != Stance.Crouching)
            //                position.Y += (standingOrigin.Y - crouchOrigin.Y) - 1;
            //        }
            //    }
            //    if (Input.Key_Down(Keys.W))
            //    {
            //        if (isOnLadder) // Climb up
            //        {
            //            speed.Y = -4f;
            //        }
            //    }
            //    if (Input.Key_NewDown(jumpKey))
            //    {
            //        if (stance == Stance.Crouching) // Crouch jump
            //        {
            //            speed.Y = -baseJumpStrength * 1.8f * jumpModifier;
            //            hasDoubleJumped = true;
            //        }
            //        else // Normal jump
            //        {
            //            speed.Y = -baseJumpStrength * jumpModifier;
            //            hasDoubleJumped = false;
            //        }
            //        speed.X += friction * Math.Sign(speed.X);
            //        Library.sounds["Jump"].Play(1,(float)Game1.random.NextDouble(),0);
            //        touchesGround = false;
            //        stance = Stance.Jumping;
            //    }
            //}
            //else
            //{
            //    stance = Stance.Jumping;
            //    if (Input.Key_Down(leftKey)) // Air control left
            //    {
            //        if (speed.X >= -maxHorizontalSpeed)
            //            speed.X -= movementSpeed / 2;
            //        flip = SpriteEffects.FlipHorizontally;
            //    }
            //    if (Input.Key_Down(rightKey)) // Air control right
            //    {
            //        if (speed.X <= maxHorizontalSpeed)
            //            speed.X += movementSpeed / 2;
            //        flip = SpriteEffects.None;
            //    }
            //    if (Input.Key_NewDown(jumpKey) && !hasDoubleJumped) // Double jump
            //    {
            //        speed.Y = -baseJumpStrength;
            //        hasDoubleJumped = true;
            //    }
            }
        }

        protected override void AnimationUpdate(GameTime gameTime)
        {
            // Animation
            //switch (stance)
            //{
            //    case Stance.Idle:
            //        baseTexture = idle;
            //        origin = idleOrigin;

            //        // Turn around twice
            //        if (turnCountdown > 0)
            //        {
            //            turnCountdown -= gameTime.ElapsedGameTime.Milliseconds;
            //        }
            //        if (turnCountdown <= 0)
            //        {
            //            if (!hasTurned)
            //            {
            //                if (flip == SpriteEffects.None)
            //                    flip = SpriteEffects.FlipHorizontally;
            //                else
            //                    flip = SpriteEffects.None;
            //                turnCountdown = 500;
            //                hasTurned = true;
            //            }
            //            else if (!hasTurnedTwice)
            //            {
            //                if (flip == SpriteEffects.None)
            //                    flip = SpriteEffects.FlipHorizontally;
            //                else
            //                    flip = SpriteEffects.None;
            //                hasTurnedTwice = true;
            //            }
            //        }
            //        break;
            //    case Stance.Standing:
            //        baseTexture = standing;
            //        origin = standingOrigin;
            //        movementSpeed = standardMovementSpeed;
            //        idleCountdown -= gameTime.ElapsedGameTime.Milliseconds;
            //        if (idleCountdown <= 0)
            //        {
            //            hasTurned = false;
            //            hasTurnedTwice = false;
            //            turnCountdown = milliSecondsTilTurn;
            //            stance = Stance.Idle;
            //        }
            //        break;
            //    case Stance.Walking:
            //        baseTexture = standing;
            //        origin = standingOrigin;
            //        movementSpeed = standardMovementSpeed;
            //        break;
            //    case Stance.Crouching:
            //        baseTexture = crouching;
            //        origin = crouchOrigin;
            //        movementSpeed = crouchingMovementSpeed;
            //        break;
            //    case Stance.Jumping:
            //        baseTexture = jumping;
            //        origin = jumpingOrigin;
            //        break;
            //    case Stance.Climbing:
            //        baseTexture = climbing;
            //        origin = climbingOrigin;
            //        break;
            //}

            //if (stance != Stance.Idle && stance != Stance.Standing)
            //{
            //    idleCountdown = milliSecondsTilIdle;
            //}
            //if (stance != Stance.Crouching && previousStance == Stance.Crouching)
            //{
            //    position.Y -= (standingOrigin.Y - crouchOrigin.Y);
            //}
            //if (stance != previousStance)
            //{
            //    Console.WriteLine(ToString() + " changed stance to " + stance.ToString());
            //    needsRedraw = true;
            //}
            if (previousFlip != flip)
            {
                Console.WriteLine(ToString() + " flipped direction to " + (flip.ToString() == "None" ? "right" : "left"));
                needsRedraw = true;
            }

            //color = Color.Multiply(Color.White, (health / 100));
        }

        public override string ToString()
        {
            return "Ghost";
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Color newColor = Color.Multiply(color, 1 - layer);
            spriteBatch.Draw(texture, position, null, new Color(newColor.R, newColor.G, newColor.B, 255), 0, origin, 1, SpriteEffects.None, 1);
        }
    }
}
