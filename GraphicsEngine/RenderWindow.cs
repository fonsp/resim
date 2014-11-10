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
		public uint fboHandle, colorTexture, depthTexture, depthRenderbuffer;
		public float focalDistance = 0.3f;

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
				Debug.WriteLine("WARNING: OpenGL could not be initialized: " + exception.Message + " @ " + exception.Source);
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
				Debug.WriteLine("WARNING: default textures could not be initialized: " + exception.Message + " @ " + exception.Source);
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
				Debug.WriteLine("WARNING: lighting could not be initialized: " + exception.Message + " @ " + exception.Source);
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
				Debug.WriteLine("WARNING: default shader could not be initialized: " + exception.Message + " @ " + exception.Source);
				Exit();
			}

			#endregion
			#region FBO init
			Debug.WriteLine("Initializing FBO..");

			//Debug.WriteLine("FBO size: {" + Width + ", " + Height + "}");
			try
			{

				// Create Color Texture
				GL.ActiveTexture(TextureUnit.Texture0);
				GL.GenTextures(1, out colorTexture);
				GL.BindTexture(TextureTarget.Texture2D, colorTexture);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Clamp);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Clamp);
				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, Width, Height, 0, PixelFormat.Rgba,
					PixelType.UnsignedByte, IntPtr.Zero);
				GL.BindTexture(TextureTarget.Texture2D, 0);

				// Create depth Texture
				GL.ActiveTexture(TextureUnit.Texture1);
				GL.GenTextures(1, out depthTexture);
				GL.BindTexture(TextureTarget.Texture2D, depthTexture);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Nearest);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Clamp);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Clamp);
				GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareMode, (int) TextureCompareMode.None);
				GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32, Width, Height, 0,
					PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
				GL.BindTexture(TextureTarget.Texture2D, 0);

				GL.ActiveTexture(TextureUnit.Texture0);

				//TODO: test for GL Error here (might be unsupported format)

				// Create an FBO and attach the textures
				GL.Ext.GenFramebuffers(1, out fboHandle);
				GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, fboHandle);
				GL.ActiveTexture(TextureUnit.Texture0);
				GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext,
					TextureTarget.Texture2D, colorTexture, 0);
				GL.ActiveTexture(TextureUnit.Texture1);
				GL.Ext.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachment,
					TextureTarget.Texture2D, depthTexture, 0);
				GL.ActiveTexture(TextureUnit.Texture0);

				if (!CheckFboStatus())
				{
					Debug.WriteLine("WARNING: FBO initialization failure");
					Exit();
				}
			}
			catch (Exception exception)
			{
				Debug.WriteLine("WARNING: FBO could not be initialized: " + exception.Message + " @ " + exception.Source);
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
				Debug.WriteLine("WARNING: custom resources could not be loaded: " + exception.Message + " @ " + exception.Source);
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
				Debug.WriteLine("WARNING: Failed to unload textures: " + exception.Message + " @ " + exception.Source);
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

			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, colorTexture);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, Width, Height, 0, PixelFormat.Rgba,
				PixelType.UnsignedByte, IntPtr.Zero);
			GL.BindTexture(TextureTarget.Texture2D, 0);

			GL.ActiveTexture(TextureUnit.Texture1);
			GL.BindTexture(TextureTarget.Texture2D, depthTexture);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.DepthComponent32, Width, Height, 0,
				PixelFormat.DepthComponent, PixelType.Float, IntPtr.Zero);
			GL.BindTexture(TextureTarget.Texture2D, 0);

			GL.ActiveTexture(TextureUnit.Texture0);

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
			if(InputManager.IsKeyDown(Key.Escape) && escapeOnEscape/* && InputManager.IsKeyDown(Key.Pause)*/)
			{
				Exit();
			}
#if DEBUG
			if(InputManager.IsKeyDown(Key.Pause) && InputManager.IsKeyUp(Key.Escape))
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
			smoothedVelocity += velocityDelta - Vector3.Divide(velocityDelta, (float)Math.Pow(smoothFactor, renderTime)); //TODO: time dilation

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

			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadIdentity();

			GL.Ext.BindRenderbuffer(RenderbufferTarget.RenderbufferExt, depthRenderbuffer);
			GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, fboHandle);
			GL.DrawBuffer((DrawBufferMode)FramebufferAttachment.ColorAttachment0Ext);
			GL.PushAttrib(AttribMask.ViewportBit);
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
			GL.PopAttrib();
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

			GL.ActiveTexture(TextureUnit.Texture1);
			GL.BindTexture(TextureTarget.Texture2D, depthTexture);
			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, colorTexture);

			if(InputManager.IsKeyToggled(Key.Number3))
			{
				Shader.blurShaderCompiled.Enable();
				Shader.blurShaderCompiled.SetUniform("tex", 0);
				Shader.blurShaderCompiled.SetUniform("depthTex", 1);
				Shader.blurShaderCompiled.SetUniform("focalDist", focalDistance);
			}
			else
			{
				Shader.hudShaderCompiled.Enable();
				Shader.hudShaderCompiled.SetUniform("tex", 0);
			}

			GL.Begin(PrimitiveType.Quads);
			GL.Color4(Color4.White);
			GL.TexCoord2(0, 1); GL.Vertex2(0, 0);
			GL.TexCoord2(0, 0); GL.Vertex2(0, Height);
			GL.TexCoord2(1, 0); GL.Vertex2(Width, Height);
			GL.TexCoord2(1, 1); GL.Vertex2(Width, 0);
			GL.End();

			GL.ActiveTexture(TextureUnit.Texture0);

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

		private bool CheckFboStatus()
		{
			switch(GL.Ext.CheckFramebufferStatus(FramebufferTarget.FramebufferExt))
			{
				case FramebufferErrorCode.FramebufferCompleteExt:
					{
						Debug.WriteLine("FBO: The framebuffer is complete and valid for rendering.");
						return true;
					}
				case FramebufferErrorCode.FramebufferIncompleteAttachmentExt:
					{
						Debug.WriteLine("ERROR: failed to create FBO: One or more attachment points are not framebuffer attachment complete. This could mean there’s no texture attached or the format isn’t renderable. For color textures this means the base format must be RGB or RGBA and for depth textures it must be a DEPTH_COMPONENT format. Other causes of this error are that the width or height is zero or the z-offset is out of range in case of render to volume.");
						break;
					}
				case FramebufferErrorCode.FramebufferIncompleteMissingAttachmentExt:
					{
						Debug.WriteLine("ERROR: failed to create FBO: There are no attachments.");
						break;
					}
				/* case  FramebufferErrorCode.GL_FRAMEBUFFER_INCOMPLETE_DUPLICATE_ATTACHMENT_EXT: 
					 {
						 Debug.WriteLine("ERROR: failed to create FBO: An object has been attached to more than one attachment point.");
						 break;
					 }*/
				case FramebufferErrorCode.FramebufferIncompleteDimensionsExt:
					{
						Debug.WriteLine("ERROR: failed to create FBO: Attachments are of different size. All attachments must have the same width and height.");
						break;
					}
				case FramebufferErrorCode.FramebufferIncompleteFormatsExt:
					{
						Debug.WriteLine("ERROR: failed to create FBO: The color attachments have different format. All color attachments must have the same format.");
						break;
					}
				case FramebufferErrorCode.FramebufferIncompleteDrawBufferExt:
					{
						Debug.WriteLine("ERROR: failed to create FBO: An attachment point referenced by GL.DrawBuffers() doesn’t have an attachment.");
						break;
					}
				case FramebufferErrorCode.FramebufferIncompleteReadBufferExt:
					{
						Debug.WriteLine("ERROR: failed to create FBO: The attachment point referenced by GL.ReadBuffers() doesn’t have an attachment.");
						break;
					}
				case FramebufferErrorCode.FramebufferUnsupportedExt:
					{
						Debug.WriteLine("ERROR: failed to create FBO: This particular FBO configuration is not supported by the implementation.");
						break;
					}
				default:
					{
						Debug.WriteLine("ERROR: failed to create FBO: Status unknown. (yes, this is really bad.)");
						break;
					}
			}
			return false;
		}
	}
}