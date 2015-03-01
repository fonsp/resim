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
		private Cannon cannon = new Cannon("cannon");

		private List<CollisionAABB> mapCollision = new List<CollisionAABB>();

		public override void InitGame()
		{
			#region Program arguments

			//TODO: Program arguments

			#endregion
			#region Entities

			skybox.mesh.material.textureName = "skybox";
			skybox.isLit = false;

			collisionVisuals.mesh.material.textureName = "map0e";
			collisionVisuals.wireFrame = true;
			collisionVisuals.mesh.shader = Shader.collisionShaderCompiled;
			collisionVisuals.writeDepthBuffer = false;
			collisionVisuals.readDepthBuffer = false;
			collisionVisuals.renderPass = 1;

			map1.mesh.material.textureName = "white";
			map1.mesh.useVBO = true;
			map1.mesh.GenerateVBO();
			map2a.mesh.material.textureName = "white";
			map2b.mesh.material.textureName = "white";
			map2c.mesh.material.textureName = "white";
			map2d.mesh.material.textureName = "white";
			map2e.mesh.material.textureName = "white";
			map2f.mesh.material.textureName = "white";
			map2a.mesh.useVBO = true;
			map2b.mesh.useVBO = true;
			map2c.mesh.useVBO = true;
			map2d.mesh.useVBO = true;
			map2e.mesh.useVBO = true;
			map2f.mesh.useVBO = true;
			map2a.mesh.GenerateVBO();
			map2b.mesh.GenerateVBO();
			map2c.mesh.GenerateVBO();
			map2d.mesh.GenerateVBO();
			map2e.mesh.GenerateVBO();
			map2f.mesh.GenerateVBO();
			map2a.mesh.material.baseColor = new Color4(1.0f, 1.0f, 1.0f, 1.0f);
			map2b.mesh.material.baseColor = new Color4(0.2f, 0.2f, 0.2f, 1.0f);
			map2c.mesh.material.baseColor = new Color4(1.0f, 0.9f, 0.6f, 1.0f);
			map2d.mesh.material.baseColor = new Color4(0.1f, 0.1f, 0.1f, 1.0f);
			map2e.mesh.material.baseColor = new Color4(0.3f, 0.3f, 0.3f, 1.0f);
			map2f.mesh.material.baseColor = new Color4(0.5f, 0.9f, 0.1f, 1.0f);

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

			cannon.position = new Vector3(3700f, 250f, -5300f);

			hudConsole.enabled = false;
			hudConsole.isVisible = false;
			hudConsole.FontTextureName = "font2";
			hudConsole.NumberOfLines = 30;
			hudConsole.DebugInput += ConsoleInputReceived;

			speedometerBase.width = 256f;
			speedometerBase.height = 64f;
			speedometerPointer.width = 5f;
			speedometerPointer.height = 68f;

			Camera.Instance.friction = new Vector3((float)config.GetDouble("playerFriction"), 1, (float)config.GetDouble("playerFriction"));

			monster.mesh.material.textureName = "huescale";

			testHueScale.mesh.material.textureName = "huescale";
			testHueScale.position = new Vector3(3000, 100, -6000);

			//map.Add(map1);
			map.Add(map2a);
			map.Add(map2b);
			map.Add(map2c);
			map.Add(map2d);
			map.Add(map2e);
			map.Add(map2f);

			HudBase.Instance.Add(hudDebug);
			HudBase.Instance.Add(hudConsole);
			HudBase.Instance.Add(speedometerBase);
			HudBase.Instance.Add(speedometerPointer);
			HudBase.Instance.Add(ActionTrigger.textField);

			RootNode.Instance.Add(monster);
			RootNode.Instance.Add(skybox);
			RootNode.Instance.Add(collisionVisuals);
			RootNode.Instance.Add(map);
			RootNode.Instance.Add(clocks);
			RootNode.Instance.Add(cannon);
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