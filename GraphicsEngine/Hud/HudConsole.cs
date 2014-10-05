// HudDebug.cs
//
// Copyright 2013 Fons van der Plas
// Fons van der Plas, fonsvdplas@gmail.com

using System;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace GraphicsLibrary.Hud
{
	/* Een HudConsole is een HudElement met een reeks TextFields met een zwarte achtergrond
	 * Het werkt net als een console/terminal op een PC
	 * 
	 * Met tab of ~ kan het ogpebracht worden in het spel
	 */
	public class HudDebugInputEventArgs : EventArgs
	{
		private readonly string input;
		private readonly string[] inputArray;

		public HudDebugInputEventArgs(string input)
		{
			this.input = input;
			inputArray = input.Split(new char[]{' '});
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
	public delegate void HudDebugInputHandler(object sender, HudDebugInputEventArgs e);

	public class HudDebug : HudElement
	{
		public event HudDebugInputHandler DebugInput;

		private uint numberOfLines;
		public uint NumberOfLines
		{
			get
			{
				return numberOfLines;
			}
			set
			{
				if (numberOfLines != value)
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
				foreach (TextField t in textFields)
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
				foreach (TextField t in textFields)
				{
					t.textMaterial.baseColor = textColor;
				}
				inputField.textMaterial.baseColor = textColor;
			}
		}
		public Color4 backgroundColor = new Color4(0f, 0f, 0f, .5f);
		public float width = 500;
		public float height = 300;
		private int size = 16;

		public int Size
		{
			get
			{
				return size;
			}
			set
			{
				size = value;
				foreach (TextField t in textFields)
				{
					t.size = value;
				}
				inputField.size = size;
			}
		}

		private TextField[] textFields;
		private TextField inputField = new TextField("asdf");

		public HudDebug(string name, uint numberOfLines) : base(name)
		{
			this.numberOfLines = numberOfLines;
			
			UpdateTextFields();

			RenderWindow.Instance.KeyPress += HandleKeyPress;
		}

		private void HandleKeyPress(object sender, KeyPressEventArgs e)
		{
			if (enabled)
			{
				byte key = (byte) e.KeyChar;
				if (key == 8) //BACKSPACE
				{
					if (input.Length > 0)
						input = input.Remove(input.Length - 1);
				}
				else if (key == 13) //ENTER
				{
					if (DebugInput != null)
					{
						DebugInput(this, new HudDebugInputEventArgs(input));
					}
					input = "";
				}
				else
				{
					input += e.KeyChar;
				}
			}
		}

		private void UpdateTextFields()
		{
			height = size * (numberOfLines + 1);
			textFields = new TextField[numberOfLines];
			for (int i = 0; i < numberOfLines; i++)
			{
				textFields[i] = new TextField("asdf");
				textFields[i].text = "";
				textFields[i].textMaterial.textureName = fontTextureName;
				textFields[i].size = size;
				textFields[i].textMaterial.baseColor = textColor;
			}
			inputField = new TextField("asfd");
			inputField.text = inputPrefix + input;
			inputField.textMaterial.textureName = fontTextureName;
			inputField.size = size;
			inputField.textMaterial.baseColor = textColor;
		}

		public void ClearInput()
		{
			input = "";
		}

		public void ClearScreen()
		{
			foreach (TextField t in textFields)
			{
				t.text = "";
			}
		}

		public void AddLine(string s)
		{
			for (int i = 0; i < numberOfLines - 1; i++)
			{
				textFields[i].text = textFields[i + 1].text;
			}
			textFields[numberOfLines-1].text = s;
		}

		public override void Render()
		{
			if (isVisible)
			{
				#region Background
				GL.Disable(EnableCap.Texture2D);

				GL.MatrixMode(MatrixMode.Modelview);
				GL.PushMatrix();
				GL.Translate(position.X, position.Y, 0);

				GL.Begin(BeginMode.Quads);
				GL.Color4(backgroundColor);

				GL.Vertex2(00, 00);
				GL.Vertex2(width * derivedScale.X, 00);
				GL.Vertex2(width * derivedScale.X, height * derivedScale.Y);
				GL.Vertex2(00, height * derivedScale.Y);

				GL.Vertex2(00, (height * derivedScale.Y)-size);
				GL.Vertex2(width * derivedScale.X, (height * derivedScale.Y) - size);
				GL.Vertex2(width * derivedScale.X, height * derivedScale.Y);
				GL.Vertex2(00, height * derivedScale.Y);

				GL.Color4(Color4.White);
				GL.End();
				GL.PopMatrix();

				GL.Enable(EnableCap.Texture2D);
				#endregion
				#region Lines
				for (int i = 0; i < numberOfLines; i++)
				{
					textFields[i].position = derivedPosition + new Vector2(0, i * size);
					textFields[i].Render();
				}
				#endregion
				#region Input line

				inputField.text = inputPrefix + input;
				inputField.position = derivedPosition + new Vector2(0, numberOfLines*size);
				inputField.Render();

				#endregion
			}
		}
	}
}
