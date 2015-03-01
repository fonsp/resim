using System.Drawing;
using System.Text;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using GraphicsLibrary.Content;
using GraphicsLibrary.Core;

namespace GraphicsLibrary.Hud
{
	public class TextField:HudElement
	{
		/// <summary>
		/// Text to draw
		/// </summary>
		public string text = "";
		public Material textMaterial = new Material("font2", Color4.White);//TODO: Default font
		public bool squareChar = false;
		public int sizeX = 8;
		public int sizeY = 12;

		public float age = 0;
		public float lifeTime = -1;

		public TextField(string name)
			: base(name)
		{

		}

		public override void Update(float timeSinceLastUpdate)
		{
			base.Update(timeSinceLastUpdate);
			if(lifeTime != -1)
			{
				age += timeSinceLastUpdate;
			}
		}

		public override void Render()
		{
			if(isVisible && (age < lifeTime || lifeTime == -1))
			{
				Shader.hudShaderCompiled.Enable();
				byte[] stringBytes = Encoding.ASCII.GetBytes(text);

				GL.BindTexture(TextureTarget.Texture2D, TextureManager.GetTexture(textMaterial.GetCurrentTexture()));

				GL.MatrixMode(MatrixMode.Modelview);
				GL.PushMatrix();
				GL.Translate(position.X, position.Y, 0);
				for(int i = 0; i < stringBytes.Length; i++)
				{
					double y = stringBytes[i] / 16;
					double x = stringBytes[i] % 16;
					const double d = 0.0625;
					x *= d;
					y *= d;
					GL.Begin(PrimitiveType.Quads);
					GL.Color4(textMaterial.GetCurrentColor(age / lifeTime));
					GL.TexCoord2(x, y); GL.Vertex2(00 + (sizeX * i), 00);
					GL.TexCoord2(d + x, y); GL.Vertex2(sizeX + (sizeX * i), 00);
					GL.TexCoord2(d + x, d + y); GL.Vertex2(sizeX + (sizeX * i), sizeY);
					GL.TexCoord2(x, d + y); GL.Vertex2(00 + (sizeX * i), sizeY);
					GL.End();
					GL.Color4(Color.White);
				}
				GL.PopMatrix();
			}
		}
	}
}