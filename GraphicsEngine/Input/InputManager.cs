// InputManager.cs
//
// Copyright 2013 Fons van der Plas
// Fons van der Plas, fonsvdplas@gmail.com

using System.Drawing;
using OpenTK;
using OpenTK.Input;
using System;

namespace GraphicsLibrary.Input
{
	/* De InputManager class wordt gebruikt om het toetsenbord en muis in het spel te gebruiken
	 */
	public static class InputManager
	{
		private static CursorLockState _cursorLockState = CursorLockState.Free;
		private static bool[] currentStates = new bool[132];
		private static bool[] previousStates = new bool[132];
		private static bool[] toggleStates = new bool[132];
		public static CursorLockState CursorLockState
		{
			get
			{
				return _cursorLockState;
			}
			set
			{
				_cursorLockState = value;
				if (value == CursorLockState.Centered)
				{
					System.Windows.Forms.Cursor.Position = new Point(RenderWindow.Instance.Bounds.Left + (RenderWindow.Instance.Bounds.Width / 2), 
					                                                 RenderWindow.Instance.Bounds.Top + (RenderWindow.Instance.Bounds.Height / 2));
				}
			}
		}

		private static bool _enableCursor = true;

		public static bool EnableCursor
		{
			get
			{
				return _enableCursor;
			}
			set
			{
				_enableCursor = value;
				//TODO:
				/* Er is momenteel geen command om de muis onzichtbaar te maken dat werkt op Linux, Mac en Windows
				 * Windows gebruikt System.Windows.Forms.Cursor.Hide(), maar dit is niet ondersteund met Mono
				 */

				/*if (value)
				{
					try
					{
						RenderWindow.Instance.CursorVisible = true;
					} catch (Exception exception){}
					try
					{
						System.Windows.Forms.Cursor.Show();
					} catch (Exception exception){}
				}
				else
				{
					try
					{
						RenderWindow.Instance.CursorVisible = false;
					} catch (Exception exception){}
					try
					{
						System.Windows.Forms.Cursor.Hide();
					} catch (Exception exception){}
				}*/
			}
		}

		public static void ShowCursor()
		{
			EnableCursor = true;
		}

		public static void HideCursor()
		{
			EnableCursor = false;
		}

		public static bool IsKeyDown(Key key)
		{
			return RenderWindow.Instance.Keyboard[key];
		}

		public static bool IsKeyUp(Key key)
		{
			return !RenderWindow.Instance.Keyboard[key];
		}

		public static bool IsKeyToggled(Key key)
		{
			return toggleStates[(int)key];
		}

		public static bool IsAnyKeyDown(Key key)
		{
			bool output = false;
			for (int i = 0; i < 131; i++)
			{
				if (RenderWindow.Instance.Keyboard[(Key) i])
				{
					output = true;
				}
			}
			return output;
		}

		public static void UpdateToggleStates()
		{
			for (int i = 0; i < 131; i++)
			{
				UpdateToggleState((Key)i);
			}
		}

		public static void UpdateToggleState(Key key)
		{
			currentStates[(int)key] = RenderWindow.Instance.Keyboard[key];

			if (currentStates[(int) key] && !previousStates[(int) key])
			{
				toggleStates[(int) key] = !toggleStates[(int) key];
			}

			previousStates[(int) key] = currentStates[(int) key];
		}

		public static void ClearToggleStates()
		{
			currentStates = new bool[132];
			previousStates = new bool[132];
			toggleStates = new bool[132];
		}

		public static bool IsButtonDown(MouseButton mouseButton)
		{
			return RenderWindow.Instance.Mouse[mouseButton];
		}

		public static bool IsButtonUp(MouseButton mouseButton)
		{
			return !RenderWindow.Instance.Mouse[mouseButton];
		}

		public static Vector2 GetMousePosition(CursorLockState cursorLockState)
		{
			CursorLockState = cursorLockState;
			return GetMousePosition();
		}

		public static Vector2 GetMousePosition()
		{
			if (CursorLockState == CursorLockState.Free)
			{
				return new Vector2(System.Windows.Forms.Cursor.Position.X - RenderWindow.Instance.Bounds.Left, 
				                   System.Windows.Forms.Cursor.Position.Y - RenderWindow.Instance.Bounds.Top);
			}

			Rectangle Bounds = RenderWindow.Instance.Bounds;
			Vector2 mouseDelta = Vector2.Zero;
			/* Hiermee blijft de muis altijd in het midden van het scherm
			 */
			mouseDelta.X = System.Windows.Forms.Cursor.Position.X - (Bounds.Left + (Bounds.Width / 2));
			mouseDelta.Y = (Bounds.Top + (Bounds.Height / 2)) - System.Windows.Forms.Cursor.Position.Y;
			System.Windows.Forms.Cursor.Position = new Point(Bounds.Left + (Bounds.Width / 2), Bounds.Top + (Bounds.Height / 2));

			return mouseDelta;
		}
	}

	public enum CursorLockState
	{
		Centered,
		Free
	}
}