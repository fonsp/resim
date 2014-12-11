using System;
using System.Diagnostics;
using GraphicsLibrary;
using GraphicsLibrary.Core;

namespace Resim
{
	public class BasicClock:Entity
	{
		public float time = 0f;

		public BasicClock(string name)
			: base(name)
		{
			mesh = clockMesh;
		}

		public override void Update(float timeSinceLastUpdate)
		{
			time += timeSinceLastUpdate;
			//Debug.WriteLine(timeSinceLastUpdate);
			base.Update(timeSinceLastUpdate);
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
