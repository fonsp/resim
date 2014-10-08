#region References
using GraphicsLibrary.Content;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
#endregion

namespace GraphicsLibrary.Core
{
	public class Entity:Node
	{
		public Mesh mesh;
		public bool isVisible = true;
		public bool isLit = true;
		public bool writeToDepthBuffer = true;

		public float materialAge = 0;
		public float materialLifetime = 1;

		public Entity(string name)
			: base(name)
		{

		}

		public override void Update(float timeSinceLastUpdate)
		{
			base.Update(timeSinceLastUpdate);
			materialAge += timeSinceLastUpdate;
		}

		public override void Render(int pass)
		{
			if(isVisible && renderPass == pass)
			{
				if(!isLit)
				{
					GL.Disable(EnableCap.Lighting);
					GL.Color4(mesh.material.GetCurrentColor(materialAge / materialLifetime));
				}

				if(!writeToDepthBuffer)
				{
					GL.DepthMask(false);
				}

				if(derivedScale != new Vector3(1, 1, 1))
				{
					GL.Enable(EnableCap.Normalize);
				}

				GL.BindTexture(TextureTarget.Texture2D, TextureManager.GetTexture(mesh.material.GetCurrentTexture()));

				GL.PushMatrix();

				Matrix4 mult = Matrix4.Scale(derivedScale) * Matrix4.Rotate(derivedOrientation) * Matrix4.CreateTranslation(derivedPosition);
				GL.MultMatrix(ref mult);

				GL.Material(MaterialFace.Front, MaterialParameter.Diffuse, mesh.material.GetCurrentColor());

				foreach(Polygon poly in mesh.polygonList)
				{
					GL.Begin(BeginMode.Polygon);

					for(int i = 0; i < poly.vertices.Length; i++)
					{
						GL.Normal3(poly.vertices[i].nrm);
						GL.TexCoord2(poly.vertices[i].tex);
						GL.Vertex3(poly.vertices[i].pos);
					}

					GL.End();
				}

				GL.PopMatrix();

				if(derivedScale != new Vector3(1, 1, 1))
				{
					GL.Disable(EnableCap.Normalize);
				}

				if(!isLit)
				{
					GL.Enable(EnableCap.Lighting);
				}
				if(!writeToDepthBuffer)
				{
					GL.DepthMask(true);
				}
			}
		}
	}
}