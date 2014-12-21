using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using GraphicsLibrary.Content;

namespace GraphicsLibrary.Core
{
	public class Entity:Node
	{

		public Mesh mesh;
		/// <summary>
		/// Entity visibility.
		/// </summary>
		public bool isVisible = true;
		/// <summary>
		/// Enable/dispable lighting.
		/// </summary>
		public bool isLit = true;
		/// <summary>
		/// Write to the OpenGL depth buffer. True by default.
		/// </summary>
		public bool writeDepthBuffer = true;
		/// <summary>
		/// Read the OpenGL depth buffer. True by default.
		/// </summary>
		public bool readDepthBuffer = true;
		/// <summary>
		/// Render a wireframe after rendering the mesh.
		/// </summary>
		public bool wireFrame = false;

		public float materialAge = 0;
		public float materialLifetime = 1;

		public Entity(string name)
			: base(name)
		{

		}

		public override void UpdateNode(float timeSinceLastUpdate)
		{
			base.UpdateNode(timeSinceLastUpdate);
			materialAge += timeSinceLastUpdate;
		}

		public override void Render(int pass)
		{
			if(isVisible && renderPass == pass && mesh != null)
			{
				if(mesh.shader == null)
				{
					if(isLit)
					{
						Shader.diffuseShaderCompiled.Enable();

						float v = (RenderWindow.Instance.smoothedVelocity - derivedVelocity).Length;
						float b = v / RenderWindow.Instance.c;
						float lf = 1f / (float)Math.Sqrt(1.0 - b * b);
						Vector3 vDir = (RenderWindow.Instance.smoothedVelocity - derivedVelocity).Normalized();
						Shader.diffuseShaderCompiled.SetUniform("bL", b);
						Shader.diffuseShaderCompiled.SetUniform("vdirL", vDir);
						Shader.diffuseShaderCompiled.SetUniform("lfL", lf);
					}
					else
					{
						Shader.unlitShaderCompiled.Enable();

						float v = (RenderWindow.Instance.smoothedVelocity - derivedVelocity).Length;
						float b = v / RenderWindow.Instance.c;
						float lf = 1f / (float)Math.Sqrt(1.0 - b * b);
						Vector3 vDir = (RenderWindow.Instance.smoothedVelocity - derivedVelocity).Normalized();
						Shader.unlitShaderCompiled.SetUniform("bL", b);
						Shader.unlitShaderCompiled.SetUniform("vdirL", vDir);
						Shader.unlitShaderCompiled.SetUniform("lfL", lf);
					}
				}
				else
				{
					mesh.shader.Enable();

					float v = (RenderWindow.Instance.smoothedVelocity - derivedVelocity).Length;
					float b = v / RenderWindow.Instance.c;
					float lf = 1f / (float)Math.Sqrt(1.0 - b * b);
					Vector3 vDir = (RenderWindow.Instance.smoothedVelocity - derivedVelocity).Normalized();
					mesh.shader.SetUniform("bL", b);
					mesh.shader.SetUniform("vdirL", vDir);
					mesh.shader.SetUniform("lfL", lf);
				}

				GL.Color4(mesh.material.GetCurrentColor(materialAge / materialLifetime));

				if(!writeDepthBuffer)
				{
					GL.DepthMask(false);
				}

				if(!readDepthBuffer)
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

				if(mesh.useVBO && mesh.hasVBO)
				{
					GL.EnableClientState(ArrayCap.TextureCoordArray);
					GL.EnableClientState(ArrayCap.NormalArray);
					GL.EnableClientState(ArrayCap.VertexArray);
					GL.InterleavedArrays(InterleavedArrayFormat.T2fN3fV3f, 0, IntPtr.Zero);

					GL.BindBuffer(BufferTarget.ArrayBuffer, mesh.VBOids[0]);
					GL.BindBuffer(BufferTarget.ElementArrayBuffer, mesh.VBOids[1]);
					GL.InterleavedArrays(InterleavedArrayFormat.T2fN3fV3f, 0, IntPtr.Zero);
					GL.DrawElements(BeginMode.Triangles, mesh.vertexArray.Length, DrawElementsType.UnsignedInt, 0);

					GL.DisableClientState(ArrayCap.TextureCoordArray);
					GL.DisableClientState(ArrayCap.NormalArray);
					GL.DisableClientState(ArrayCap.VertexArray);
				}
				else
				{
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