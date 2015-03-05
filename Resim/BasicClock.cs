using System;
using GraphicsLibrary;
using GraphicsLibrary.Core;
using OpenTK;

namespace Resim
{
	public class BasicClock:Entity
	{
		public float time = 0f;
		public Vector3 A, B;
		public float period = 3.14159f;

		public BasicClock(string name, Vector3 a, Vector3 b)
			: base(name)
		{
			A = a;
			B = b;
			period = Math.Max(period, 3.1416f * (B - A).Length / RenderWindow.Instance.c);
		}

		public BasicClock(string name, Vector3 a, Vector3 b, float period)
			: base(name)
		{
			A = a;
			B = b;
			this.period = Math.Max(period, 3.1416f * (B - A).Length / RenderWindow.Instance.c);
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

			float factor = (2f * 3.14159265358979f) / period;
			float apparentTime = RenderWindow.Instance.worldTime - (derivedPosition - Camera.Instance.position).Length / RenderWindow.Instance.c;

			position = Vector3.Lerp(A, B, ((float)Math.Sin(apparentTime * factor) + 1f) * .5f);
			velocity = Vector3.Multiply(B - A, (float)Math.Cos(apparentTime * factor) * factor * .5f);
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
