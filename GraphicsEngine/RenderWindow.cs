//#define FULLSCREEN

using System;
using System.Diagnostics;
using System.Drawing;
using GraphicsLibrary.Input;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using GraphicsLibrary.Timing;
using GraphicsLibrary.Content;
using GraphicsLibrary.Core;
using GraphicsLibrary.Hud;

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

		public bool escapeOnEscape = true;
		private readonly GameTimer updateSw = new GameTimer();
		private float _time = 0f;
		public float c = 2000f;
		public float v = 0f;
		public float b = 0f;
		public float lf = 1f;
		private Vector3 smoothedVelocity = Vector3.Zero;
		public float smoothFactor = 4000f;
		public float time { get { return _time; } }
		public bool enableVelocity = true;
		protected double timeSinceLastUpdate = 0;
		public double timeMultiplier = 1;
		public int amountOfRenderPasses = 3;
		public Shader defaultShader = Shader.diffuseShader;
		public uint[] elementBase = new uint[1000000];
		public uint FboHandle, ColorTexture, DepthRenderbuffer;

		public RenderWindow(string windowName, int width, int height)
			: base(width, height, GraphicsMode.Default, windowName
#if FULLSCREEN
			, GameWindowFlags.Fullscreen
#endif
)
		{
			VSync = VSyncMode.Off;

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

				for(uint i = 0; i < elementBase.Length; i++)
				{
					elementBase[i] = i;
				}
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
				GL.Light(LightName.Light0, LightParameter.Ambient, new float[] { .2f, .2f, .2f, 1.0f });
				GL.Light(LightName.Light0, LightParameter.Diffuse, new float[] { .95f, .95f, .95f, 1.0f });
				GL.Light(LightName.Light0, LightParameter.Position, Vector4.Normalize(new Vector4(.4f, -.9f, .5f, 0.0f)));

				GL.Enable(EnableCap.Lighting);
				GL.Enable(EnableCap.Light0);
			}
			catch(Exception exception)
			{
				Debug.WriteLine("WARNING: lighting could not be initialized: {0}", exception.Message);
				Exit();
			}

			#endregion
			#region Default shaders init
			Debug.WriteLine("Initializing default shaders..");
			//TODO: useless?

			try
			{
				Shader asdf = Shader.diffuseShaderCompiled;
				asdf = Shader.unlitShaderCompiled;
				asdf = null;
			}
			catch(Exception exception)
			{
				Debug.WriteLine("WARNING: default shader could not be initialized: {0}", exception.Message);
				Exit();
			}

			#endregion
			#region FBO init
			Debug.WriteLine("Initializing FBO..");

			Debug.WriteLine("FBO size: {" + Width + ", " + Height + "}");

			// Create Color Texture
			GL.GenTextures(1, out ColorTexture);
			GL.BindTexture(TextureTarget.Texture2D, ColorTexture);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

			// test for GL Error here (might be unsupported format)

			GL.BindTexture(TextureTarget.Texture2D, 0); // prevent feedback, reading and writing to the same image is a bad idea

			// Create Depth Renderbuffer
			GL.Ext.GenRenderbuffers(1, out DepthRenderbuffer);
			GL.Ext.BindRenderbuffer(RenderbufferTarget.RenderbufferExt, DepthRenderbuffer);
			GL.Ext.RenderbufferStorage(RenderbufferTarget.RenderbufferExt, (RenderbufferStorage)All.DepthComponent32, Width, Height);
			
			// test for GL Error here (might be unsupported format)

			// Create an FBO and attach the textures
			GL.Ext.GenFramebuffers(1, out FboHandle);
			GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, FboHandle);
			GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, ColorTexture, 0);
			GL.Ext.FramebufferRenderbuffer(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt, RenderbufferTarget.RenderbufferExt, DepthRenderbuffer);

			// now GL.Ext.CheckFramebufferStatus( FramebufferTarget.FramebufferExt ) can be called, check the end of this page for a snippet.

			

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

			//TODO: FBO

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
			_time += (float)timeSinceLastUpdate;

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
			#region Shader updates

			float renderTime = (float)e.Time; //TODO e.Time accuracy

			Shader.diffuseShaderCompiled.SetUniform("time", _time);
			Shader.unlitShaderCompiled.SetUniform("time", _time);
			Shader.depthShaderCompiled.SetUniform("time", _time);
			Shader.wireframeShaderCompiled.SetUniform("time", _time);
			Shader.collisionShaderCompiled.SetUniform("time", _time);

			Vector3 velocityDelta = Camera.Instance.velocity - smoothedVelocity;
			smoothedVelocity += velocityDelta - Vector3.Divide(velocityDelta, (float) Math.Pow(smoothFactor, renderTime));

			v = smoothedVelocity.Length;
			b = v / c;
			lf = 1f / (float)Math.Sqrt(1.0 - b);

			Shader.diffuseShaderCompiled.SetUniform("b", b);
			Shader.unlitShaderCompiled.SetUniform("b", b);
			Shader.depthShaderCompiled.SetUniform("b", b);
			Shader.wireframeShaderCompiled.SetUniform("b", b);
			Shader.collisionShaderCompiled.SetUniform("b", b);

			Vector3 vDir = smoothedVelocity.Normalized();

			Shader.diffuseShaderCompiled.SetUniform("vdir", vDir);
			Shader.unlitShaderCompiled.SetUniform("vdir", vDir);
			Shader.depthShaderCompiled.SetUniform("vdir", vDir);
			Shader.wireframeShaderCompiled.SetUniform("vdir", vDir);
			Shader.collisionShaderCompiled.SetUniform("vdir", vDir);

			Shader.diffuseShaderCompiled.SetUniform("cpos", Camera.Instance.position);
			Shader.unlitShaderCompiled.SetUniform("cpos", Camera.Instance.position);
			Shader.depthShaderCompiled.SetUniform("cpos", Camera.Instance.position);
			Shader.wireframeShaderCompiled.SetUniform("cpos", Camera.Instance.position);
			Shader.collisionShaderCompiled.SetUniform("cpos", Camera.Instance.position);

			Matrix4 cRot = Matrix4.CreateFromQuaternion(Camera.Instance.derivedOrientation);

			Shader.diffuseShaderCompiled.SetUniform("crot", cRot);
			Shader.unlitShaderCompiled.SetUniform("crot", cRot);
			Shader.depthShaderCompiled.SetUniform("crot", cRot);
			Shader.wireframeShaderCompiled.SetUniform("crot", cRot);
			Shader.collisionShaderCompiled.SetUniform("crot", cRot);

			#endregion
			#region 3D

			
			UpdateViewport();

			//Matrix4 modelview = /*Matrix4.LookAt(Vector3.Zero, Vector3.Zero-Vector3.UnitZ, Vector3.UnitY) * */Matrix4.CreateFromQuaternion(Camera.Instance.derivedOrientation);

			GL.MatrixMode(MatrixMode.Modelview);

			//GL.LoadMatrix(ref modelview);
			GL.LoadIdentity();

			// since there's only 1 Color buffer attached this is not explicitly required
			GL.Ext.BindRenderbuffer(RenderbufferTarget.RenderbufferExt, DepthRenderbuffer);
			GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, FboHandle);
			GL.DrawBuffer((DrawBufferMode)FramebufferAttachment.ColorAttachment0Ext);
			GL.PushAttrib(AttribMask.ViewportBit); // stores GL.Viewport() parameters
			GL.Viewport(0, 0, Width, Height);

			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			//////////////

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

			//////////////
			GL.PopAttrib(); // restores GL.Viewport() parameters
			GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0); // return to visible framebuffer
			GL.DrawBuffer(DrawBufferMode.Back);

			GL.Ext.BindRenderbuffer(RenderbufferTarget.RenderbufferExt, 0);
			GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);

			Shader.hudShaderCompiled.Enable();
			GL.Color4(Color4.White);

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




			if (InputManager.IsKeyToggled(Key.Number3))
			{
				Shader.blurShaderCompiled.Enable();
			}
			else
			{
				Shader.hudShaderCompiled.Enable();
			}
			

			GL.BindTexture(TextureTarget.Texture2D, ColorTexture);

			GL.Begin(PrimitiveType.Quads);
			GL.Color4(Color4.White);
			GL.TexCoord2(0, 1); GL.Vertex2(0, 0);
			GL.TexCoord2(0, 0); GL.Vertex2(0, Height);
			GL.TexCoord2(1, 0); GL.Vertex2(Width, Height);
			GL.TexCoord2(1, 1); GL.Vertex2(Width, 0);
			GL.End();

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