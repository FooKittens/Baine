using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Baine
{
    class Trap : Platform
    {
        public float damage { get; protected set; }

        public Trap(Vector2 position, string textureKey, float layer, bool isLadder, Material.Preset materialPreset)
            : base(position, textureKey, layer, isLadder, materialPreset)
        {
            damage = 1f;
            identifier = SaveFileManager.SaveTypeIdentifier.Trap;
        }

        public Trap(System.IO.BinaryReader r)
            : base(r)
        {
            damage = r.ReadSingle();
            identifier = SaveFileManager.SaveTypeIdentifier.Trap;
        }

        public override void GetSaveData(System.IO.BinaryWriter w)
        {
            base.GetSaveData(w);
            w.Write((float)damage);
        }
    }
}
