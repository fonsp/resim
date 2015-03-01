using System.IO;
using OpenTK;
using GraphicsLibrary.Content;
using GraphicsLibrary.Core;
using GraphicsLibrary.Hud;
using GraphicsLibrary.Collision;

namespace Resim.Program
{
	public partial class Game
	{
		private Entity skybox = new Entity("skybox");
		private Entity collisionVisuals = new Entity("collisionVisuals");
		private Entity map1 = new Entity("map1");
		private Entity map2a = new Entity("map2a");
		private Entity map2b = new Entity("map2b");
		private Entity map2c = new Entity("map2c");
		private Entity map2d = new Entity("map2d");
		private Entity map2e = new Entity("map2e");
		private Entity map2f = new Entity("map2f");
		private Entity monster = new Entity("monster");
		private Entity testHueScale = new Entity("testHueScale");
		private HudConsole hudConsole = new HudConsole("hudConsole", 5);
		private HudDebug hudDebug = new HudDebug("hudDebug");
		private HudImage speedometerBase = new HudImage("spedb", "spedb");
		private HudImage speedometerPointer = new HudImage("spedp", "spedp");
		private CollisionAABB playerAABB;

		public override void LoadResources()
		{
			config = new Config("Game.ini");

			TextureManager.AddTexture("monsterTexture", @"Content/textures/monsterText1.png");
			TextureManager.AddTexture("skybox", @"Content/textures/skybox.png");
			TextureManager.AddTexture("font0", @"Content/textures/font0.png");
			TextureManager.AddTexture("font1", @"Content/textures/font1.png");
			TextureManager.AddTexture("font2", @"Content/textures/font2.png");
			TextureManager.AddTexture("crosshair0", @"Content/textures/crosshair0.png");
			TextureManager.AddTexture("white", @"Content/textures/white.png");
			TextureManager.AddTexture("huescale", @"Content/textures/huescale.png");
			TextureManager.AddTexture("spedb", @"Content/textures/speedometer_base.png");
			TextureManager.AddTexture("spedp", @"Content/textures/speedometer_pointer.png");
			TextureManager.AddTexture("map0a", @"Content/textures/map0/darkBrick.png", OpenTK.Graphics.OpenGL.TextureMinFilter.Linear, OpenTK.Graphics.OpenGL.TextureMagFilter.Linear);
			TextureManager.AddTexture("map0b", @"Content/textures/map0/rockWall.png", OpenTK.Graphics.OpenGL.TextureMinFilter.Linear, OpenTK.Graphics.OpenGL.TextureMagFilter.Linear);
			TextureManager.AddTexture("map0c", @"Content/textures/map0/crate.png", OpenTK.Graphics.OpenGL.TextureMinFilter.Linear, OpenTK.Graphics.OpenGL.TextureMagFilter.Linear);
			TextureManager.AddTexture("map0d", @"Content/textures/map0/metal.png", OpenTK.Graphics.OpenGL.TextureMinFilter.Linear, OpenTK.Graphics.OpenGL.TextureMagFilter.Linear);
			TextureManager.AddTexture("map0e", @"Content/textures/floor0.png", OpenTK.Graphics.OpenGL.TextureMinFilter.Linear, OpenTK.Graphics.OpenGL.TextureMagFilter.Linear);
			TextureManager.AddTexture("playerTexture", @"Content/textures/player.png");

			testHueScale.mesh = ObjConverter.ConvertObjToMesh(File.ReadAllText(@"Content/models/monsterUVd.obj"), new Vector3(101, -19, 205));
			monster.mesh = ObjConverter.ConvertObjToMesh(File.ReadAllText(@"Content/models/monsterUVd.obj"), new Vector3(101, -19, 205));
			collisionVisuals.mesh = ObjConverter.ConvertObjToMesh(File.ReadAllText(@"Content/models/map1/collision.obj"));
			skybox.mesh = ObjConverter.ConvertObjToMesh(File.ReadAllText(@"Content/models/skybox3.obj"));
			map1.mesh = ObjConverter.ConvertObjToVboMesh(File.ReadAllText(@"Content/models/map1/map1.obj")); //TODO: offset
			map2a.mesh = ObjConverter.ConvertObjToVboMesh(File.ReadAllText(@"Content/models/map2/map2a.obj"));
			map2b.mesh = ObjConverter.ConvertObjToVboMesh(File.ReadAllText(@"Content/models/map2/map2c.obj"));
			map2c.mesh = ObjConverter.ConvertObjToVboMesh(File.ReadAllText(@"Content/models/map2/map2b.obj"));
			map2d.mesh = ObjConverter.ConvertObjToVboMesh(File.ReadAllText(@"Content/models/map2/map2d.obj"));
			map2e.mesh = ObjConverter.ConvertObjToVboMesh(File.ReadAllText(@"Content/models/map2/map2e.obj"));
			map2f.mesh = ObjConverter.ConvertObjToVboMesh(File.ReadAllText(@"Content/models/map2/map2f.obj"));

			mapCollision.AddRange(ObjConverter.ConvertObjToAABBarray(File.ReadAllText(@"Content/models/map1/collision.obj")));
			playerAABB = ObjConverter.ConvertObjToAABBarray(File.ReadAllText(@"Content/models/player.obj"))[0];

			BasicClock.clockMesh = ObjConverter.ConvertObjToVboMesh(File.ReadAllText(@"Content/models/player.obj"));
			BasicClock.clockMesh.material.textureName = "huescale";
		}
	}
}