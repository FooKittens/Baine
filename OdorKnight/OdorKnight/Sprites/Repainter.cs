using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Baine
{
    static class Repainter
    {
        public static void ReplaceRGB(ref Texture2D texture, Material material)
        {
            ReplaceRGB(ref texture, material.redReplacement, material.greenReplacement, material.blueReplacement);
        }

        public static void ReplaceRGB(ref Texture2D texture, Color redReplacement, Color greenReplacement, Color blueReplacement)
        {
            Color[] colors = new Color[texture.Width * texture.Height];
            texture.GetData(colors);
            for (int i = 0; i < colors.Length; i++)
            {
                if (colors[i] == Color.Red)
                    colors[i] = redReplacement;
                if (colors[i] == new Color(0, 255, 0))
                    colors[i] = greenReplacement;
                if (colors[i] == Color.Blue)
                    colors[i] = blueReplacement;
            }
            texture.SetData(colors);
        }

        public static Texture2D GetTextureCopy(Texture2D texture)
        {
            Color[] colors = new Color[texture.Width * texture.Height];
            texture.GetData(colors);

            Texture2D newTexture = new Texture2D(texture.GraphicsDevice, texture.Width, texture.Height);
            newTexture.SetData(colors);
            return newTexture;
        }
    }
}
