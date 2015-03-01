using GraphicsLibrary.Core;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using GraphicsLibrary.Content;

namespace GraphicsLibrary.Hud
{
	public class HudImage:HudElement
	{
		public string imageTextureName = "default";

		public float width = 512f;
		public float height = 512f;

		public Color4 color = Color4.White;

		public HudImage(string name)
			: base(name)
		{

		}

		public HudImage(string name, string imageTextureName)
			: base(name)
		{
			this.imageTextureName = imageTextureName;
		}

		public override void Render()
		{
			if(isVisible)
			{
				Shader.hudShaderCompiled.Enable();
				GL.BindTexture(TextureTarget.Texture2D, TextureManager.GetTexture(imageTextureName));

				GL.MatrixMode(MatrixMode.Modelview);
				GL.PushMatrix();
				GL.Translate(derivedPosition.X, derivedPosition.Y, 0);

				GL.Begin(PrimitiveType.Quads);
				GL.Color4(color);
				GL.TexCoord2(0, 0); GL.Vertex2(00, 00);
				GL.TexCoord2(1, 0); GL.Vertex2(width * derivedScale.X, 00);
				GL.TexCoord2(1, 1); GL.Vertex2(width * derivedScale.X, height * derivedScale.Y);
				GL.TexCoord2(0, 1); GL.Vertex2(00, height * derivedScale.Y);
				GL.End();
				GL.PopMatrix();
			}
		}
	}
}