using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Baine
{
    class Material
    {
        public enum Preset : byte
        {
            Dirt,
            Rock,
            Sand,
            Grass,
            Rubber,
            Metal,
            Glass,
            Ice,
            COUNT
        }

        /// <summary>
        /// A value of 1 drains all movement, whereas 0 preserves it
        /// </summary>
        public float friction { get; private set; }
        public float jumpStrenghtModifier { get; private set; }
        /// <summary>
        /// Base color
        /// </summary>
        public Color redReplacement { get; private set; }
        /// <summary>
        /// Brighter color
        /// </summary>
        public Color greenReplacement { get; private set; }
        /// <summary>
        /// Darker color
        /// </summary>
        public Color blueReplacement { get; private set; }
        private string name;

        public Material(Preset preset)
        {
            name = preset.ToString();
            switch (preset)
            {
                case Preset.Dirt:
                    friction = 0.3f;
                    jumpStrenghtModifier = 0.9f;
                    redReplacement = new Color(81, 32, 0);
                    greenReplacement = new Color(127, 88, 62);
                    blueReplacement = new Color(53, 21, 0);
                    break;
                case Preset.Rock:
                    friction = 0.5f;
                    jumpStrenghtModifier = 1;
                    redReplacement = new Color(128, 128, 128);
                    greenReplacement = new Color(175, 175, 175);
                    blueReplacement = new Color(96, 96, 96);
                    break;
                case Preset.Rubber: // wtf placeholder colors
                    friction = 2f;
                    jumpStrenghtModifier = 1.1f;
                    redReplacement = new Color(128, 128, 128);
                    greenReplacement = new Color(137, 137, 174);
                    blueReplacement = new Color(127, 88, 62);
                    break;
                case Preset.Metal: // Even out colors, make slightly less purple
                    friction = 0.4f;
                    jumpStrenghtModifier = 1f;
                    redReplacement = new Color(183, 183, 213);
                    greenReplacement = new Color(228, 240, 255);
                    blueReplacement = new Color(137, 137, 174);
                    break;
                case Preset.Glass: 
                    friction = 0.3f;
                    jumpStrenghtModifier = 1f;
                    redReplacement = new Color(100, 100, 110, 128);
                    greenReplacement = new Color(110, 110, 120, 128);
                    blueReplacement = new Color(90, 90, 100, 128);
                    break;
                case Preset.Sand:
                    friction = 0.65f;
                    jumpStrenghtModifier = 0.8f;
                    redReplacement = new Color(255, 151, 61);
                    greenReplacement = new Color(255, 190, 112);
                    blueReplacement = new Color(255, 120, 30);
                    break;
                case Preset.Grass:
                    friction = 0.2f;
                    jumpStrenghtModifier = 1f;
                    redReplacement = new Color(0, 151, 0);
                    greenReplacement = new Color(0, 182, 0);
                    blueReplacement = new Color(0, 113, 0);
                    break;
                case Preset.Ice:
                    friction = 0.01f;
                    jumpStrenghtModifier = 1;
                    redReplacement = new Color(193, 230, 255);
                    greenReplacement = new Color(255, 255, 255);
                    blueReplacement = new Color(142, 210, 255);
                    break;
            }
        }

        public override string ToString()
        {
            return name;
        }
    }
}
