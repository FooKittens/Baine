using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MajgEngine;

namespace Baine
{
    class Game1 : Microsoft.Xna.Framework.Game
    {
        public static GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static Cursor cursor;
        public static Random random;
        SoundEffect song;
        SoundEffectInstance songLoop;
        public static Level level;
        public static Camera camera;
        public static Camera HUDCamera;
        public static LevelEditor levelEditor;
        public static Baine baine;
        public static SaveFileManager saver;
        Menu menu;
        public static bool gameIsStarted;
        public static int currentLevel;
        ParallaxBackground parallaxClouds;
        ParallaxBackground parallaxMoon;

        public static GameState currentState;
        public enum GameState
        {
            MainMenu,
            Initialize,
            InGame,
            Editor,
            Exit,
            GameOver
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            gameIsStarted = false;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Library.LoadContent(Content);
            random = new Random();
            cursor = new Cursor();
            level = new Level();
            levelEditor = new LevelEditor();
            saver = new SaveFileManager();
            saver.LoadLevel("level1");
            camera = new Camera(new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height));
            HUDCamera = new Camera(new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height));
            camera.CenterOn(level.startPos);
            HUDCamera.CenterOn(level.startPos);
            camera.Update();
            HUDCamera.Update();
            menu = new Menu();
            currentState = GameState.MainMenu;
            parallaxClouds = new ParallaxBackground(Vector2.Zero, Library.textures["Clouds"], 0.5f);
            parallaxMoon = new ParallaxBackground(new Vector2(700, 20), Library.textures["Moon"], 0.4f);
            //graphics.PreferredBackBufferHeight = (int)(graphics.PreferredBackBufferWidth / GraphicsDevice.DisplayMode.AspectRatio);
            if(GraphicsDevice.DisplayMode.AspectRatio == (8/6))
                graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            song = Library.sounds["BaineSong"];
            songLoop = song.CreateInstance();
            songLoop.IsLooped = true;
            songLoop.Play();
        }

        protected override void UnloadContent()
        {
        }

        public static void ReloadLevel()
        {
            saver.LoadLevel("level" + currentLevel);
        }

        protected override void Update(GameTime gameTime)
        {
            Input.Update();
            cursor.Update();
            camera.Update();
            if (Input.GP_IsPressed(PlayerIndex.One, Buttons.Back))
                currentState = GameState.Exit;

            switch (currentState)
            {
                case GameState.MainMenu:
                    menu.Update();
                    break;
                case GameState.Initialize:
                    Game1.baine = new Baine(Game1.level.startPos, "BaineStanding", Level.BaineLayer);
                    currentState = GameState.InGame;
                    break;
                case GameState.InGame:
                    baine.Update(gameTime);
                    level.Update(gameTime);
                    if (Input.Key_NewDown(Keys.Escape))
                        currentState = GameState.MainMenu;
                    break;
                case GameState.Editor:
                    levelEditor.Update();
                    break;
                case GameState.Exit:
                    SaveFileManager saver = new SaveFileManager();
                    this.Exit();
                    break;
                case GameState.GameOver:
                    if (Input.Key_AnyNewKey())
                        currentState = GameState.MainMenu;
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // Redraw what needs redraw
            foreach (Sprite sprite in level.Sprites)
            {
                if (sprite.needsRedraw)
                    sprite.Redraw(spriteBatch, GraphicsDevice);
            }
            Sprite s = levelEditor.CurrentSprite;
            if (s != null && s.needsRedraw)
                s.Redraw(spriteBatch, GraphicsDevice);
            if (baine != null && baine.needsRedraw)
                baine.Redraw(spriteBatch, GraphicsDevice);

            GraphicsDevice.Clear(Color.Black);
            // Draw Game
            switch (currentState)
            {
                case GameState.MainMenu:
                    DrawParallax(spriteBatch, camera);

                    spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, HUDCamera.Transform);
                    level.Draw(spriteBatch, HUDCamera);
                    spriteBatch.End();
                    spriteBatch.Begin();
                    menu.Draw(spriteBatch);
                    spriteBatch.End();
                    break;
                case GameState.Initialize:
                    DrawParallax(spriteBatch, camera);

                    spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, HUDCamera.Transform);
                    level.Draw(spriteBatch, HUDCamera);
                    spriteBatch.End();
                    break;
                case GameState.InGame:
                    DrawParallax(spriteBatch, camera);

                    spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, camera.Transform);
                    level.Draw(spriteBatch, camera);
                    baine.Draw(spriteBatch);
                    spriteBatch.End();
                    break;
                case GameState.Editor:
                    DrawParallax(spriteBatch, camera);

                    spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, camera.Transform);
                    level.Draw(spriteBatch, camera);
                    levelEditor.Draw(spriteBatch);
                    spriteBatch.End();
                    break;
                case GameState.Exit:
                    break;
                case GameState.GameOver:
                    DrawGameOverScreen(spriteBatch, camera);
                    break;
            }

            DrawHUD();

            base.Draw(gameTime);
        }


        #region Drawing Methods
        /// <summary>
        /// Draws the game HUD
        /// </summary>
        private void DrawHUD()
        {
            spriteBatch.Begin();
            if (currentState == GameState.InGame)
            {
                spriteBatch.DrawString(Library.fonts["SmallFont"], "Health: " + baine.health.ToString() + "\nLives: " + baine.LivesLeft, Vector2.Zero, Color.White);
                if (baine.isHidden)
                    spriteBatch.Draw(Library.textures["InvisibleIndicator"], new Vector2(40, graphics.PreferredBackBufferHeight - 40), null, Color.White, 0, new Vector2(21, 11), 1, SpriteEffects.None, 1);
                else
                    spriteBatch.Draw(Library.textures["VisibleIndicator"], new Vector2(40, graphics.PreferredBackBufferHeight - 40), null, Color.White, 0, new Vector2(21, 11), 1, SpriteEffects.None, 1);
            }
            else if (currentState == GameState.Editor)
            {
                levelEditor.DrawHUD(spriteBatch);
            }

            cursor.Draw(spriteBatch);
            spriteBatch.End();
        }

        /// <summary>
        /// Draws the game over text over the parallax
        /// </summary>
        private void DrawGameOverScreen(SpriteBatch spriteBatch, Camera camera)
        {
            DrawParallax(spriteBatch, camera);

            spriteBatch.Begin();
            Texture2D gameOverTexture = Library.textures["GameOver"];
            Vector2 drawPosition = new Vector2(graphics.PreferredBackBufferWidth / 2 - gameOverTexture.Width / 2, graphics.PreferredBackBufferHeight / 2 - gameOverTexture.Height / 2);
            spriteBatch.Draw(gameOverTexture, drawPosition, Color.White);
            spriteBatch.End();
        }

        /// <summary>
        /// Draws clouds and moon according to camera position
        /// </summary>
        private void DrawParallax(SpriteBatch spriteBatch, Camera camera)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.LinearWrap, null, null);
            parallaxMoon.Draw(spriteBatch, camera, false);
            parallaxClouds.Draw(spriteBatch, camera, true);
            spriteBatch.End();
        }
        #endregion
    }
}
