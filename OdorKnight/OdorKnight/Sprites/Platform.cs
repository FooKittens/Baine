using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MajgEngine;

namespace Baine
{
    [Serializable]
    class Platform : Sprite
    {
        public Material material { get; protected set; }
        Material.Preset materialPreset;
        public bool IsLadder { get; protected set; }

        public Platform(Vector2 position, string textureKey, float layer, bool isLadder, Material.Preset materialPreset)
            : base(position, textureKey, layer)
        {
            this.materialPreset = materialPreset;
            origin = new Vector2((baseTexture.Width / 2) - ((baseTexture.Width / 2) % 8), (baseTexture.Height / 2) - ((baseTexture.Height / 2) % 8));
            SetMaterial(materialPreset);
            identifier = SaveFileManager.SaveTypeIdentifier.Platform;
            IsLadder = isLadder;
        }
        public Platform(System.IO.BinaryReader r)
            : base(r)
        {
            // 7 Set material
            materialPreset = (Material.Preset)r.ReadByte();

            // 8 Set IsLadder
            IsLadder = r.ReadBoolean();

            // Set other data
            origin = new Vector2((baseTexture.Width / 2) - ((baseTexture.Width / 2) % 8), (baseTexture.Height / 2) - ((baseTexture.Height / 2) % 8));
            SetMaterial(materialPreset);
            identifier = SaveFileManager.SaveTypeIdentifier.Platform;
        }

        public void SetIsLadder(bool ladder)
        {
            IsLadder = ladder;
        }

        public void SetMaterial(Material.Preset materialPreset)
        {
            this.materialPreset = materialPreset;
            material = new Material(materialPreset);
            baseTexture = Repainter.GetTextureCopy(Library.textures[textureKey]);
            Repainter.ReplaceRGB(ref baseTexture, material);
            needsRedraw = true;
        }

        public override string ToString()
        {
            if (material != null && textureKey != null)
                return textureKey.TrimStart('_') + " " + material.ToString();
            return "Platform" + GetHashCode().ToString();
        }

        public override void GetSaveData(System.IO.BinaryWriter w)
        {
            // Step 1-6
            base.GetSaveData(w);

            // 7 Save material
            w.Write((byte)materialPreset);

            // 8 Save IsLadder
            w.Write((bool)IsLadder);
        }

        public override void Redraw(SpriteBatch sb, GraphicsDevice GraphicsDevice)
        {
            base.Redraw(sb, GraphicsDevice);
            origin = new Vector2((baseTexture.Width / 2) - ((baseTexture.Width / 2) % 8), (baseTexture.Height / 2) - ((baseTexture.Height / 2) % 8));
        }
    }
}