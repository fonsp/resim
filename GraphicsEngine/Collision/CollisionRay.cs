using OpenTK;

namespace GraphicsLibrary.Collision
{
	public class CollisionRay
	{
		/// <summary>
		/// Ray origin
		/// </summary>
		public Vector3 eye;
		/// <summary>
		/// Ray direction
		/// </summary>
		public Vector3 dir;

		/// <summary>
		/// Creates a new collision ray
		/// </summary>
		/// <param name="origin">Ray origin</param>
		/// <param name="direction">Ray direction</param>
		public CollisionRay(Vector3 origin, Vector3 direction)
		{
			eye = origin;
			dir = direction;
		}
	}
}