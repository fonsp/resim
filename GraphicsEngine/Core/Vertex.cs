// Vertex.cs
//
// Copyright 2013 Fons van der Plas
// Fons van der Plas, fonsvdplas@gmail.com

using OpenTK;

namespace GraphicsLibrary.Core
{
	/* Een Vertex is een hoekpunt van een polygon (zie Polygon.cs)
	 * Hierin worden de gegevens van de vertex opgeslagen voor OpenGL
	 */
	public struct Vertex
	{
		public Vector3 pos;
		public Vector3 nrm;
		public Vector2 tex;

		public Vertex(Vector3 pos, Vector3 nrm, Vector2 tex)
		{
			this.pos = pos;
			this.nrm = nrm;
			this.tex = tex;
		}

		public Vertex(Vector3 pos)
		{
			this.pos = pos;
			nrm = Vector3.Zero;
			tex = Vector2.Zero;
		}
	}
}