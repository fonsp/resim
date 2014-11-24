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
		private Entity monster = new Entity("monster");
		private Mesh playerMesh;
		private HudConsole hudConsole = new HudConsole("HudConsole", 5);
		private HudImage crossHair = new HudImage("crosshair", "crosshair0");
		private HudImage crossHair1 = new HudImage("crosshair1", "crosshair0");
		private CollisionAABB playerAABB;

		public override void LoadResources()
		{
			config = new Config("Game.ini");

			TextureManager.AddTexture("monsterTexture", @"Content/textures/monsterText1.png");
			TextureManager.AddTexture("skybox", @"Content/textures/skybox.png");
			TextureManager.AddTexture("font0", @"Content/textures/font0.png");
			TextureManager.AddTexture("font1", @"Content/textures/font1.png");
			TextureManager.AddTexture("crosshair0", @"Content/textures/crosshair0.png");
			TextureManager.AddTexture("white", @"Content/textures/white.png");
			TextureManager.AddTexture("map0a", @"Content/textures/map0/darkBrick.png", OpenTK.Graphics.OpenGL.TextureMinFilter.Linear, OpenTK.Graphics.OpenGL.TextureMagFilter.Linear);
			TextureManager.AddTexture("map0b", @"Content/textures/map0/rockWall.png", OpenTK.Graphics.OpenGL.TextureMinFilter.Linear, OpenTK.Graphics.OpenGL.TextureMagFilter.Linear);
			TextureManager.AddTexture("map0c", @"Content/textures/map0/crate.png", OpenTK.Graphics.OpenGL.TextureMinFilter.Linear, OpenTK.Graphics.OpenGL.TextureMagFilter.Linear);
			TextureManager.AddTexture("map0d", @"Content/textures/map0/metal.png", OpenTK.Graphics.OpenGL.TextureMinFilter.Linear, OpenTK.Graphics.OpenGL.TextureMagFilter.Linear);
			TextureManager.AddTexture("map0e", @"Content/textures/floor0.png", OpenTK.Graphics.OpenGL.TextureMinFilter.Linear, OpenTK.Graphics.OpenGL.TextureMagFilter.Linear);
			TextureManager.AddTexture("playerTexture", @"Content/textures/player.png");

			playerMesh = ObjConverter.ConvertObjToMesh(File.ReadAllText(@"Content/models/player.obj"));
			monster.mesh = ObjConverter.ConvertObjToMesh(File.ReadAllText(@"Content/models/monsterUVd.obj"), new Vector3(101, -19, 205));
			collisionVisuals.mesh = ObjConverter.ConvertObjToMesh(File.ReadAllText(@"Content/models/map1/collision.obj"));
			skybox.mesh = ObjConverter.ConvertObjToMesh(File.ReadAllText(@"Content/models/skybox3.obj"));
			map1.mesh = ObjConverter.ConvertObjToVboMesh(File.ReadAllText(@"Content/models/map1/map1.obj")); //TODO: offset

			mapCollision.AddRange(ObjConverter.ConvertObjToAABBarray(File.ReadAllText(@"Content/models/map1/collision.obj")));
			playerAABB = ObjConverter.ConvertObjToAABBarray(File.ReadAllText(@"Content/models/player.obj"))[0];

			BasicClock.clockMesh = ObjConverter.ConvertObjToMesh(File.ReadAllText(@"Content/models/player.obj"));
			BasicClock.clockMesh.material.textureName = "playerTexture";
		}
	}
}