using OpenTK;
using OpenTK.Graphics.OpenGL;
using GraphicsLibrary.Content;

namespace GraphicsLibrary.Core
{
	public class Entity:Node
	{
		public Mesh mesh;
		public bool isVisible = true;
		public bool isLit = true;
		public bool writeDepthBuffer = true;
		public bool readDepthBuffer = true;
		public bool wireFrame = false;

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
				if(mesh.shader == null)
				{
					if (isLit)
					{
						Shader.diffuseShaderCompiled.Enable();
					}
					else
					{
						Shader.unlitShaderCompiled.Enable();
					}
				}
				else
				{
					mesh.shader.Enable();
				}

				GL.Color4(mesh.material.GetCurrentColor(materialAge / materialLifetime));

				if(!writeDepthBuffer)
				{
					GL.DepthMask(false);
				}

				if (!readDepthBuffer)
				{
					GL.Disable(EnableCap.DepthTest);
				}
				if(derivedScale != new Vector3(1, 1, 1))
				{
					GL.Enable(EnableCap.Normalize); //TODO
				}

				GL.BindTexture(TextureTarget.Texture2D, TextureManager.GetTexture(mesh.material.GetCurrentTexture()));

				GL.PushMatrix();

				Matrix4 mult = Matrix4.Scale(derivedScale) * Matrix4.Rotate(derivedOrientation) * Matrix4.CreateTranslation(derivedPosition);
				GL.MultMatrix(ref mult);

				GL.Material(MaterialFace.Front, MaterialParameter.Diffuse, mesh.material.GetCurrentColor());

				foreach(Polygon poly in mesh.polygonList)
				{
					GL.Begin(PrimitiveType.Polygon);

					for(int i = 0; i < poly.vertices.Length; i++)
					{
						GL.Normal3(poly.vertices[i].nrm);
						GL.TexCoord2(poly.vertices[i].tex);
						GL.Vertex3(poly.vertices[i].pos);
					}

					GL.End();
				}
				if(wireFrame)
				{
					GL.Disable(EnableCap.DepthTest);
					Shader.wireframeShaderCompiled.Enable();
					foreach(Polygon poly in mesh.polygonList)
					{
						GL.Begin(PrimitiveType.LineLoop);

						for(int i = 0; i < poly.vertices.Length; i++)
						{
							GL.Normal3(poly.vertices[i].nrm);
							GL.TexCoord2(poly.vertices[i].tex);
							GL.Vertex3(poly.vertices[i].pos);
						}

						GL.End();
					}
					GL.Enable(EnableCap.DepthTest);
				}

				GL.PopMatrix();

				if(derivedScale != new Vector3(1, 1, 1))
				{
					GL.Disable(EnableCap.Normalize);
				}
				if(!writeDepthBuffer)
				{
					GL.DepthMask(true);
				}
				if(!readDepthBuffer)
				{
					GL.Enable(EnableCap.DepthTest);
				}
			}
		}
	}
}