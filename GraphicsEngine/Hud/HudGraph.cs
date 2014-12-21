using GraphicsLibrary.Core;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace GraphicsLibrary.Hud
{
	public class HudGraph:HudElement
	{
		private HudImage back = new HudImage("asdf");
		public int width = 256;
		public int height = 128;
		/// <summary>
		/// Background color
		/// </summary>
		public Color4 backgroundColor = new Color4(0f, 0f, 0f, .2f);
		/// <summary>
		/// Line color
		/// </summary>
		public Color4 color = Color4.White;
		/// <summary>
		/// The current value to be displayed.
		/// </summary>
		public byte value = 100;
		private int index = 0;
		private float dt = 0f;

		/// <summary>
		/// The data buffer, use 'value' to display data.
		/// </summary>
		public byte[] dataBuffer = new byte[256];

		public HudGraph(string name)
			: base(name)
		{

		}

		public override void Update(float timeSinceLastUpdate)
		{
			base.Update(timeSinceLastUpdate);

			while(dt >= 0.05f)
			{
				dt -= 0.05f;

				dataBuffer[index] = value;
				index = (index + 1) % 256;
			}
			dt += timeSinceLastUpdate;
		}

		public override void Render()
		{
			if(isVisible)
			{
				back.derivedPosition = derivedPosition;
				back.width = width;
				back.height = height;
				back.color = backgroundColor;
				back.imageTextureName = "white";

				back.Render();

				Shader.hudShaderCompiled.Enable();

				GL.MatrixMode(MatrixMode.Modelview);
				GL.PushMatrix();
				GL.Translate(derivedPosition.X, derivedPosition.Y, 0);

				GL.Color4(color);

				GL.Begin(PrimitiveType.LineStrip);

				for(int i = 0; i < 256; i++)
				{
					GL.Vertex2((width * i)/256, height - (dataBuffer[(i + index) % 256] * height)/256);
				}

				GL.End();
				GL.PopMatrix();

			}
		}
	}
}