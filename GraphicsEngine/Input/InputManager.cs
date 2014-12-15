using System.Drawing;
using OpenTK;
using OpenTK.Input;

namespace GraphicsLibrary.Input
{
	public static class InputManager
	{
		public static bool enabled = true;
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
				if(value == CursorLockState.Centered)
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
				RenderWindow.Instance.CursorVisible = value;
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
			return enabled && RenderWindow.Instance.Keyboard[key];
		}

		public static bool IsKeyUp(Key key)
		{
			return enabled && !RenderWindow.Instance.Keyboard[key];
		}

		public static bool IsKeyToggled(Key key)
		{
			return toggleStates[(int)key];
		}

		public static bool IsAnyKeyDown(Key key)
		{
			if(!enabled)
			{
				return false;
			}
			bool output = false;
			for(int i = 0; i < 131; i++)
			{
				if(RenderWindow.Instance.Keyboard[(Key)i])
				{
					output = true;
				}
			}
			return output;
		}

		public static void UpdateToggleStates()
		{
			if(enabled)
			{
				for(int i = 0; i < 131; i++)
				{
					UpdateToggleState((Key)i);
				}
			}
		}

		public static void UpdateToggleState(Key key)
		{
			currentStates[(int)key] = RenderWindow.Instance.Keyboard[key];

			if(currentStates[(int)key] && !previousStates[(int)key])
			{
				toggleStates[(int)key] = !toggleStates[(int)key];
			}

			previousStates[(int)key] = currentStates[(int)key];
		}

		public static void ClearToggleStates()
		{
			currentStates = new bool[132];
			previousStates = new bool[132];
			toggleStates = new bool[132];
		}

		public static bool IsButtonDown(MouseButton mouseButton)
		{
			return enabled && RenderWindow.Instance.Mouse[mouseButton];
		}

		public static bool IsButtonUp(MouseButton mouseButton)
		{
			return enabled && !RenderWindow.Instance.Mouse[mouseButton];
		}

		public static Vector2 GetMousePosition(CursorLockState cursorLockState)
		{
			CursorLockState = cursorLockState;
			return GetMousePosition();
		}

		public static Vector2 GetMousePosition()
		{
			if(CursorLockState == CursorLockState.Free)
			{
				return new Vector2(System.Windows.Forms.Cursor.Position.X - RenderWindow.Instance.Bounds.Left,
								   System.Windows.Forms.Cursor.Position.Y - RenderWindow.Instance.Bounds.Top);
			}

			Rectangle Bounds = RenderWindow.Instance.Bounds;
			Vector2 mouseDelta = Vector2.Zero;

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