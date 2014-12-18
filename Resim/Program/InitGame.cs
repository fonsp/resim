using System;
using System.Collections.Generic;
using OpenTK;
using GraphicsLibrary;
using GraphicsLibrary.Core;
using GraphicsLibrary.Hud;
using GraphicsLibrary.Input;
using GraphicsLibrary.Collision;
using OpenTK.Graphics;

namespace Resim.Program
{
	public partial class Game
	{
		private Node map = new Node("map");
		private Node clocks = new Node("clocks");

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

			BasicClock.clockMesh.useVBO = true;
			BasicClock.clockMesh.GenerateVBO();

			for(float x = 2000; x <= 4000; x += 500)
			{
				for(float z = -7000; z <= -5000; z += 500)
				{
					BasicClock clock = new BasicClock("Clock" + x.ToString("F0") + z.ToString("F0"));
					clock.position.X = x;
					clock.position.Z = z;
					clock.position.Y = 100;
					clocks.Add(clock);
				}
			}

			hudConsole.enabled = false;
			hudConsole.isVisible = false;
			hudConsole.FontTextureName = "font2";
			hudConsole.NumberOfLines = 30;
			hudConsole.DebugInput += ConsoleInputReceived;

			crossHair.width = crossHair.height = 32;
			crossHair.color = Color4.LightBlue;
			crossHair1.width = crossHair1.height = 32;
			crossHair1.color = Color4.Orange;

			Camera.Instance.friction = new Vector3((float)config.GetDouble("playerFriction"), 1, (float)config.GetDouble("playerFriction"));

			monster.mesh.material.textureName = "huescale";//"monsterTexture";

			testHueScale.mesh.material.textureName = "huescale";
			testHueScale.position = new Vector3(3000, 100, -6000);

			map.Add(map1);

			HudBase.Instance.Add(hudDebug);
			HudBase.Instance.Add(hudConsole);
			//HudBase.Instance.Add(crossHair);
			//HudBase.Instance.Add(crossHair1);

			RootNode.Instance.Add(monster);
			RootNode.Instance.Add(skybox);
			RootNode.Instance.Add(collisionVisuals);
			RootNode.Instance.Add(map);
			RootNode.Instance.Add(clocks);
			RootNode.Instance.Add(testHueScale);

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