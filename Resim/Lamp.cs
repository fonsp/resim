using GraphicsLibrary.Core;
using OpenTK;
using OpenTK.Graphics;

namespace Resim
{
	public class Lamp:Entity
	{
		public static Mesh lampHeadMesh, lampPostMesh;
		public Entity lampHead, lampPost;
		public bool enabled = true;

		public Lamp(string name)
			: base(name)
		{
			lampHead = new Entity(name + "_head");
			lampHead.mesh = new Mesh();
			lampHead.mesh.polygonList = lampHeadMesh.polygonList;
			lampHead.mesh.vertexArray = lampHeadMesh.vertexArray;
			lampHead.mesh.useVBO = lampHeadMesh.useVBO;
			lampHead.mesh.hasVBO = lampHeadMesh.hasVBO;
			lampHead.mesh.VBOids = lampHeadMesh.VBOids;
			lampHead.position = new Vector3(0f, 60f, 0f);
			Add(lampHead);



			lampPost = new Entity(name + "_post");
			lampPost.mesh = new Mesh();
			lampPost.mesh.polygonList = lampPostMesh.polygonList;
			lampPost.mesh.vertexArray = lampPostMesh.vertexArray;
			lampPost.mesh.useVBO = lampPostMesh.useVBO;
			lampPost.mesh.hasVBO = lampPostMesh.hasVBO;
			lampPost.mesh.VBOids = lampPostMesh.VBOids;
			lampPost.mesh.material.baseColor = new Color4(0.1f, 0.1f, 0.1f, 1.0f);
			Add(lampPost);

			lampHead.mesh.material.textureName = lampPost.mesh.material.textureName = "white";
		}

		public void Enable()
		{
			lampHead.mesh.material.baseColor = new Color4(10.0f, 1.0f, 1.0f, 1.0f);
			enabled = true;
		}

		public void Disable()
		{
			lampHead.mesh.material.baseColor = new Color4(0.2f, 0.2f, 0.2f, 1.0f);
			enabled = false;
		}

		public static void Switch(Node lampNode)
		{
			Lamp lamp = (Lamp)lampNode;
			if(lamp.enabled)
			{
				lamp.Disable();
			}
			else
			{
				lamp.Enable();
			}
		}
	}
}