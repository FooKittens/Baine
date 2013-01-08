using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace MajgEngine
{
    static class Library
    {
        public static Dictionary<string, SpriteFont> fonts { get; private set; }

        public static Dictionary<string, Texture2D> textures { get; private set; }

        public static Dictionary<string, SoundEffect> sounds { get; private set; }

        public static void LoadContent(ContentManager Content)
        {
            #region Fonts
            fonts = new Dictionary<string, SpriteFont>();
            fonts.Add("SmallFont", Content.Load<SpriteFont>(@"Fonts/SmallFont"));
            #endregion

            #region Textures
            textures = new Dictionary<string, Texture2D>();
            // GG
            textures.Add("GameOver", Content.Load<Texture2D>(@"Images/Menu/GameOver"));

            // Engine
            textures.Add("RadialMenuHighlight", Content.Load<Texture2D>(@"Images/Engine/RadialMenuHighlight"));
            textures.Add("RadialMenuItem", Content.Load<Texture2D>(@"Images/Engine/RadialMenuItem"));
            textures.Add("Cursor", Content.Load<Texture2D>(@"Images/Engine/Cursor"));
            textures.Add("Pixel", Content.Load<Texture2D>(@"Images/WhitePixel"));

            // Baine
            textures.Add("BaineStanding", Content.Load<Texture2D>(@"Images/Characters/Baine/Standing"));
            textures.Add("BaineCrouching", Content.Load<Texture2D>(@"Images/Characters/Baine/Crouching"));
            textures.Add("BaineJumping", Content.Load<Texture2D>(@"Images/Characters/Baine/Jump"));
            textures.Add("BaineIdle", Content.Load<Texture2D>(@"Images/Characters/Baine/Idle"));
            textures.Add("BaineClimbing", Content.Load<Texture2D>(@"Images/Characters/Baine/Climbing"));

            // Enemy2
            textures.Add("Enemy2Standing", Content.Load<Texture2D>(@"Images/Characters/Enemy2/Standing"));
            textures.Add("Enemy2Moving", Content.Load<Texture2D>(@"Images/Characters/Enemy2/Moving"));

            // Platforms
            textures.Add("MaterialPreview", Content.Load<Texture2D>(@"Images/Platforms/MaterialPreview"));
            textures.Add("_8x8", Content.Load<Texture2D>(@"Images/Platforms/_8x8"));
            textures.Add("_3x1", Content.Load<Texture2D>(@"Images/Platforms/_3x1"));
            textures.Add("_8x1", Content.Load<Texture2D>(@"Images/Platforms/_8x1"));
            textures.Add("_10x1", Content.Load<Texture2D>(@"Images/Platforms/_10x1"));
            textures.Add("_16x16", Content.Load<Texture2D>(@"Images/Platforms/_16x16"));

            // Menu buttons
            textures.Add("ExitButton", Content.Load<Texture2D>(@"Images/Menu/Exit"));
            textures.Add("OptionsButton", Content.Load<Texture2D>(@"Images/Menu/Options"));
            textures.Add("EditorButton", Content.Load<Texture2D>(@"Images/Menu/Editor"));
            textures.Add("SaveButton", Content.Load<Texture2D>(@"Images/Menu/Save"));
            textures.Add("LoadButton", Content.Load<Texture2D>(@"Images/Menu/Load"));
            textures.Add("NewButton", Content.Load<Texture2D>(@"Images/Menu/New"));
            textures.Add("ContinueButton", Content.Load<Texture2D>(@"Images/Menu/Continue"));
            textures.Add("EndButton", Content.Load<Texture2D>(@"Images/Menu/End"));

            // Traps
            textures.Add("Spikes", Content.Load<Texture2D>(@"Images/Traps/Spikes"));

            // Flags
            textures.Add("StartFlag", Content.Load<Texture2D>(@"Images/Flags/Start"));
            textures.Add("GoalFlag", Content.Load<Texture2D>(@"Images/Flags/Goal"));

            // Items
            textures.Add("HealthPotion", Content.Load<Texture2D>(@"Images/Items/HealthPotion"));

            // HUD
            textures.Add("VisibleIndicator", Content.Load<Texture2D>(@"Images/HUD/Visible"));
            textures.Add("InvisibleIndicator", Content.Load<Texture2D>(@"Images/HUD/Invisible"));

            // Background
            textures.Add("Clouds", Content.Load<Texture2D>(@"Images/Background/clouds"));
            textures.Add("Moon", Content.Load<Texture2D>(@"Images/Background/Moon"));
            textures.Add("Star1", Content.Load<Texture2D>(@"Images/Background/Star1"));
            textures.Add("Star2", Content.Load<Texture2D>(@"Images/Background/Star2"));
            #endregion

            #region Sounds
            sounds = new Dictionary<string, SoundEffect>();
            sounds.Add("Footsteps", Content.Load<SoundEffect>(@"Sounds/Footsteps"));
            sounds.Add("Jump", Content.Load<SoundEffect>(@"Sounds/Jump Sound"));
            sounds.Add("Swoosh", Content.Load<SoundEffect>(@"Sounds/Swoosh"));
            sounds.Add("Nom", Content.Load<SoundEffect>(@"Sounds/Nom"));
            sounds.Add("BaineSong", Content.Load<SoundEffect>(@"Sounds/BaineSong"));
            #endregion
        }
    }
}
