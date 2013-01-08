using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MajgEngine
{
	static class Input
	{
		private static PlayerIndex[] playerIndices = new PlayerIndex[4] {PlayerIndex.One, PlayerIndex.Two, PlayerIndex.Three, PlayerIndex.Four };
		private static GamePadState[] gamePadStates = new GamePadState[4];
		private static GamePadState[] prevGamePadStates = new GamePadState[4];
		private static KeyboardState keys;
		private static KeyboardState prevKeys;
    private static MouseState mouse;
    private static MouseState prevMouse;

		public static void Update()
		{
			prevKeys = keys;
      prevMouse = mouse;
			keys = Keyboard.GetState();
      mouse = Mouse.GetState();
			for(int i = 0; i < 4; i++)
			{
				prevGamePadStates[i] = gamePadStates[i];
				gamePadStates[i] = GamePad.GetState(playerIndices[i]);
			}            
		}

        #region GamePads

        public static bool GP_IsPressed(PlayerIndex index, Buttons button)
		{
			if (gamePadStates[(int)index].IsButtonDown(button))
				return true;
			return false;
		}

        public static bool GP_NewPress(PlayerIndex index, Buttons button)
        {
            if (gamePadStates[(int)index].IsButtonDown(button) && prevGamePadStates[(int)index].IsButtonUp(button))
                return true;
            return false;
        }

        public static bool GP_NewRelease(PlayerIndex index, Buttons button)
        {
            if (gamePadStates[(int)index].IsButtonUp(button) && prevGamePadStates[(int)index].IsButtonDown(button))
                return true;
            return false;
        }

		public static Vector2 GP_RightThumbstick(PlayerIndex index)
		{
			return gamePadStates[(int)index].ThumbSticks.Right;
		}

		public static Vector2 GP_LeftThumbstick(PlayerIndex index)
		{
			return gamePadStates[(int)index].ThumbSticks.Left;
		}

        #endregion

        #region Keyboard

        public static bool Key_AnyNewKey()
        {
            if (keys.GetPressedKeys().Length > 0 && prevKeys.GetPressedKeys().Length == 0)
                return true;
            return false;
        }

        public static bool Key_NewDown(Keys key)
		{
			if (keys.IsKeyDown(key) && prevKeys.IsKeyUp(key))
				return true;
			return false;
        }

        public static bool Key_Down(Keys key)
        {
            if (keys.IsKeyDown(key))
                return true;
            return false;
        }

        public static bool Key_Release(Keys key)
        {
            if (keys.IsKeyUp(key) && prevKeys.IsKeyDown(key))
                return true;
            return false;
        }

        #endregion

        #region Mouse

        public static bool Mouse_LeftDown()
        {
            return mouse.LeftButton == ButtonState.Pressed;
        }

        public static bool Mouse_LeftNewDown()
        {
            return mouse.LeftButton == ButtonState.Pressed && prevMouse.LeftButton == ButtonState.Released;
        }

        public static bool Mouse_LeftNewUp()
        {
            return mouse.LeftButton == ButtonState.Released && prevMouse.LeftButton == ButtonState.Pressed;
        }

        public static bool Mouse_RightDown()
        {
            return mouse.RightButton == ButtonState.Pressed;
        }

        public static bool Mouse_RightNewDown()
        {
            return mouse.RightButton == ButtonState.Pressed && prevMouse.RightButton == ButtonState.Released;
        }

        public static bool Mouse_RightNewUp()
        {
            return mouse.RightButton == ButtonState.Released && prevMouse.RightButton == ButtonState.Pressed;
        }

        public static Vector2 Mouse_Position()
        {
            return new Vector2(mouse.X, mouse.Y);
        }

        public static Vector2 Mouse_PreviousPosition()
        {
            return new Vector2(prevMouse.X, prevMouse.Y);
        }

        #endregion
    }
}
