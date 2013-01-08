using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MajgEngine;

namespace Baine
{
    class Baine : MovingSprite
    {
        // Status
        public int LivesLeft { get; protected set; }

        // Movement
        const float crouchingMovementSpeed = 0.6f;
        float baseJumpStrength = 10;
        float jumpModifier;
        bool hasDoubleJumped;
        public float health { get; private set; }
        bool isOnLadder;
        Rectangle hideRectangle;
        public bool isHidden { get; protected set; }
        protected const float standardMovementSpeed = 2;

        // Textures
        Texture2D idle;
        Vector2 idleOrigin;
        Texture2D standing;
        Vector2 standingOrigin;
        Texture2D crouching;
        Vector2 crouchOrigin;
        Texture2D jumping;
        Vector2 jumpingOrigin;
        Texture2D climbing;
        Vector2 climbingOrigin;

        // Animation
        const int milliSecondsTilIdle = 3000;
        int idleCountdown;
        const int milliSecondsTilTurn = 1000;
        int turnCountdown;
        bool hasTurned;
        bool hasTurnedTwice;

        // Controls
        Keys leftKey = Keys.A;
        Keys rightKey = Keys.D;
        Keys jumpKey = Keys.Space;
        Keys crouchKey = Keys.S;

        Stance stance;
        Stance previousStance;
        enum Stance : byte
        {
            Idle,
            Standing,
            Walking,
            Crouching,
            Jumping,
            Climbing
        }

        public Baine(Vector2 position, string textureKey, float layer)
            : base(position, textureKey, layer)
        {
            touchesGround = false;
            hasDoubleJumped = false;
            isOnLadder = false;
            health = 100;
            LivesLeft = 3;

            idle = Library.textures["BaineIdle"];
            idleOrigin = new Vector2(idle.Width / 2, idle.Height / 2);
            standing = Library.textures["BaineStanding"];
            standingOrigin = new Vector2(standing.Width / 2, standing.Height / 2);
            crouching = Library.textures["BaineCrouching"];
            crouchOrigin = new Vector2(crouching.Width / 2, crouching.Height / 2);
            jumping = Library.textures["BaineJumping"];
            jumpingOrigin = new Vector2(jumping.Width / 2, jumping.Height / 2);
            climbing = Library.textures["BaineClimbing"];
            climbingOrigin = new Vector2(climbing.Width / 2, climbing.Height / 2);
            identifier = SaveFileManager.SaveTypeIdentifier.Baine;
            this.layer = Level.BaineLayer;
        }

        public Baine(System.IO.BinaryReader r)
            :base(r)
        {
            // Get Baine-data
            hasDoubleJumped = r.ReadBoolean();
            isOnLadder = r.ReadBoolean();
            health = r.ReadSingle();
            stance = (Stance)r.ReadByte();
            isHidden = r.ReadBoolean();
            LivesLeft = r.ReadInt32();

            // Other
            idle = Library.textures["BaineIdle"];
            idleOrigin = new Vector2(idle.Width / 2, idle.Height / 2);
            standing = Library.textures["BaineStanding"];
            standingOrigin = new Vector2(standing.Width / 2, standing.Height / 2);
            crouching = Library.textures["BaineCrouching"];
            crouchOrigin = new Vector2(crouching.Width / 2, crouching.Height / 2);
            jumping = Library.textures["BaineJumping"];
            jumpingOrigin = new Vector2(jumping.Width / 2, jumping.Height / 2);
            climbing = Library.textures["BaineClimbing"];
            climbingOrigin = new Vector2(climbing.Width / 2, climbing.Height / 2);
            identifier = SaveFileManager.SaveTypeIdentifier.Baine;
            this.layer = Level.BaineLayer;
        }

        public override void Update(GameTime gameTime)
        {
            Game1.camera.CenterOn(position);

            MovementUpdate();

            InputUpdate();

            AnimationUpdate(gameTime);
        }

        public override void GetSaveData(System.IO.BinaryWriter w)
        {
            // Base data
            base.GetSaveData(w);

            // Other stuff
            w.Write((bool)hasDoubleJumped);
            w.Write((bool)isOnLadder);
            w.Write((float)health);
            w.Write((byte)stance);
            w.Write((bool)isHidden);
            w.Write((int)LivesLeft);
        }

        protected override void MovementUpdate()
        {
            isHidden = false;
            isOnLadder = false;
            previousFlip = flip;
            previousPosition = position;
            previousStance = stance;
            
            position += speed;
            speed.Y += PhysicsHelper.Gravity;
            if (position.X < standingOrigin.X)
                position.X = standingOrigin.X;

            Vector2 goal = Game1.level.goalPos;
            if (Bounds.Contains((int)goal.X, (int)goal.Y))
            {
                Game1.saver.LoadLevel("level" + ++Game1.currentLevel);
                position = Game1.level.startPos;
                speed = Vector2.Zero;
                return;
            }

            foreach (Sprite sprite in Game1.level.Sprites)
            {
                if (sprite.layer > layer && sprite.Bounds.Contains(hideRectangle))
                {
                    isHidden = true;
                }
                Platform platform = sprite as Platform;
                if (platform != null)
                {
                    if (PerPixelHits(platform))
                    {
                        if (platform.IsLadder)
                        {
                            if (Input.Key_Down(Keys.W) || stance == Stance.Climbing)
                            {
                                isOnLadder = true;
                                if (Math.Abs(speed.Y) > 0)
                                    speed.Y = 0;
                                speed.X *= (1 - friction);
                            }
                        }
                        if (platform.layer == Level.BaineLayer)
                        {
                            jumpModifier = platform.material.jumpStrenghtModifier;
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
                            if (stance != Stance.Idle)
                                stance = Stance.Standing;

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

                            Trap trap = platform as Trap;
                            if (trap != null)
                            {
                                health -= trap.damage;
                                needsRedraw = true;
                            }
                        }
                    }
                }

                if (sprite is HealthPotion && PerPixelHits(sprite))
                {
                    DrinkPotion(sprite);
                }

                previousSpeed = speed;

                if ((speed.Y == 0 && previousSpeed.Y == 0))
                {
                    touchesGround = true;
                }
                if (Math.Abs(speed.Y) > 2)
                {
                    if (isOnLadder)
                    {
                        stance = Stance.Jumping;
                        touchesGround = false;
                    }
                }
            }
            hideRectangle = new Rectangle((int)position.X - (int)(origin.X), (int)position.Y - 15, (int)origin.X * 2, 8);

            if (!isHidden)
            {
                foreach (MovingSprite movingSprite in Game1.level.MovingSprites)
                {
                    Enemy2 enemy = movingSprite as Enemy2;
                    if (enemy != null && PerPixelHits(enemy))
                    {
                        health -= enemy.Damage;
                        needsRedraw = true;
                    }
                }
            }
            if (position.Y + origin.Y > Game1.graphics.PreferredBackBufferHeight || health <= 0)
            {
                if (LivesLeft > 0)
                {
                    Game1.ReloadLevel();
                    position = Game1.level.startPos;
                    LivesLeft -= 1;
                    health = 100;
                    speed = Vector2.Zero;
                }
                else
                {
                    Game1.currentState = Game1.GameState.GameOver;
                    Game1.gameIsStarted = false;
                }
                return;
            }
        }

        private void DrinkPotion(Sprite sprite)
        {
            sprite.IsDead = true;
            health += 33;
            health = MathHelper.Clamp(health, 0, 100);
            Library.sounds["Nom"].Play();
            needsRedraw = true;
            Console.WriteLine(ToString() + " healed to " + health.ToString() + " hp.");
        }

        protected override void InputUpdate()
        {
            if (touchesGround)
            {
                if (Input.Key_Down(leftKey)) // Ground control left
                {
                    if (speed.X >= -maxHorizontalSpeed)
                        speed.X -= ((movementSpeed / 2) + (movementSpeed * friction));
                    stance = Stance.Walking;
                    flip = SpriteEffects.FlipHorizontally;
                }
                if (Input.Key_Down(rightKey)) // Ground control right
                {
                    if (speed.X <= maxHorizontalSpeed)
                        speed.X += ((movementSpeed / 2) + (movementSpeed * friction));
                    stance = Stance.Walking;
                    flip = SpriteEffects.None;
                }
                if (Input.Key_Down(crouchKey))
                {
                    if (isOnLadder) // Climb down
                    {
                        speed.Y = 4;
                    }
                    else // Crouch
                    {
                        stance = Stance.Crouching;
                        if (previousStance != Stance.Crouching)
                            position.Y += (standingOrigin.Y - crouchOrigin.Y) - 1;
                    }
                }
                if (Input.Key_Down(Keys.W))
                {
                    if (isOnLadder) // Climb up
                    {
                        speed.Y = -4f;
                    }
                }
                if (Input.Key_NewDown(jumpKey))
                {
                    if (stance == Stance.Crouching) // Crouch jump
                    {
                        speed.Y = -baseJumpStrength * 1.8f * jumpModifier;
                        hasDoubleJumped = true;
                    }
                    else // Normal jump
                    {
                        speed.Y = -baseJumpStrength * jumpModifier;
                        hasDoubleJumped = false;
                    }
                    speed.X += friction * Math.Sign(speed.X);
                    Library.sounds["Jump"].Play(1,(float)Game1.random.NextDouble(),0);
                    touchesGround = false;
                    stance = Stance.Jumping;
                }
            }
            else
            {
                stance = Stance.Jumping;
                if (Input.Key_Down(leftKey)) // Air control left
                {
                    if (speed.X >= -maxHorizontalSpeed)
                        speed.X -= movementSpeed / 2;
                    flip = SpriteEffects.FlipHorizontally;
                }
                if (Input.Key_Down(rightKey)) // Air control right
                {
                    if (speed.X <= maxHorizontalSpeed)
                        speed.X += movementSpeed / 2;
                    flip = SpriteEffects.None;
                }
                if (Input.Key_NewDown(jumpKey) && !hasDoubleJumped) // Double jump
                {
                    speed.Y = -baseJumpStrength;
                    hasDoubleJumped = true;
                }
            }
            if (isOnLadder)
            {
                movementSpeed = standardMovementSpeed;
                stance = Stance.Climbing;
            }
        }

        protected override void AnimationUpdate(GameTime gameTime)
        {
            // Animation
            switch (stance)
            {
                case Stance.Idle:
                    baseTexture = idle;
                    origin = idleOrigin;

                    // Turn around twice
                    if (turnCountdown > 0)
                    {
                        turnCountdown -= gameTime.ElapsedGameTime.Milliseconds;
                    }
                    if (turnCountdown <= 0)
                    {
                        if (!hasTurned)
                        {
                            if (flip == SpriteEffects.None)
                                flip = SpriteEffects.FlipHorizontally;
                            else
                                flip = SpriteEffects.None;
                            turnCountdown = 500;
                            hasTurned = true;
                        }
                        else if (!hasTurnedTwice)
                        {
                            if (flip == SpriteEffects.None)
                                flip = SpriteEffects.FlipHorizontally;
                            else
                                flip = SpriteEffects.None;
                            hasTurnedTwice = true;
                        }
                    }
                    break;
                case Stance.Standing:
                    baseTexture = standing;
                    origin = standingOrigin;
                    movementSpeed = standardMovementSpeed;
                    idleCountdown -= gameTime.ElapsedGameTime.Milliseconds;
                    if (idleCountdown <= 0)
                    {
                        hasTurned = false;
                        hasTurnedTwice = false;
                        turnCountdown = milliSecondsTilTurn;
                        stance = Stance.Idle;
                    }
                    break;
                case Stance.Walking:
                    baseTexture = standing;
                    origin = standingOrigin;
                    movementSpeed = standardMovementSpeed;
                    break;
                case Stance.Crouching:
                    baseTexture = crouching;
                    origin = crouchOrigin;
                    movementSpeed = crouchingMovementSpeed;
                    break;
                case Stance.Jumping:
                    baseTexture = jumping;
                    origin = jumpingOrigin;
                    break;
                case Stance.Climbing:
                    baseTexture = climbing;
                    origin = climbingOrigin;
                    break;
            }

            if (stance != Stance.Idle && stance != Stance.Standing)
            {
                idleCountdown = milliSecondsTilIdle;
            }
            if (stance != Stance.Crouching && previousStance == Stance.Crouching)
            {
                position.Y -= (standingOrigin.Y - crouchOrigin.Y);
            }
            if (stance != previousStance)
            {
                Console.WriteLine(ToString() + " changed stance to " + stance.ToString());
                needsRedraw = true;
            }
            if (previousFlip != flip)
            {
                Console.WriteLine(ToString() + " flipped direction to " + (flip.ToString() == "None" ? "right" : "left"));
                needsRedraw = true;
            }

            color = Color.Multiply(Color.White, (health / 100));
        }

        public override string ToString()
        {
            return "Mr. Baine";
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
