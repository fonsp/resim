using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicsLibrary.Core;
using OpenTK;
using OpenTK.Graphics;

namespace Resim
{
	public class LampSwitch:Entity
	{
		public Node connectedLamps;
		public Entity lampHead, lampPost;
		public bool enabled = true;

		public LampSwitch(string name, Node connectedLamps) : base(name)
		{
			this.connectedLamps = connectedLamps;
			lampHead = new Entity(name + "_head");
			lampPost = new Entity(name + "_post");
			lampHead.mesh = new Mesh();
			lampPost.mesh = new Mesh();
			lampHead.mesh.polygonList = Lamp.lampHeadMesh.polygonList;
			lampPost.mesh.polygonList = Lamp.lampPostMesh.polygonList;
			lampHead.mesh.material.textureName = lampPost.mesh.material.textureName = "white";
			lampHead.position = new Vector3(0f, 60f, 0f);
			lampHead.mesh.material.baseColor = new Color4(0.9f, 0.3f, 0.3f, 1.0f);
			lampPost.mesh.material.baseColor = new Color4(0.6f, 0.3f, 0.3f, 1.0f);
			Add(lampHead);
			Add(lampPost);
		}

		public override void Update(float timeSinceLastUpdate)
		{
			if((derivedPosition - Camera.Instance.position).Length < ActionTrigger.maxDistance)
			{
				ActionTrigger.Display("toggle lights");
				if(ActionTrigger.onActive)
				{
					foreach(Lamp lamp in connectedLamps.children.Values)
					{
						lamp.QueueMethod(Lamp.Switch);
					}
				}
			}
		}
	}
}
