using OpenTK;

namespace GraphicsLibrary.Core
{
	public struct Vertex
	{
		public Vector2 tex;
		public Vector3 nrm;
		public Vector3 pos;
		
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