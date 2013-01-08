using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MajgEngine;
using Microsoft.Xna.Framework.Input;

namespace MajgEngine
{
    class RadialMenu
    {
        // Items
        List<Item> displayedItems;
        List<Item> allItems;
        int itemCount;
        int index;

        // Control
        public delegate bool MenuControl();
        MenuControl openMenu;
        MenuControl closeMenu;
        bool applyOnChange;
        public bool IsOpen { get; private set; }

        // Function
        public delegate void Function();
        
        // Drawing
        Vector2 position, diff;
        float angle, radius, scale, nullRadius;
        Color fade;

        public RadialMenu(List<Function> functions, List<Texture2D> icons, bool applyOnChange, MenuControl openMenu, MenuControl closeMenu)
        {
            this.openMenu = openMenu;
            IsOpen = false;
            this.closeMenu = closeMenu;
            this.applyOnChange = applyOnChange;
            index = 0;
            itemCount = functions.Count;
            allItems = new List<Item>();
            for (int i = 0; i < itemCount; i++)
			{
                allItems.Add(new Item(i, this, functions[i], icons[i]));
            }
            displayedItems = new List<Item>();
            nullRadius = 20;
            fade = Color.White;
            radius = 9 * itemCount + nullRadius + 12;
        }

        public void Update(Cursor cursor)
        {
            Vector2 mousePos = Input.Mouse_Position();

            if (openMenu())
            {
                position = mousePos;
                
                for (int i = 0; i < itemCount; i++)
                {
                    displayedItems.Add(allItems[i]);
                    displayedItems[i].Open();
                }
                IsOpen = true;
                Use();
            }

            if (IsOpen)
            {
                Vector2 input = Input.GP_LeftThumbstick(PlayerIndex.One);
                if (input != Vector2.Zero)
                {
                    input.Y *= -1;
                    float rotation = (float)Math.Atan2(input.Y, input.X);
                    Vector2 direction = new Vector2((float)Math.Cos(rotation) * 32, (float)Math.Sin(rotation) * 32);
                    PhysicsHelper.MoveTowards(ref mousePos, position + direction, 5);
                    Mouse.SetPosition((int)mousePos.X, (int)mousePos.Y);
                    cursor.Face(input);
                }
                diff = mousePos - position;
                angle = (float)Math.Atan2(diff.Y, diff.X);
                float percentualPosition = (MathHelper.WrapAngle(angle - MathHelper.PiOver2) + MathHelper.Pi) / MathHelper.TwoPi;
                if (diff.Length() > nullRadius)
                    index = Convert.ToInt32(displayedItems.Count * percentualPosition) >= itemCount ? 0 : Convert.ToInt32(displayedItems.Count * percentualPosition);
            }

            for (int i = 0; i < displayedItems.Count; i++)
            {
                displayedItems[i].Update(index);
                if (displayedItems[i].IsClosed())
                    displayedItems.RemoveAt(i);
            }

            if (displayedItems.Count > 0)
            {
                scale = displayedItems[displayedItems.Count / 2].scale;
                fade = Color.Multiply(Color.White, displayedItems[displayedItems.Count / 2].scale);
            }

            if (closeMenu())
            {
                for (int i = 0; i < displayedItems.Count; i++)
                {
                    displayedItems[i].Close();
                }
                IsOpen = false;
            }
        }

        public int GetIndex()
        {
            return index;
        }

        public void Use()
        {
            allItems[index].Use();
        }

        public void Draw(SpriteBatch sb)
        {
            for (int i = 0; i < displayedItems.Count; i++)
            {
                displayedItems[i].Draw(sb);
            }
            if (displayedItems.Count > 0)
            {
                SpriteFont font = Library.fonts["SmallFont"];
                string text = (index + 1).ToString();
                Texture2D texture = Library.textures["RadialMenuItem"];
                Texture2D highlightTexture = Library.textures["RadialMenuHighlight"];
                sb.Draw(highlightTexture, position, null, fade, 0, new Vector2((float)highlightTexture.Width / 2, (float)highlightTexture.Height / 2), (float)20 / 32, SpriteEffects.None, 0);
                sb.Draw(texture, position, null, fade, 0, new Vector2((float)texture.Width / 2, (float)texture.Height / 2), (float)20 / 32, SpriteEffects.None, 0);
                sb.DrawString(font, text, position, Color.Black, 0, font.MeasureString(text) / 2, scale, SpriteEffects.None, 1);
            }
        }

        private class Item
        {
            Vector2 originalPosition, originalTarget, position, targetPosition, origin;
            Texture2D texture;
            bool isHighlighted;
            bool wasHighlighted;
            float speed, radius;
            RadialMenu parent;
            int index;
            public float scale { get; private set; }
            Function function;
            float textureScale;

            public Item(int index, RadialMenu parent, Function function, Texture2D icon)
            {
                texture = icon;
                origin = new Vector2((float)texture.Width / 2, (float)texture.Height / 2);
                
                int bigSide = Math.Max(texture.Height, texture.Width);
                if (bigSide > 64)
                    textureScale = 64 / (float)bigSide;
                else
                    textureScale = 1;

                this.index = index;
                this.function = function;
                this.parent = parent;
                if (index == 0)
                    isHighlighted = true;
                else
                    isHighlighted = false;
            }

            public void Update(int highlightedIndex)
            {
                wasHighlighted = isHighlighted;

                Vector2 diff = targetPosition - position;
                Vector2 scaleDiff = originalTarget - position;
                scale = (radius - scaleDiff.Length()) / radius;
                if (diff.Length() < speed)
                {
                    position = targetPosition;
                }
                else
                {
                    diff.Normalize();
                    position += diff * speed;
                }

                isHighlighted = highlightedIndex == index;

                if (parent.applyOnChange && isHighlighted && !wasHighlighted)
                {
                    Use();
                }
            }

            public void Open()
            {
                radius = parent.radius;
                this.position = parent.position;
                float rotation = ((MathHelper.TwoPi / parent.itemCount) * index) - MathHelper.PiOver2;
                targetPosition = position + new Vector2((float)Math.Cos(rotation) * radius, (float)Math.Sin(rotation) * radius);
                originalTarget = targetPosition;
                originalPosition = position;
                Vector2 diff = targetPosition - position;
                speed = diff.Length() / (index * 4);
            }

            public void Close()
            {
                targetPosition = originalPosition;
            }

            public void Use()
            {
                if (isHighlighted)
                    function();
            }

            public bool IsClosed()
            {
                return position == originalPosition;
            }

            public void Draw(SpriteBatch sb)
            {
                Color fade = Color.White;
                if (!isHighlighted)
                    fade = Color.Multiply(Color.White, 0.7f);
                sb.Draw(texture, position, null, fade, 0, origin, scale * textureScale, SpriteEffects.None, 0);
            }
        }
    }
}
