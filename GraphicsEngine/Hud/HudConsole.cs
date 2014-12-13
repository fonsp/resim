using System;
using GraphicsLibrary.Core;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace GraphicsLibrary.Hud
{
	public class HudConsoleInputEventArgs:EventArgs
	{
		private readonly string input;
		private readonly string[] inputArray;

		public HudConsoleInputEventArgs(string input)
		{
			this.input = input;
			inputArray = input.Split(new char[] { ' ' });
		}

		public string Input
		{
			get
			{
				return input;
			}
		}

		public string[] InputArray
		{
			get
			{
				return inputArray;
			}
		}
	}
	public delegate void HudConsoleInputHandler(object sender, HudConsoleInputEventArgs e);

	public class HudConsole:HudElement
	{
		public event HudConsoleInputHandler DebugInput;

		private uint numberOfLines;
		public uint NumberOfLines
		{
			get
			{
				return numberOfLines;
			}
			set
			{
				if(numberOfLines != value)
				{
					numberOfLines = value;
					UpdateTextFields();
				}
			}
		}

		public string inputPrefix = ">";
		public string input = "";
		public bool enabled = false;
		private string fontTextureName = "default"; //TODO: default font
		public string FontTextureName
		{
			get
			{
				return fontTextureName;
			}
			set
			{
				fontTextureName = value;
				foreach(TextField t in textFields)
				{
					t.textMaterial.textureName = fontTextureName;
				}
				inputField.textMaterial.textureName = fontTextureName;
			}
		}
		private Color4 textColor = Color4.White;

		public Color4 TextColor
		{
			get
			{
				return textColor;
			}
			set
			{
				textColor = value;
				foreach(TextField t in textFields)
				{
					t.textMaterial.baseColor = textColor;
				}
				inputField.textMaterial.baseColor = textColor;
			}
		}
		public Color4 backgroundColor = new Color4(0f, 0f, 0f, .5f);
		public float width = 480;
		public float height = 300;
		private int sizeX = 8;

		public int SizeX
		{
			get
			{
				return sizeX;
			}
			set
			{
				sizeX = value;
				foreach(TextField t in textFields)
				{
					t.sizeX = value;
				}
				inputField.sizeX = sizeX;
			}
		}

		private int sizeY = 12;

		public int SizeY
		{
			get
			{
				return sizeY;
			}
			set
			{
				sizeY = value;
				foreach(TextField t in textFields)
				{
					t.sizeY = value;
				}
				inputField.sizeY = sizeY;
			}
		}

		private TextField[] textFields;
		private TextField inputField = new TextField("asdf");
		private HudImage back = new HudImage("back");

		public HudConsole(string name, uint numberOfLines)
			: base(name)
		{
			this.numberOfLines = numberOfLines;

			UpdateTextFields();

			RenderWindow.Instance.KeyPress += HandleKeyPress;
			RenderWindow.Instance.KeyDown += HandleKeyDown;
		}

		private void HandleKeyDown(object sender, KeyboardKeyEventArgs e)
		{
			if(enabled)
			{
				if(e.Key == Key.BackSpace)
				{
					if(input.Length > 0)
						input = input.Remove(input.Length - 1);
				}
				else if(e.Key == Key.Enter || e.Key == Key.KeypadEnter)
				{
					if(DebugInput != null)
					{
						DebugInput(this, new HudConsoleInputEventArgs(input));
					}
					input = "";
				}
			}
		}

		private void HandleKeyPress(object sender, KeyPressEventArgs e)
		{
			if(enabled)
			{
				byte key = (byte)e.KeyChar;
				if(key == 8) //BACKSPACE
				{
				}
				else if(key == 13) //ENTER
				{
				}
				else
				{
					input += e.KeyChar;
				}
			}
		}

		private void UpdateTextFields()
		{
			height = sizeY * (numberOfLines + 1);
			textFields = new TextField[numberOfLines];
			for(int i = 0; i < numberOfLines; i++)
			{
				textFields[i] = new TextField("asdf");
				textFields[i].text = "";
				textFields[i].textMaterial.textureName = fontTextureName;
				textFields[i].sizeX = sizeX;
				textFields[i].sizeY = sizeY;
				textFields[i].textMaterial.baseColor = textColor;
			}
			inputField = new TextField("asfd");
			inputField.text = inputPrefix + input;
			inputField.textMaterial.textureName = fontTextureName;
			inputField.sizeX = sizeX;
			inputField.sizeY = sizeY;
			inputField.textMaterial.baseColor = textColor;
		}

		public void ClearInput()
		{
			input = "";
		}

		public void ClearScreen()
		{
			foreach(TextField t in textFields)
			{
				t.text = "";
			}
		}

		public void AddLine(string s)
		{
			AddLine(s, Color4.White);
		}

		public void AddLine(string s, Color4 color)
		{
			for(int i = 0; i < numberOfLines - 1; i++)
			{
				textFields[i].text = textFields[i + 1].text;
				textFields[i].textMaterial.baseColor = textFields[i + 1].textMaterial.baseColor;
			}
			textFields[numberOfLines - 1].text = s;
			textFields[numberOfLines - 1].textMaterial.baseColor = color;
		}

		public void AddText(string s, Color4 color)
		{
			while(s.Length > width / sizeX)
			{
				AddLine(s.Substring(0, (int)width / sizeX), color);
				s = s.Remove(0, (int)width / sizeX);
			}
			AddLine(s, color);
		}

		public void AddText(string s)
		{
			AddText(s, Color4.White);
		}

		public override void Render()
		{
			if(isVisible)
			{
				int lineStart;
				for(lineStart = 0; lineStart < numberOfLines; lineStart++)
				{
					if(textFields[lineStart].text != "")
					{
						break;
					}
				}
				Shader.hudShaderCompiled.Enable();
				#region Background

				/*GL.Disable(EnableCap.Texture2D);

				GL.MatrixMode(MatrixMode.Modelview);
				GL.PushMatrix();
				GL.Translate(position.X, position.Y, 0);

				GL.Begin(PrimitiveType.Quads);
				GL.Color4(backgroundColor);

				GL.Vertex2(00, lineStart * sizeY);
				GL.Vertex2(width * derivedScale.X, lineStart * sizeY);
				GL.Vertex2(width * derivedScale.X, height * derivedScale.Y);
				GL.Vertex2(00, height * derivedScale.Y);

				GL.Vertex2(00, (height * derivedScale.Y) - sizeY);
				GL.Vertex2(width * derivedScale.X, (height * derivedScale.Y) - sizeY);
				GL.Vertex2(width * derivedScale.X, height * derivedScale.Y);
				GL.Vertex2(00, height * derivedScale.Y);

				GL.Color4(Color4.White);
				GL.End();

				GL.PopMatrix();

				GL.Enable(EnableCap.Texture2D);*/

				back.color = backgroundColor;
				back.width = width;
				back.imageTextureName = "white";

				back.derivedPosition = position;
				back.derivedPosition.Y += lineStart * sizeY;
				back.height = height - (lineStart * sizeY);
				back.Render();

				back.derivedPosition = position;
				back.derivedPosition.Y += height - sizeY;
				back.height = sizeY;
				back.Render();

				#endregion
				#region Lines

				for(int i = 0; i < numberOfLines; i++)
				{
					textFields[i].position = derivedPosition + new Vector2(0, i * sizeY);
					textFields[i].Render();
				}

				#endregion
				#region Input line

				inputField.text = inputPrefix + input;
				inputField.position = derivedPosition + new Vector2(0, numberOfLines * sizeY);
				inputField.Render();

				#endregion
			}
		}
	}
}