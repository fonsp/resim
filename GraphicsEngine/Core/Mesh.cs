using System.Collections.Generic;

namespace GraphicsLibrary.Core
{
	public class Mesh
	{
		public List<Polygon> polygonList = new List<Polygon>();
		public Material material = new Material();
		public Shader shader;

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
	}
}