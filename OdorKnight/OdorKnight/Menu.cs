using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MajgEngine;
using Microsoft.Xna.Framework.Input;

namespace Baine
{
    class Menu
    {
        private class Button
        {
            Texture2D texture;
            public Vector2 position;
            bool pressed;
            Rectangle bounds;

            public Button(Texture2D texture, Vector2 position)
            {
                this.texture = texture;
                this.position = position;
                bounds = new Rectangle((int)position.X + texture.Bounds.Left, (int)position.Y + texture.Bounds.Top, texture.Width, texture.Height);
                Repainter.ReplaceRGB(ref texture, new Color(230, 230, 230, 200), new Color(215, 215, 215, 200), new Color(64, 64, 64, 200));
            }

            public bool Clicked()
            {
                Vector2 mousePos = Input.Mouse_Position();
                if (bounds.Contains((int)mousePos.X, (int)mousePos.Y))
                {
                    pressed = true;
                    if (Input.Mouse_LeftNewUp())
                        return true;
                }
                else
                    pressed = false;
                return false;
            }

            public void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.Draw(texture, position, pressed ? Color.White : Color.LightGray);
            }
        }

        Button newGameButton;
        Button continueButton;
        Button editorButton;
        Button loadButton;
        Button optionsButton;
        Button saveButton;
        Button exitButton;
        Button endButton;

        public Menu()
        {
            int rightX = Game1.graphics.PreferredBackBufferWidth - 50;
            int bottomY = Game1.graphics.PreferredBackBufferHeight - 50;
            Texture2D texture;

            // Lhs
            newGameButton = new Button(Library.textures["NewButton"], new Vector2(50));
            continueButton = new Button(Library.textures["ContinueButton"], new Vector2(50));
            loadButton = new Button(Library.textures["LoadButton"], new Vector2(50, 150));
            saveButton = new Button(Library.textures["SaveButton"], new Vector2(50, 250));
            
            // Rhs
            texture = Library.textures["EditorButton"];
            editorButton = new Button(texture, new Vector2(rightX - texture.Width, bottomY - texture.Height - 200));
            texture = Library.textures["OptionsButton"];
            optionsButton = new Button(texture, new Vector2(rightX - texture.Width, bottomY - texture.Height - 100));
            texture = Library.textures["ExitButton"];
            exitButton = new Button(texture, new Vector2(rightX - texture.Width, bottomY - texture.Height));
            texture = Library.textures["EndButton"];
            endButton = new Button(texture, new Vector2(rightX - texture.Width, bottomY - texture.Height));
        }

        public void Update()
        {
            if (Game1.gameIsStarted)
            {
                if (continueButton.Clicked() || Input.Key_NewDown(Keys.Escape))
                {
                    Game1.currentState = Game1.GameState.InGame;
                }
                if (saveButton.Clicked())
                {
                    Game1.saver.SaveProgress("testProgress");
                }
                if (endButton.Clicked())
                {
                    Game1.gameIsStarted = false;
                Game1.saver.LoadLevel("level1");
                }
            }
            else
            {
                if (newGameButton.Clicked())
                {
                    Game1.gameIsStarted = true;
                    Game1.currentState = Game1.GameState.Initialize;
                    Game1.currentLevel = 1;
                    Game1.camera.CenterOn(Game1.HUDCamera.Position);
                }
                if (exitButton.Clicked() || Input.Key_NewDown(Keys.Escape))
                    Game1.currentState = Game1.GameState.Exit;
            }
            if (loadButton.Clicked())
            {
                Game1.saver.LoadProgress("testProgress");
                Game1.currentState = Game1.GameState.InGame;
                Game1.gameIsStarted = true;
            }

            if (editorButton.Clicked())
            {
                Game1.saver.LoadLevel("level1");
                Game1.levelEditor.CurrentLevel = 1;
                Game1.currentState = Game1.GameState.Editor;
            }
            if (optionsButton.Clicked())
            {
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Game1.gameIsStarted)
            {
                continueButton.Draw(spriteBatch);
                saveButton.Draw(spriteBatch);
                endButton.Draw(spriteBatch);
            }
            else
            {
                newGameButton.Draw(spriteBatch);
                exitButton.Draw(spriteBatch);
            }
            loadButton.Draw(spriteBatch);
            editorButton.Draw(spriteBatch);
            optionsButton.Draw(spriteBatch);
            
        }
    }
}
