using System;
using System.Collections.Generic;
using System.Diagnostics;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GraphicsLibrary.Core
{
	public class Mesh
	{
		public List<Polygon> polygonList = new List<Polygon>();
		public Material material = new Material();
		public Shader shader;
		//VBO
		public Vertex[] vertexArray;
		public bool useVBO = false;
		public bool hasVBO = false;
		public uint[] VBOids = new uint[2];

		public override string ToString()
		{
			string output = polygonList.Count + "\n";
			foreach(Polygon p in polygonList)
			{
				//output += p.vertices[3].pos + "\n";
				foreach(Vertex v in p.vertices)
				{
					output += v.pos + "\n";
				}
			}
			return output;
		}

		public void GenerateVBO()
		{
			if(!useVBO)
			{
				Debug.WriteLine("WARNING: VBO generation failed: Mesh is using immediate mode");
				return;
			}
			if(hasVBO)
			{
				Debug.WriteLine("WARNING: VBO generation failed: VBO already exists");
				return;
			}
			if(vertexArray == null)
			{
				Debug.WriteLine("WARNING: VBO generation failed: vertexArray is null");
				return;
			}

			int stride = BlittableValueType.StrideOf(vertexArray);
			

			GL.GenBuffers(1, out VBOids[0]);
			GL.BindBuffer(BufferTarget.ArrayBuffer, VBOids[0]);
			GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertexArray.Length * stride), vertexArray, BufferUsageHint.StaticDraw);

			GL.GenBuffers(1, out VBOids[1]);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, VBOids[1]);
			GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(vertexArray.Length * sizeof(uint)), IntPtr.Zero, BufferUsageHint.StaticDraw);
			GL.BufferSubData(BufferTarget.ElementArrayBuffer, IntPtr.Zero, (IntPtr)(vertexArray.Length * sizeof(uint)), RenderWindow.Instance.elementBase);
			hasVBO = true;
			Debug.WriteLine("VBO generation complete");
		}
	}
}