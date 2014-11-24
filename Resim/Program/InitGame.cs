using System;
using System.Collections.Generic;
using OpenTK;
using GraphicsLibrary;
using GraphicsLibrary.Core;
using GraphicsLibrary.Hud;
using GraphicsLibrary.Input;
using GraphicsLibrary.Collision;

namespace Resim.Program
{
	public partial class Game
	{
		private Node map = new Node("map");
		private Random random = new Random();

		private List<CollisionAABB> mapCollision = new List<CollisionAABB>();

		public override void InitGame()
		{
			#region Program arguments

			//TODO: Program arguments

			#endregion
			#region Entities

			skybox.mesh.material.textureName = "skybox";
			skybox.isLit = false;
			float size = Camera.Instance.ZFar / 1.8f;//Sqrt(3)
			skybox.scale = new Vector3(size, size, size);

			collisionVisuals.mesh.material.textureName = "map0e";
			collisionVisuals.wireFrame = true;
			collisionVisuals.mesh.shader = Shader.collisionShaderCompiled;
			collisionVisuals.writeDepthBuffer = false;
			collisionVisuals.readDepthBuffer = false;
			collisionVisuals.renderPass = 1;

			map1.mesh.material.textureName = "white";
			map1.mesh.useVBO = true;
			map1.mesh.GenerateVBO();

			hudConsole.enabled = false;
			hudConsole.isVisible = false;
			hudConsole.FontTextureName = "font0";
			hudConsole.NumberOfLines = 30;
			hudConsole.DebugInput += ConsoleInputReceived;

			crossHair.width = crossHair.height = 32;

			Camera.Instance.friction = new Vector3((float)config.GetDouble("playerFriction"), 1, (float)config.GetDouble("playerFriction"));

			monster.mesh.material.textureName = "monsterTexture";

			playerMesh.material.textureName = "playerTexture";

			//HudBase.Instance.Add(grainImage);
			HudBase.Instance.Add(hudConsole);
			HudBase.Instance.Add(crossHair);

			RootNode.Instance.Add(monster);
			RootNode.Instance.Add(skybox);
			RootNode.Instance.Add(collisionVisuals);
			RootNode.Instance.Add(map);
			/*map.Add(map0a);
			map.Add(map0b);
			map.Add(map0c);
			map.Add(map0d);
			map.Add(map0e);*/
			map.Add(map1);

			#endregion

			Camera.Instance.position = new Vector3(2700, 300, -6075);

			RenderWindow.Instance.KeyPress += HandleKeyPress;
			RenderWindow.Instance.Title = "ReSim";
			InputManager.CursorLockState = CursorLockState.Centered;
			InputManager.HideCursor();

			mapCollision.Add(monsterAABB);
		}
	}
}