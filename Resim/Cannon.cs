using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicsLibrary.Core;
using OpenTK;

namespace Resim
{
	public class Cannon:Entity
	{
		public Vector3 newVelocity = new Vector3(-1000f, 800f, -900f);
		public Cannon(string name) : base(name)
		{
		}

		public override void Update(float timeSinceLastUpdate)
		{
			if((derivedPosition - Camera.Instance.position).Length < ActionTrigger.maxDistance)
			{
				ActionTrigger.Display("jump");
				if(ActionTrigger.onActive)
				{
					Camera.Instance.velocity = newVelocity;
				}
			}
		}
	}
}
