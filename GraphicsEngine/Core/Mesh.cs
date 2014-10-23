using System;
using System.Collections.Generic;
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
			if(!useVBO || hasVBO)
			{
				return;
			}
			int stride = BlittableValueType.StrideOf(vertexArray);
			

			GL.GenBuffers(1, out VBOids[0]);
			GL.BindBuffer(BufferTarget.ArrayBuffer, VBOids[0]);
			GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertexArray.Length * stride), vertexArray, BufferUsageHint.StaticDraw);

			GL.GenBuffers(1, out VBOids[1]);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, VBOids[1]);
			GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(vertexArray.Length * sizeof(ushort)), IntPtr.Zero, BufferUsageHint.StaticDraw);
			GL.BufferSubData(BufferTarget.ElementArrayBuffer, IntPtr.Zero, (IntPtr)(vertexArray.Length * sizeof(ushort)), RenderWindow.Instance.elementBase);
			hasVBO = true;
		}
	}
}