// RenderWindow.cs
//
// Copyright 2013 Fons van der Plas
// Fons van der Plas, fonsvdplas@gmail.com

//#define FULLSCREEN

#region References
using System;
using System.Diagnostics;
using System.Drawing;
using GraphicsLibrary.Content;
using GraphicsLibrary.Core;
using GraphicsLibrary.Hud;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

using GraphicsLibrary.Timing;
#endregion

namespace GraphicsLibrary
{
	/* De RenderWindow class is de basis van de 3D-applicatie
	 * Hierin wordt het grootste deel van de communicatie met de GPU gedaan
	 * 
	 * De class is een singleton, omdat er in mijn spel maar 1 venster hoeft te zijn, en alle klassen toegang tot hetzelfde venster moet hebben
	 * 
	 * RenderWindow overridet GameWindow, een class van OpenTK.
	 */
	public class RenderWindow : GameWindow
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

		public RenderWindow(string windowName, int width, int height): base(width, height, GraphicsMode.Default, windowName
#if FULLSCREEN
			, GameWindowFlags.Fullscreen
#endif
			)
		{
			VSync = VSyncMode.On;
			
		}
		public RenderWindow(string windowName): this(windowName, 1280, 720)
		{ }

		public RenderWindow(): this("Default render window")
		{ }

		/* Deze functie wordt door OpenTK opgeroepen als het spel start
		 * In deze functie wordt OpeGL opgestart met de juiste parameters
		 * Vanaf hier worden ook verschillende functies van GraphicsProgram opgeroepen (zie GraphicsProgram.cs)
		 * 
		 * Alle GL-functies staan in een try-catch constructie zodat evt. errors opgeslagen worden in het .log-bestand
		 * zodat ik fouten wat makkelijker kan opsporen op PC's zonder MonoDevelop of Visual Studio.
		 */
		protected override void OnLoad(EventArgs e)
		{
			Debug.WriteLine("Initializing OpenGL..");
			#region General stuffz
			this.WindowBorder = OpenTK.WindowBorder.Resizable;

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
			catch (Exception exception)
			{
				Debug.WriteLine("WARNING: OpenGL could not be initialized: {0}", exception.Message);
				Exit();
			}

			#endregion
			Debug.WriteLine("Initializing textures..");
			#region Texture loading

			try
			{
				TextureManager.AddTexture("default", @"Content/textures/defaultTexture.png", TextureMinFilter.Linear, TextureMagFilter.Nearest);

				GL.BindTexture(TextureTarget.ProxyTexture2D, TextureManager.GetTexture("default"));

				GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, Color4.Black);
				GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, new float[] {50.0f});
			}
			catch (Exception exception)
			{
				Debug.WriteLine("WARNING: default textures could not be initialized: {0}", exception.Message);
				Exit();
			}

			#endregion
			Debug.WriteLine("Initializing lighting..");
			#region Init lighting

			try
			{
				GL.Light(LightName.Light0, LightParameter.Ambient, new float[] {.4f, .4f, .4f, 0.0f});
				GL.Light(LightName.Light0, LightParameter.Diffuse, new float[] {.95f, .95f, .95f, 0.0f});
				GL.Light(LightName.Light0, LightParameter.Position, new float[] {.0f, .0f, .0f, 0.0f});

				GL.Enable(EnableCap.Lighting);
				GL.Enable(EnableCap.Light0);
			}
			catch (Exception exception)
			{
				Debug.WriteLine("WARNING: lighting could not be initialized: {0}", exception.Message);
				Exit();
			}

			#endregion
			Debug.WriteLine("Initializing camera..");
			#region Camera init
			Camera.Instance.Fov = 120f;
			Camera.Instance.width = Width;
			Camera.Instance.height = Height;
			OnResize(new EventArgs());
			#endregion
			Debug.WriteLine("Loading custom resources..");
			#region Custom resources
			/*try
			{*/
				program.LoadResources();
			/*}
			catch (Exception exception)
			{
				Debug.WriteLine("WARNING: custom resources could not be loaded: {0}", exception.Message);
			}*/
			#endregion
			Debug.WriteLine(TextureManager.numberOfTextures + " textures were loaded");
			Debug.WriteLine("Initializing game..");
			program.InitGame();
			updateSw.GetElapsedTimeInSeconds();
			Debug.WriteLine("Loading complete");
		}

		/* Deze functie wordt door OpenTK opgeroepen als het spel sluit
		 */
		protected override void OnUnload(EventArgs e)
		{
			Debug.WriteLine("Unloading textures..");

			try
			{
				TextureManager.ClearTextureCache();
			}
			catch (Exception exception)
			{
				Debug.WriteLine("WARNING: Failed to unload textures: {0}", exception.Message);
				throw;
			}

			Debug.WriteLine("Unloading resources complete");
		}

		/* Deze functie wordt door OpenTK opgeroepen als het venster van grootte verandert
		 * Hierin wordt OpenGL, Camera en GraphicsProgram geupdate
		 */
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

		/* Deze functie wordt door OpenTK opgeroepen na OnRenderFrame
		 * Deze functie werkt dus parrallel met de GPU
		 * Hierin wordt GraphicsProgram geupdate, en alle Nodes worden geupdate (zie Node.cs)
		 */
		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			if (Keyboard[Key.Escape] && escapeOnEscape/* && Keyboard[Key.Pause]*/)
			{
				Exit();
			}
#if DEBUG
			/*if (Keyboard[Key.Pause] && !Keyboard[Key.Escape])
			{
				Debugger.Break();
			}*/
#endif
			timeSinceLastUpdate = e.Time * timeMultiplier;

			program.Update((float)timeSinceLastUpdate);
			if (enableVelocity)
			{
				
				RootNode.Instance.Update((float)timeSinceLastUpdate);
				if (Camera.Instance.parent == null)
				{
					Camera.Instance.Update((float)timeSinceLastUpdate);
				}
				HudBase.Instance.Update((float)timeSinceLastUpdate);
			} else {
				RootNode.Instance.Update();
				if (Camera.Instance.parent == null)
				{
					Camera.Instance.Update();
				}
				HudBase.Instance.Update();
			}
			
			
			// End of engine update
		}

		/* Deze functie wordt door OpenTK opgeroepen nadat de GPU de buffers omdraait
		 * Hierin worden dus alle OpenGL-functies opgeroepen
		 * 
		 * Nadat de 3D-scene klaar is wordt OpenGL in een 2D-modus gezet en wordt de HUD gerendert
		 */
		protected override void OnRenderFrame(FrameEventArgs e)
		{
			#region 3D
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			UpdateViewport();
			

			Matrix4 modelview = Matrix4.LookAt(Camera.Instance.position, Camera.Instance.position - Vector3.UnitZ, Vector3.UnitY) * Matrix4.Rotate(Camera.Instance.derivedOrientation);
			
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadMatrix(ref modelview);

			GL.Light(LightName.Light0, LightParameter.Position, new float[] { .8f, .9f, 1.0f, 0.0f });

			for (int i = 0; i < amountOfRenderPasses; i++)
			{
				RootNode.Instance.StartRender(i);
				
			}

			/*modelview = Matrix4.LookAt(Camera.Instance.position, Camera.Instance.position - Vector3.UnitZ, Vector3.UnitY);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadMatrix(ref modelview);*/

			for (int i = 0; i < amountOfRenderPasses; i++)
			{
				if (Camera.Instance.parent == null)
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