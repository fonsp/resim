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
		private Entity ground = new Entity("ground");
		private Entity map0a = new Entity("map0a");
		private Entity map0b = new Entity("map0b");
		private Entity map0c = new Entity("map0c");
		private Entity map0d = new Entity("map0d");
		private Entity map0e = new Entity("map0e");
		private Entity beam = new Entity("gunBeam");
		private Entity flashA = new Entity("flashA");
		private Entity flashB = new Entity("flashB");
		private Entity monster = new Entity("monster");
		private Mesh playerMesh;
		private HudDebug hudDebug = new HudDebug("hudDebug", 5);
		private HudImage crossHair = new HudImage("crosshair", "crosshair0");
		private HudImage grainImage = new HudImage("grainImage", "grain0");
		private TextField[] comboTextFields = new TextField[200];
		private CollisionAABB playerAABB;

		public override void LoadResources()
		{
			config = new Config("Game.ini");

			TextureManager.AddTexture("monsterTexture", @"Content/textures/monsterText1.png");
			TextureManager.AddTexture("bush", @"Content/textures/bush1.png");
			TextureManager.AddTexture("skybox", @"Content/textures/skybox.png");
			TextureManager.AddTexture("heavyTank", @"Content/textures/heavytank.png");
			TextureManager.AddTexture("font0", @"Content/textures/font0.png");
			TextureManager.AddTexture("font1", @"Content/textures/font1.png");
			TextureManager.AddTexture("font2", @"Content/textures/comboFont.png");
			TextureManager.AddTexture("crosshair0", @"Content/textures/crosshair0.png");
			TextureManager.AddTexture("white", @"Content/textures/white.png");
			TextureManager.AddTexture("beam0", @"Content/textures/beam0.png");
			TextureManager.AddTexture("grain0", @"Content/textures/grain3200alpha.png");
			TextureManager.AddTexture("flashA", @"Content/textures/flashA.png");
			TextureManager.AddTexture("flashB", @"Content/textures/flashB.png");
			TextureManager.AddTexture("map0a", @"Content/textures/map0/darkBrick.png", OpenTK.Graphics.OpenGL.TextureMinFilter.Linear, OpenTK.Graphics.OpenGL.TextureMagFilter.Linear);
			TextureManager.AddTexture("map0b", @"Content/textures/map0/rockWall.png", OpenTK.Graphics.OpenGL.TextureMinFilter.Linear, OpenTK.Graphics.OpenGL.TextureMagFilter.Linear);
			TextureManager.AddTexture("map0c", @"Content/textures/map0/crate.png", OpenTK.Graphics.OpenGL.TextureMinFilter.Linear, OpenTK.Graphics.OpenGL.TextureMagFilter.Linear);
			TextureManager.AddTexture("map0d", @"Content/textures/map0/metal.png", OpenTK.Graphics.OpenGL.TextureMinFilter.Linear, OpenTK.Graphics.OpenGL.TextureMagFilter.Linear);
			TextureManager.AddTexture("map0e", @"Content/textures/floor0.png", OpenTK.Graphics.OpenGL.TextureMinFilter.Linear, OpenTK.Graphics.OpenGL.TextureMagFilter.Linear);
			TextureManager.AddTexture("playerTexture", @"Content/textures/player.png");

			playerMesh = ObjConverter.ConvertObjToMesh(File.ReadAllText(@"Content/models/player.obj"));
			monster.mesh = ObjConverter.ConvertObjToMesh(File.ReadAllText(@"Content/models/monsterUVd.obj"), new Vector3(101, -19, 205));
			ground.mesh = ObjConverter.ConvertObjToMesh(File.ReadAllText(@"Content/models/floor1s.obj"));
			skybox.mesh = ObjConverter.ConvertObjToMesh(File.ReadAllText(@"Content/models/skybox3.obj"));
			beam.mesh = ObjConverter.ConvertObjToMesh(File.ReadAllText(@"Content/models/roundBeam.obj"));
			flashA.mesh = ObjConverter.ConvertObjToMesh(File.ReadAllText(@"Content/models/flashA.obj"));
			flashB.mesh = ObjConverter.ConvertObjToMesh(File.ReadAllText(@"Content/models/flashB.obj"));
			map0a.mesh = ObjConverter.ConvertObjToMesh(File.ReadAllText(@"Content/models/map0/map0a.obj"));
			map0b.mesh = ObjConverter.ConvertObjToMesh(File.ReadAllText(@"Content/models/map0/map0b.obj"));
			map0c.mesh = ObjConverter.ConvertObjToMesh(File.ReadAllText(@"Content/models/map0/map0c.obj"));
			map0d.mesh = ObjConverter.ConvertObjToMesh(File.ReadAllText(@"Content/models/map0/map0d.obj"));
			map0e.mesh = ObjConverter.ConvertObjToMesh(File.ReadAllText(@"Content/models/map0/map0e.obj"));

			mapCollision.AddRange(ObjConverter.ConvertObjToAABBarray(File.ReadAllText(@"Content/models/map0/collision.obj")));

			playerAABB = ObjConverter.ConvertObjToAABBarray(File.ReadAllText(@"Content/models/player.obj"))[0];
		}
	}
}