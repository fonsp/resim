// CollisionRay.cs
//
// Copyright 2013 Fons van der Plas
// Fons van der Plas, fonsvdplas@gmail.com

using OpenTK;

namespace GraphicsLibrary.Collision
{
	/* Deze class slaat de richting en positie van een lijn op
	 * Wordt gebruikt in CollisionBox.cs
	 */
	public class CollisionRay
	{
		public Vector3 eye, dir;

		public CollisionRay(Vector3 origin, Vector3 direction)
		{
			eye = origin;
			dir = direction;
		}
	}
}