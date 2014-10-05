// Mesh.cs
//
// Copyright 2013 Fons van der Plas
// Fons van der Plas, fonsvdplas@gmail.com

using System.Collections.Generic;

namespace GraphicsLibrary.Core
{
	/* Een mesh wordt gebruikt door een Entity
	 * om een reeks polygons en een materiaal op te slaan
	 */
	public class Mesh
	{
		public List<Polygon> polygonList = new List<Polygon>();
		public Material material = new Material();

		//Voor debug
		public override string ToString()
		{
			string output = polygonList.Count + "\n";
			foreach (Polygon p in polygonList)
			{
				//output += p.vertices[3].pos + "\n";
				foreach (Vertex v in p.vertices)
				{
					output += v.pos + "\n";
				}
			}
			return output;
		}
	}
}