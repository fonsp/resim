using OpenTK;

namespace GraphicsLibrary.Collision
{
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