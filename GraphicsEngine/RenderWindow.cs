//#define FULLSCREEN

#region References
using System;
using System.Diagnostics;
using System.Drawing;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

using GraphicsLibrary.Timing;
using GraphicsLibrary.Content;
using GraphicsLibrary.Core;
using GraphicsLibrary.Hud;
#endregion

namespace GraphicsLibrary
{
	public class RenderWindow:GameWindow
	{
		#region SingleTon
		public GraphicsProgram program;

		private static RenderWindow instance;

		public static RenderWindow Instance
		{
			get { return instance ?? (instance = new RenderWindow()); }
		}
		#endregion
		#region Variables
		public bool escapeOnEscape = true;

		private readonly GameTimer updateSw = new GameTimer();
		public bool enableVelocity = true;

		protected double timeSinceLastUpdate = 0;
		public double timeMultiplier = 1;

		public int amountOfRenderPasses = 3;
		#endregion

		public RenderWindow(string windowName, int width, int height)
			: base(width, height, GraphicsMode.Default, windowName
#if FULLSCREEN
			, GameWindowFlags.Fullscreen
#endif
)
		{
			VSync = VSyncMode.On;

		}
		public RenderWindow(string windowName)
			: this(windowName, 1280, 720)
		{ }

		public RenderWindow()
			: this("Default render window")
		{ }

		protected override void OnLoad(EventArgs e)
		{
			#region General
			Debug.WriteLine("Initializing OpenGL..");
			WindowBorder = WindowBorder.Resizable;

			try
			{
				GL.ClearColor(Color.Black);

				GL.ShadeModel(ShadingModel.Smooth);
				GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
				//GL.Enable(EnableCap.ColorMaterial);
				GL.Enable(EnableCap.DepthTest);
				GL.Enable(EnableCap.Blend);
				GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
				GL.Enable(EnableCap.CullFace);
				GL.CullFace(CullFaceMode.Back);
			}
			catch(Exception exception)
			{
				Debug.WriteLine("WARNING: OpenGL could not be initialized: {0}", exception.Message);
				Exit();
			}

			#endregion
			#region Texture loading
			Debug.WriteLine("Initializing textures..");

			try
			{
				TextureManager.AddTexture("default", @"Content/textures/defaultTexture.png", TextureMinFilter.Linear, TextureMagFilter.Nearest);

				GL.BindTexture(TextureTarget.ProxyTexture2D, TextureManager.GetTexture("default"));

				GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, Color4.Black);
				GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, new float[] { 50.0f });
			}
			catch(Exception exception)
			{
				Debug.WriteLine("WARNING: default textures could not be initialized: {0}", exception.Message);
				Exit();
			}

			#endregion
			#region Lighting init
			Debug.WriteLine("Initializing lighting..");

			try
			{
				GL.Light(LightName.Light0, LightParameter.Ambient, new float[] { .4f, .4f, .4f, 0.0f });
				GL.Light(LightName.Light0, LightParameter.Diffuse, new float[] { .95f, .95f, .95f, 0.0f });
				GL.Light(LightName.Light0, LightParameter.Position, new float[] { .0f, .0f, .0f, 0.0f });

				GL.Enable(EnableCap.Lighting);
				GL.Enable(EnableCap.Light0);
			}
			catch(Exception exception)
			{
				Debug.WriteLine("WARNING: lighting could not be initialized: {0}", exception.Message);
				Exit();
			}

			#endregion
			#region Camera init
			Debug.WriteLine("Initializing camera..");

			Camera.Instance.Fov = 120f;
			Camera.Instance.width = Width;
			Camera.Instance.height = Height;
			OnResize(new EventArgs());

			#endregion
			#region Custom resources
			Debug.WriteLine("Loading custom resources..");

			/*try
			{*/
			program.LoadResources();
			/*}
			catch (Exception exception)
			{
				Debug.WriteLine("WARNING: custom resources could not be loaded: {0}", exception.Message);
			}*/

			Debug.WriteLine(TextureManager.numberOfTextures + " textures were loaded");

			#endregion
			#region Game init
			Debug.WriteLine("Initializing game..");

			program.InitGame();
			updateSw.GetElapsedTimeInSeconds();
			#endregion

			Debug.WriteLine("Loading complete");
		}

		protected override void OnUnload(EventArgs e)
		{
			Debug.WriteLine("Unloading textures..");

			try
			{
				TextureManager.ClearTextureCache();
			}
			catch(Exception exception)
			{
				Debug.WriteLine("WARNING: Failed to unload textures: {0}", exception.Message);
				throw;
			}

			Debug.WriteLine("Unloading resources complete");
		}

		protected override void OnResize(EventArgs e)
		{
			Debug.WriteLine("Window resized to " + ClientRectangle);

			GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientSize.Width, ClientSize.Height);

			Camera.Instance.width = Width;
			Camera.Instance.height = Height;

			UpdateViewport();

			program.Resize(ClientRectangle);
		}

		public void UpdateViewport()
		{
			Matrix4 proj = Camera.Instance.projection;
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadMatrix(ref proj);
		}

		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			if(Keyboard[Key.Escape] && escapeOnEscape/* && Keyboard[Key.Pause]*/)
			{
				Exit();
			}
#if DEBUG
			if(Keyboard[Key.Pause] && !Keyboard[Key.Escape])
			{
				Debugger.Break();
			}
#endif
			timeSinceLastUpdate = e.Time * timeMultiplier;

			program.Update((float)timeSinceLastUpdate);
			if(enableVelocity)
			{

				RootNode.Instance.Update((float)timeSinceLastUpdate);
				if(Camera.Instance.parent == null)
				{
					Camera.Instance.Update((float)timeSinceLastUpdate);
				}
				HudBase.Instance.Update((float)timeSinceLastUpdate);
			}
			else
			{
				RootNode.Instance.Update();
				if(Camera.Instance.parent == null)
				{
					Camera.Instance.Update();
				}
				HudBase.Instance.Update();
			}
		}

		protected override void OnRenderFrame(FrameEventArgs e)
		{
			#region 3D
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			UpdateViewport();


			Matrix4 modelview = Matrix4.LookAt(Camera.Instance.position, Camera.Instance.position - Vector3.UnitZ, Vector3.UnitY) * Matrix4.Rotate(Camera.Instance.derivedOrientation);

			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadMatrix(ref modelview);

			GL.Light(LightName.Light0, LightParameter.Position, new float[] { .8f, .9f, 1.0f, 0.0f });

			for(int i = 0; i < amountOfRenderPasses; i++)
			{
				RootNode.Instance.StartRender(i);

			}

			/*modelview = Matrix4.LookAt(Camera.Instance.position, Camera.Instance.position - Vector3.UnitZ, Vector3.UnitY);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadMatrix(ref modelview);*/

			for(int i = 0; i < amountOfRenderPasses; i++)
			{
				if(Camera.Instance.parent == null)
				{
					Camera.Instance.StartRender(i);
				}
			}

			#endregion
			#region HUD
			GL.DepthMask(false);
			GL.Disable(EnableCap.DepthTest);
			GL.Disable(EnableCap.Lighting);
			GL.Disable(EnableCap.CullFace);

			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			GL.Ortho(0, ClientRectangle.Width, ClientRectangle.Height, 0, -1, 10);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadIdentity();
			//////////////

			HudBase.Instance.StartRender();

			//////////////
			GL.Color3(Color.White);
			GL.DepthMask(true);
			GL.Enable(EnableCap.DepthTest);
			GL.Enable(EnableCap.Lighting);
			GL.Enable(EnableCap.CullFace);
			#endregion

			SwapBuffers();
		}
	}
}