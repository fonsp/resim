using System;
using GraphicsLibrary.Core;

namespace Resim
{
	public class BasicClock:Entity
	{
		public float time = 0f;
		public float rotateVelocity = 2f;

		public BasicClock(string name)
			: base(name)
		{
			mesh = clockMesh;
		}

		public override void Update(float timeSinceLastUpdate)
		{
			base.Update(timeSinceLastUpdate);

			/*if((derivedPosition - Camera.Instance.position).Length < ActionTrigger.maxDistance)
			{
				ActionTrigger.Display("derp");
				if(ActionTrigger.onActive)
				{
					Camera.Instance.position = derivedPosition + new Vector3(0f, 200f, 0f);
				}
			}*/

			time += timeSinceLastUpdate;
		}

		public static void Jump(Node clock)
		{
			clock.position.Y += 100;
		}

		public override void Render(int pass)
		{
			if(mesh == null && clockMesh != null)
			{
				mesh = clockMesh;
			}
			base.Render(pass);
		}

		public override string ToString()
		{
			return "Clock " + name + " set at: " + time;
		}

		public static Mesh clockMesh;
	}
}
