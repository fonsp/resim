// Polygon.cs
//
// Copyright 2013 Fons van der Plas
// Fons van der Plas, fonsvdplas@gmail.com

namespace GraphicsLibrary.Core
{
	/* Een polygon is deel van mesh
	 * Hierin wordt een reeks van (vaak 3) hoekpunten opgeslagen (zie Vertex.cs)
	 */
	public struct Polygon
	{
		public Vertex[] vertices;

		public Polygon(Vertex[] vertices)
		{
			this.vertices = vertices;
		}
	}
}