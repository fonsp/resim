// Face.cs
//
// Copyright 2013 Fons van der Plas
// Fons van der Plas, fonsvdplas@gmail.com

namespace GraphicsLibrary.Core
{
	/* Deze struct wordt gebruikt door ObjConverter.cs om de indices op te slaan
	 */
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

