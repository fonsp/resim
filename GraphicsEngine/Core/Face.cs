namespace GraphicsLibrary.Core
{
	public struct Face
	{
		public int[] vIndices;
		public int[] vtIndices;
		public int[] vnIndices;

		public Face(int[] vertexIndices, int[] textureIndices, int[] normalIndices)
		{
			vIndices = vertexIndices;
			vnIndices = normalIndices;
			vtIndices = textureIndices;
		}
	}
}