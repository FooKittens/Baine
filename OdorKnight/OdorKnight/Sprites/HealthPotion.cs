using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Baine
{
    class HealthPotion : Sprite
    {
        public HealthPotion(Vector2 position, string textureKey, float layer)
            : base(position, textureKey, layer)
        {
            identifier = SaveFileManager.SaveTypeIdentifier.HealthPotion;
            origin = new Vector2(8, 16);
        }

        public HealthPotion(System.IO.BinaryReader r)
            : base(r)
        {
            identifier = SaveFileManager.SaveTypeIdentifier.HealthPotion;
            origin = new Vector2(8, 16);
        }

        public override string ToString()
        {
            return "Health Potion";
        }
    }
}
