using System;
using System.Collections.Generic;
using System.Net.Mime;
using GraphicsLibrary;
using GraphicsLibrary.Hud;
using GraphicsLibrary.Input;
using OpenTK;
using OpenTK.Input;

namespace Resim
{
	public static class ActionTrigger
	{
		public static float maxDistance = 300f;
		public static TextField textField;
		public static bool enable = false;
		private static bool prevDown = false;
		private static string _actionName = "";
		public static string actionName
		{
			get
			{
				return _actionName;
			}
		}

		static ActionTrigger()
		{
			textField = new TextField("actionTrigger");
			textField.sizeX = 16;
			textField.sizeY = 24;
			_actionName = "ASDF";
		}

		public static void Display(string action)
		{
			enable = true;
			_actionName = action;
			textField.text = "Press E to " + action;
			textField.position.X = (RenderWindow.Instance.Width - textField.sizeX * textField.text.Length) / 2;
		}

		public static bool onActive = false;

		public static void Update()
		{
			textField.isVisible = enable;
			enable = false;
			onActive = InputManager.IsKeyDown(Key.E) && !prevDown;
			prevDown = InputManager.IsKeyDown(Key.E);
		}
	}
}