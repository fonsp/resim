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
		private Node lamps1 = new Node("lamps1");
		private Node lamps2 = new Node("lamps2");
		private LampSwitch lampSwitch1, lampSwitch2, lampSwitch3, lampSwitch4, lampSwitch5, lampSwitch6, lampSwitch7, lampSwitch8;
		private List<Entity> slides = new List<Entity>();
		private Node slideNode = new Node("slides");
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
					BasicClock clock = new BasicClock("Clock" + x.ToString("F0") + "_" + z.ToString("F0"));
					clock.position.X = x;
					clock.position.Z = z;
					clock.position.Y = 100;
					clocks.Add(clock);
				}
			}

			cannon.position = new Vector3(3700f, 0f, -5300f);

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
			testHueScale.position = new Vector3(3700f, 75f, -5300f);

			Lamp.lampHeadMesh.material.textureName = Lamp.lampPostMesh.material.textureName = "white";
			Lamp.lampHeadMesh.useVBO = true;
			Lamp.lampHeadMesh.GenerateVBO();
			Lamp.lampPostMesh.useVBO = true;
			Lamp.lampPostMesh.GenerateVBO();

			for(float x = 2500; x <= 3500; x += 250)
			{
				for(float z = -6550; z <= -5550; z += 250)
				{
					Lamp lamp = new Lamp("lamp1_" + x.ToString("F0") + "_" + z.ToString("F0"));
					lamp.position.X = x;
					lamp.position.Z = z;
					lamp.position.Y = 75f;
					lamps1.Add(lamp);
				}
			}

			for(float z = -6250; z <= -5850; z += 200)
			{
				Lamp lamp = new Lamp("lamp2_" + z.ToString("F0"));
				lamp.position.X = 1600f;
				lamp.position.Z = z;
				lamp.position.Y = 150f;
				lamps2.Add(lamp);
			}


			lampSwitch1 = new LampSwitch("lampswitch1", lamps1);
			lampSwitch1.position = new Vector3(1950f, 150f, -6050f);
			lampSwitch2 = new LampSwitch("lampswitch2", lamps1);
			lampSwitch2.position = new Vector3(3750f, 75f, -6050f);
			lampSwitch3 = new LampSwitch("lampswitch3", lamps1);
			lampSwitch3.position = new Vector3(3000f, 75f, -6830f);
			lampSwitch4 = new LampSwitch("lampswitch4", lamps1);
			lampSwitch4.position = new Vector3(3000f, 75f, -5250f);
			lampSwitch5 = new LampSwitch("lampswitch5", lamps2);
			lampSwitch5.position = new Vector3(1950f, 150f, -6050f);
			lampSwitch6 = new LampSwitch("lampswitch6", lamps2);
			lampSwitch6.position = new Vector3(1600f, 264f, -5475f);
			lampSwitch7 = new LampSwitch("lampswitch7", lamps2);
			lampSwitch7.position = new Vector3(1600f, 264f, -6625f);
			lampSwitch8 = new LampSwitch("lampswitch8", lamps1);
			lampSwitch8.position = new Vector3(2630f, 715f, -5700f);

			for(int i = 0; i < config.GetInt("numSlides"); i++)
			{
				Entity slide = new Entity("slide" + i.ToString("D2"));
				slide.mesh = new Mesh();
				slide.mesh.polygonList = slideMesh.polygonList;
				slide.mesh.material.textureName = "slide" + i.ToString("D2");
				slide.position.Z = - 100 * i;
				slides.Add(slide);
				slideNode.Add(slide);
			}

			slideNode.position = new Vector3(10f, 249.6f, -5100f);

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
			//RootNode.Instance.Add(clocks);
			RootNode.Instance.Add(cannon);
			RootNode.Instance.Add(testHueScale);
			RootNode.Instance.Add(lamps1);
			RootNode.Instance.Add(lamps2);
			RootNode.Instance.Add(lampSwitch1);
			RootNode.Instance.Add(lampSwitch2);
			RootNode.Instance.Add(lampSwitch3);
			RootNode.Instance.Add(lampSwitch4);
			RootNode.Instance.Add(lampSwitch5);
			RootNode.Instance.Add(lampSwitch6);
			RootNode.Instance.Add(lampSwitch7);
			RootNode.Instance.Add(lampSwitch8);
			RootNode.Instance.Add(slideNode);

			#endregion

			Camera.Instance.position = new Vector3(2700, 250, -6075);

			RenderWindow.Instance.KeyPress += HandleKeyPress;
			RenderWindow.Instance.Title = "ReSim";
			InputManager.CursorLockState = CursorLockState.Centered;
			InputManager.HideCursor();

			mapCollision.Add(slideAABB);
		}
	}
}