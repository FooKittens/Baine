using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using MajgEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Baine
{
    class LevelEditor
    {
        public Sprite CurrentSprite { get; private set; }
        Material.Preset currentMaterial;
        public RadialMenu TileMenu { get; private set; }
        public RadialMenu TrapMenu { get; private set; }
        public RadialMenu MaterialMenu { get; private set; }
        public RadialMenu PotionMenu { get; private set; }
        public RadialMenu EnemyMenu { get; private set; }
        bool snapToGrid;
        bool isCurrentSpriteAlreadyInLevel;
        Vector2 currentPosition;
        Vector2 gridSize;
        float currentLayer;
        bool isLadder;
        public int CurrentLevel { get; set; }

        public LevelEditor()
        {
            CurrentLevel = 1;
            snapToGrid = true;
            gridSize = new Vector2(8);
            currentMaterial = Material.Preset.Dirt;
            currentLayer = Level.BaineLayer;
            isLadder = false;

            // Create tile selection menu
            List<RadialMenu.Function> tileFunctions = new List<RadialMenu.Function>();
            List<Texture2D> tileTextures = new List<Texture2D>();
            tileFunctions.Add(TilesNull);
            tileTextures.Add(Library.textures["Cursor"]);
            for (int i = 0; i < (int)Level.PlatformType.COUNT; i++)
            {
                Texture2D t;
                t = Repainter.GetTextureCopy(Library.textures[((Level.PlatformType)i).ToString()]);
                Repainter.ReplaceRGB(ref t, Color.Gray, Color.LightGray, Color.DarkGray);
                tileTextures.Add(t);
                tileFunctions.Add(TilesSelect);
            }

            TileMenu = new RadialMenu(tileFunctions, tileTextures, true, TilesOpen, TilesClose);

            // Create material selection menu
            List<RadialMenu.Function> materialFunctions = new List<RadialMenu.Function>();
            List<Texture2D> materialTextures = new List<Texture2D>();

            Texture2D temp8x8 = Library.textures["MaterialPreview"];
            for (int i = 0; i < (int)Material.Preset.COUNT; i++)
            {
                Texture2D t;
                t = Repainter.GetTextureCopy(temp8x8);
                Repainter.ReplaceRGB(ref t, new Material((Material.Preset)i));
                materialTextures.Add(t);
                materialFunctions.Add(MaterialSelect);
            }

            MaterialMenu = new RadialMenu(materialFunctions, materialTextures, true, MaterialsOpen, MaterialsClose);

            // Create trap selection menu
            List<RadialMenu.Function> trapFunctions = new List<RadialMenu.Function>();
            List<Texture2D> trapTextures = new List<Texture2D>();

            for (int i = 0; i < (int)Level.TrapType.COUNT; i++)
            {
                Texture2D t;
                t = Repainter.GetTextureCopy(Library.textures[((Level.TrapType)i).ToString()]);
                Repainter.ReplaceRGB(ref t, Color.Gray, Color.LightGray, Color.DarkGray);
                trapTextures.Add(t);
                trapFunctions.Add(TrapSelect);
            }

            TrapMenu = new RadialMenu(trapFunctions, trapTextures, true, TrapsOpen, TrapsClose);

            // Create potion selection menu
            List<RadialMenu.Function> potionFunctions = new List<RadialMenu.Function>();
            List<Texture2D> potionTextures = new List<Texture2D>();

            for (int i = 0; i < (int)Level.PotionType.COUNT; i++)
            {
                Texture2D t;
                t = Library.textures[((Level.PotionType)i).ToString()];
                potionTextures.Add(t);
                potionFunctions.Add(HealthPotionSelect);
            }

            PotionMenu = new RadialMenu(potionFunctions, potionTextures, true, PotionsOpen, PotionsClose);

            // Create enemy selection menu
            List<RadialMenu.Function> enemyFunctions = new List<RadialMenu.Function>();
            List<Texture2D> enemyTextures = new List<Texture2D>();

            enemyTextures.Add(Library.textures["Enemy2Moving"]);
            enemyFunctions.Add(EnemySelect);

            EnemyMenu = new RadialMenu(enemyFunctions, enemyTextures, true, EnemiesOpen, EnemiesClose);
        }

        #region Sprite selection menu
        private bool TilesOpen()
        {
            return Input.Mouse_RightNewDown();
        }
        private bool TilesClose()
        {
            return Input.Mouse_RightNewUp();
        }
        private void TilesNull()
        {
            CurrentSprite = null;
        }
        private void TilesSelect()
        {
            CurrentSprite = new Platform(currentPosition, ((Level.PlatformType)(TileMenu.GetIndex() - 1)).ToString(), currentLayer, isLadder, currentMaterial);
            isCurrentSpriteAlreadyInLevel = false;
        }

        private bool TrapsOpen()
        {
            return Input.Key_NewDown(Keys.T);
        }
        private bool TrapsClose()
        {
            return Input.Key_Release(Keys.T);
        }
        private void TrapSelect()
        {
            CurrentSprite = new Trap(currentPosition, ((Level.TrapType)(TrapMenu.GetIndex())).ToString(), currentLayer, false, currentMaterial);
            isCurrentSpriteAlreadyInLevel = false;
        }

        private bool PotionsOpen()
        {
            return Input.Key_NewDown(Keys.P);
        }
        private bool PotionsClose()
        {
            return Input.Key_Release(Keys.P);
        }
        private void HealthPotionSelect()
        {
            CurrentSprite = new HealthPotion(currentPosition, ((Level.PotionType)(TrapMenu.GetIndex())).ToString(), currentLayer);
        }

        private bool EnemiesOpen()
        {
            return Input.Key_NewDown(Keys.E);
        }
        private bool EnemiesClose()
        {
            return Input.Key_Release(Keys.E);
        }

        private void EnemySelect()
        {
            CurrentSprite = new Enemy2(currentPosition, "Enemy2Moving", 1);
        }

        #endregion 
        /* To add platforms, simply add image content, load it in the Library class,
         set the key to _sizeOfTexture(_8x8 etc), and add that key in the Level.PlatformType enum. Done! */
        #region Material selection menu
        private bool MaterialsOpen()
        {
            return Input.Key_NewDown(Keys.M);
        }
        private bool MaterialsHold()
        {
            return Input.Key_Down(Keys.M);
        }
        private bool MaterialsClose()
        {
            return Input.Key_Release(Keys.M);
        }
        private void MaterialSelect()
        {
            currentMaterial = (Material.Preset)MaterialMenu.GetIndex();
            Platform p = CurrentSprite as Platform;
            if (p != null)
            {
                p.SetMaterial(currentMaterial);
                CurrentSprite.needsRedraw = true;
            }
        }
        #endregion
        /* To add materials, simply add a material in the Material.Preset enum and modify the switch/case block in the Material constructor! */

        public void Update()
        {
            // Set layer
            if (Input.Key_NewDown(Keys.NumPad0))
                currentLayer = 0.1f;
            if (Input.Key_NewDown(Keys.NumPad1))
                currentLayer = 0.2f;
            if (Input.Key_NewDown(Keys.NumPad2))
                currentLayer = 0.3f;
            if (Input.Key_NewDown(Keys.NumPad3))
                currentLayer = 0.4f;
            if (Input.Key_NewDown(Keys.NumPad4))
                currentLayer = 0.5f;
            if (Input.Key_NewDown(Keys.NumPad5))
                currentLayer = Level.BaineLayer;
            if (Input.Key_NewDown(Keys.NumPad6))
                currentLayer = 0.7f;
            if (Input.Key_NewDown(Keys.NumPad7))
                currentLayer = 0.8f;
            if (Input.Key_NewDown(Keys.NumPad8))
                currentLayer = 0.9f;
            if (Input.Key_NewDown(Keys.NumPad9))
                currentLayer = 1f;
            if (CurrentSprite != null)
                CurrentSprite.layer = currentLayer;

            // Set isLadder
            if (Input.Key_NewDown(Keys.L))
                isLadder = !isLadder;

            // Set cursor position
            if (Input.Key_NewDown(Keys.G))
                snapToGrid = !snapToGrid;
            if (Input.Key_NewDown(Keys.R) && CurrentSprite != null)
                CurrentSprite.Rotate(MathHelper.PiOver2);

            Camera cam = Game1.camera;

            Vector2 mousePosInLevel = cam.TranslatePositionByCamera(Input.Mouse_Position());
            if (snapToGrid)
            {
                currentPosition.X = (int)(mousePosInLevel.X / gridSize.X) * gridSize.X;
                currentPosition.Y = (int)(mousePosInLevel.Y / gridSize.Y) * gridSize.Y;
            }
            else
                currentPosition = mousePosInLevel;



            // Tile modification
            if (CurrentSprite == null)
            {
                if (Input.Mouse_LeftNewDown())
                {
                    foreach (Sprite sprite in Game1.level.Sprites)
                    {
                        if (sprite.Bounds.Contains(new Point((int)mousePosInLevel.X, (int)mousePosInLevel.Y)))
                        {
                            CurrentSprite = sprite;
                            isCurrentSpriteAlreadyInLevel = true;
                            currentLayer = sprite.layer;
                        }
                    }
                    foreach (Sprite sprite in Game1.level.MovingSprites)
                    {
                        if (sprite.Bounds.Contains(new Point((int)mousePosInLevel.X, (int)mousePosInLevel.Y)))
                        {
                            CurrentSprite = sprite;
                            isCurrentSpriteAlreadyInLevel = true;
                            currentLayer = sprite.layer;
                        }
                    }
                }
            }
            else
            {
                CurrentSprite.SetPosition(currentPosition);
                if (Input.Mouse_LeftNewDown())
                {
                    Platform platform = CurrentSprite as Platform;
                    if (platform != null)
                    {
                        platform.SetIsLadder(isLadder);
                        if (isCurrentSpriteAlreadyInLevel) // Do not add duplicate sprites
                            TileMenu.Use(); 
                        else
                        {
                            Game1.level.AddSprite(platform); // Add new sprites
                            TileMenu.Use();
                        }
                    }
                    Trap trap = CurrentSprite as Trap;
                    if (trap != null)
                    {
                        if (isCurrentSpriteAlreadyInLevel) // Do not add duplicate sprites
                            TrapMenu.Use();
                        else
                        {
                            Game1.level.AddSprite(trap); // Add new sprites
                            TrapMenu.Use();
                        }
                    }
                    HealthPotion potion = CurrentSprite as HealthPotion;
                    if (potion != null)
                    {
                        if (isCurrentSpriteAlreadyInLevel) // Do not add duplicate sprites
                            PotionMenu.Use();
                        else
                        {
                            Game1.level.AddSprite(potion); // Add new sprites
                            PotionMenu.Use();
                        }
                    }
                    Enemy2 enemy2 = CurrentSprite as Enemy2;
                    if (enemy2 != null)
                    {
                        if (isCurrentSpriteAlreadyInLevel) // Do not add duplicate sprites
                            PotionMenu.Use();
                        else
                        {
                            Game1.level.AddSprite(enemy2); // Add new sprites
                            PotionMenu.Use();
                        }
                    }
                }
                if (Input.Mouse_RightNewDown() || Input.Key_NewDown(Keys.Delete))
                {
                    if (isCurrentSpriteAlreadyInLevel)
                    {
                        MovingSprite movingSprite = CurrentSprite as MovingSprite;
                        if (movingSprite != null)
                            Game1.level.RemoveSprite(movingSprite);
                        else
                            Game1.level.RemoveSprite(CurrentSprite);
                    }
                    CurrentSprite = null;
                    isCurrentSpriteAlreadyInLevel = false;
                }
            }

            if (Input.Key_Down(Keys.Z))
                Game1.level.startPos = mousePosInLevel;
            if (Input.Key_Down(Keys.X))
                Game1.level.goalPos = mousePosInLevel;

            // Update menus
            TileMenu.Update(Game1.cursor);
            MaterialMenu.Update(Game1.cursor);
            TrapMenu.Update(Game1.cursor);
            PotionMenu.Update(Game1.cursor);
            EnemyMenu.Update(Game1.cursor);

            // Check for level change
            if (Input.Key_NewDown(Keys.F1))
                ChangeToLevel(1);
            if (Input.Key_NewDown(Keys.F2))
                ChangeToLevel(2);
            if (Input.Key_NewDown(Keys.F3))
                ChangeToLevel(3);
            if (Input.Key_NewDown(Keys.F4))
                ChangeToLevel(4);
            if (Input.Key_NewDown(Keys.F5))
                ChangeToLevel(5);
            if (Input.Key_NewDown(Keys.F6))
                ChangeToLevel(6);
            if (Input.Key_NewDown(Keys.F7))
                ChangeToLevel(7);
            if (Input.Key_NewDown(Keys.F8))
                ChangeToLevel(8);
            if (Input.Key_NewDown(Keys.F9))
                ChangeToLevel(9);
            if (Input.Key_NewDown(Keys.F10))
                ChangeToLevel(10);
            if (Input.Key_NewDown(Keys.F11))
                ChangeToLevel(11);
            if (Input.Key_NewDown(Keys.F12))
                ChangeToLevel(12);
            // Check for exit
            if (Input.Key_NewDown(Keys.Escape))
            {
                Game1.saver.SaveLevel("level" + CurrentLevel.ToString());
                Game1.currentState = Game1.GameState.MainMenu;
                Game1.currentLevel = 1;
                Game1.saver.LoadLevel("level1");
            }
        }

        private void ChangeToLevel(int level)
        {
            if (CurrentLevel != level)
            {
                Game1.saver.SaveLevel("level" + CurrentLevel.ToString());
            }
            CurrentLevel = level;
            Game1.saver.LoadLevel("level" + CurrentLevel.ToString());
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if(CurrentSprite != null && !TileMenu.IsOpen)
                CurrentSprite.Draw(spriteBatch);
            spriteBatch.Draw(Library.textures["StartFlag"], Game1.level.startPos, null, Color.White, 0, new Vector2(5, 40), 1, SpriteEffects.None, 1);
        }

        public void DrawHUD(SpriteBatch spriteBatch)
        {
            TileMenu.Draw(spriteBatch);
            TrapMenu.Draw(spriteBatch);
            MaterialMenu.Draw(spriteBatch);
            PotionMenu.Draw(spriteBatch);
            EnemyMenu.Draw(spriteBatch);

            //flagMenu.Draw(spriteBatch);
            Camera cam = Game1.camera;
            string status = "";
            status += "Current level: " + CurrentLevel.ToString();
            status += "\nGrid snapping: " + (snapToGrid ? "On " : "Off ") + "(G)";
            status += "\nCamera position: " + cam.Position.ToString() + " (WASD)";
            status += "\nZoom: " + cam.Scale.ToString() + " (+ / -)";
            status += "\nCurrent sprite: " + (CurrentSprite != null ? CurrentSprite.ToString() : "None") + " (M, R, T, P, Delete, mouse)";
            status += "\nLayer: " + (currentLayer == Level.BaineLayer ? "0,6 (Baine's layer)" : currentLayer.ToString());
            status += "\n" + (isLadder ? "Create as ladder" : "Do not create as ladder") + " (L)";
            status += "\n" + Game1.level.startPos.ToString();
            spriteBatch.DrawString(Library.fonts["SmallFont"], status, Vector2.Zero, Color.White);
        }
    }
}
