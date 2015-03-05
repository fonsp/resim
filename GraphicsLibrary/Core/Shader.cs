using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GraphicsLibrary.Core
{
	public class Shader
	{
		public string vertexShader, fragmentShader;
		public Dictionary<string, int> uniforms = new Dictionary<string, int>();
		private int vso, fso, sp;
		private bool compiled;
		public bool Compiled { get { return compiled; } }

		/// <summary>
		/// Pointer to the vertex shader object in GPU memory
		/// </summary>
		public int vertexShaderObject
		{
			get { return vso; }
		}

		/// <summary>
		/// Pointer to the fragment shader object in GPU memory
		/// </summary>
		public int fragmentShaderObject
		{
			get { return fso; }
		}

		/// <summary>
		/// Pointer to the shader program in GPU memory
		/// </summary>
		public int shaderProgram
		{
			get { return sp; }
		}

		public Shader(string vertexShader, string fragmentShader)
		{
			this.vertexShader = vertexShader;
			this.fragmentShader = fragmentShader;
		}

		public Shader()
		{
			vertexShader = "";
			fragmentShader = "";
		}

		/// <summary>
		/// Tranfer shader code to GPU and compile.
		/// </summary>
		public void GenerateShaders()
		{
			int statusCode;
			string info;

			//Create
			vso = GL.CreateShader(ShaderType.VertexShader);
			fso = GL.CreateShader(ShaderType.FragmentShader);

			//Load & compile vertex shader
			GL.ShaderSource(vso, vertexShader);
			GL.CompileShader(vso);
			GL.GetShader(vso, ShaderParameter.CompileStatus, out statusCode);

			if(statusCode != 1)
			{
				GL.GetShaderInfoLog(vso, out info);
				Debug.WriteLine("VSO failed to compile");
				throw new ApplicationException(info);
			}

			//Load & compile fragment shader
			GL.ShaderSource(fso, fragmentShader);
			GL.CompileShader(fso);
			GL.GetShader(fso, ShaderParameter.CompileStatus, out statusCode);

			if(statusCode != 1)
			{
				GL.GetShaderInfoLog(fso, out info);
				Debug.WriteLine("FSO failed to compile");
				throw new ApplicationException(info);
			}

			sp = GL.CreateProgram();
			GL.AttachShader(sp, fso);
			GL.AttachShader(sp, vso);
			GL.LinkProgram(sp);

			compiled = true;

			Enable();
		}

		/// <summary>
		/// Use this shader for rendering OpenGL geometry.
		/// </summary>
		public void Enable()
		{
			if(compiled)
			{
				GL.UseProgram(sp);
			}
			else
			{
				Debug.WriteLine("WARNING: failed to enable shader: shader does not exist");
			}
		}

		/// <summary>
		/// Remove this shader from GPU memory.
		/// </summary>
		public void Remove()
		{
			if(compiled)
			{
				GL.DetachShader(sp, vso);
				GL.DetachShader(sp, fso);
				GL.DeleteShader(vso);
				GL.DeleteShader(fso);
				compiled = false;
			}
			else
			{
				Debug.WriteLine("WARNING: failed to remove shader: shader does not exist");
			}
		}

		#region Default geometry shaders

		public static Shader diffuseShader
		{
			get
			{
				return new Shader
				{
					vertexShader = File.ReadAllText(@"Content/shaders/diffuse.vsh"),
					fragmentShader = File.ReadAllText(@"Content/shaders/diffuse.fsh")
				};
			}
		}

		private static Shader diffuseShaderCompiledi;
		public static Shader diffuseShaderCompiled
		{
			get
			{
				if(diffuseShaderCompiledi == null)
				{
					diffuseShaderCompiledi = diffuseShader;
				}
				if(diffuseShaderCompiledi.Compiled == false)
				{
					diffuseShaderCompiledi.GenerateShaders();
				}
				return diffuseShaderCompiledi;
			}
		}

		public static Shader unlitShader
		{
			get
			{
				return new Shader
				{
					vertexShader = File.ReadAllText(@"Content/shaders/unlit.vsh"),
					fragmentShader = File.ReadAllText(@"Content/shaders/unlit.fsh")
				};
			}
		}

		private static Shader unlitShaderCompiledi;
		public static Shader unlitShaderCompiled
		{
			get
			{
				if(unlitShaderCompiledi == null)
				{
					unlitShaderCompiledi = unlitShader;
				}
				if(unlitShaderCompiledi.Compiled == false)
				{
					unlitShaderCompiledi.GenerateShaders();
				}
				return unlitShaderCompiledi;
			}
		}

		public static Shader wireframeShader
		{
			get
			{
				return new Shader
				{
					vertexShader = File.ReadAllText(@"Content/shaders/wireframe.vsh"),
					fragmentShader = File.ReadAllText(@"Content/shaders/wireframe.fsh")
				};
			}
		}

		private static Shader wireframeShaderCompiledi;
		public static Shader wireframeShaderCompiled
		{
			get
			{
				if(wireframeShaderCompiledi == null)
				{
					wireframeShaderCompiledi = wireframeShader;
				}
				if(wireframeShaderCompiledi.Compiled == false)
				{
					wireframeShaderCompiledi.GenerateShaders();
				}
				return wireframeShaderCompiledi;
			}
		}

		public static Shader collisionShader
		{
			get
			{
				return new Shader
				{
					vertexShader = File.ReadAllText(@"Content/shaders/collision.vsh"),
					fragmentShader = File.ReadAllText(@"Content/shaders/collision.fsh")
				};
			}
		}

		private static Shader collisionShaderCompiledi;
		public static Shader collisionShaderCompiled
		{
			get
			{
				if(collisionShaderCompiledi == null)
				{
					collisionShaderCompiledi = collisionShader;
				}
				if(collisionShaderCompiledi.Compiled == false)
				{
					collisionShaderCompiledi.GenerateShaders();
				}
				return collisionShaderCompiledi;
			}
		}

		public static Shader hudShader
		{
			get
			{
				return new Shader
				{
					vertexShader = File.ReadAllText(@"Content/shaders/hud.vsh"),
					fragmentShader = File.ReadAllText(@"Content/shaders/hud.fsh")
				};
			}
		}

		private static Shader hudShaderCompiledi;
		public static Shader hudShaderCompiled
		{
			get
			{
				if(hudShaderCompiledi == null)
				{
					hudShaderCompiledi = hudShader;
				}
				if(hudShaderCompiledi.Compiled == false)
				{
					hudShaderCompiledi.GenerateShaders();
				}
				return hudShaderCompiledi;
			}
		}

		public static Shader fboShader
		{
			get
			{
				return new Shader
				{
					vertexShader = File.ReadAllText(@"Content/shaders/fbo.vsh"),
					fragmentShader = File.ReadAllText(@"Content/shaders/fbo.fsh")
				};
			}
		}

		private static Shader fboShaderCompiledi;
		public static Shader fboShaderCompiled
		{
			get
			{
				if(fboShaderCompiledi == null)
				{
					fboShaderCompiledi = fboShader;
				}
				if(fboShaderCompiledi.Compiled == false)
				{
					fboShaderCompiledi.GenerateShaders();
				}
				return fboShaderCompiledi;
			}
		}
		#endregion
		#region Default gfx shaders

		public static Shader blurShader
		{
			get
			{
				return new Shader
				{
					vertexShader = File.ReadAllText(@"Content/shaders/blur.vsh"),
					fragmentShader = File.ReadAllText(@"Content/shaders/blur.fsh")
				};
			}
		}

		private static Shader blurShaderCompiledi;
		public static Shader blurShaderCompiled
		{
			get
			{
				if(blurShaderCompiledi == null)
				{
					blurShaderCompiledi = blurShader;
				}
				if(blurShaderCompiledi.Compiled == false)
				{
					blurShaderCompiledi.GenerateShaders();
				}
				return blurShaderCompiledi;
			}
		}

		public static Shader ssaoShader
		{
			get
			{
				return new Shader
				{
					vertexShader = File.ReadAllText(@"Content/shaders/ssao.vsh"),
					fragmentShader = File.ReadAllText(@"Content/shaders/ssao.fsh")
				};
			}
		}

		private static Shader ssaoShaderCompiledi;
		public static Shader ssaoShaderCompiled
		{
			get
			{
				if(ssaoShaderCompiledi == null)
				{
					ssaoShaderCompiledi = ssaoShader;
				}
				if(ssaoShaderCompiledi.Compiled == false)
				{
					ssaoShaderCompiledi.GenerateShaders();
				}
				return ssaoShaderCompiledi;
			}
		}

		public static Shader crtShader
		{
			get
			{
				return new Shader
				{
					vertexShader = File.ReadAllText(@"Content/shaders/crt.vsh"),
					fragmentShader = File.ReadAllText(@"Content/shaders/crt.fsh")
				};
			}
		}

		private static Shader crtShaderCompiledi;
		public static Shader crtShaderCompiled
		{
			get
			{
				if(crtShaderCompiledi == null)
				{
					crtShaderCompiledi = crtShader;
				}
				if(crtShaderCompiledi.Compiled == false)
				{
					crtShaderCompiledi.GenerateShaders();
				}
				return crtShaderCompiledi;
			}
		}

		public static Shader ditherShader
		{
			get
			{
				return new Shader
				{
					vertexShader = File.ReadAllText(@"Content/shaders/dither.vsh"),
					fragmentShader = File.ReadAllText(@"Content/shaders/dither.fsh")
				};
			}
		}

		private static Shader ditherShaderCompiledi;
		public static Shader ditherShaderCompiled
		{
			get
			{
				if(ditherShaderCompiledi == null)
				{
					ditherShaderCompiledi = ditherShader;
				}
				if(ditherShaderCompiledi.Compiled == false)
				{
					ditherShaderCompiledi.GenerateShaders();
				}
				return ditherShaderCompiledi;
			}
		}

		#endregion
		#region Uniform management

		/// <summary>
		/// Sets a uniform in the specified shader.
		/// </summary>
		/// <param name="name">Uniform name</param>
		/// <param name="value">New value</param>
		public void SetUniform(string name, bool value)
		{
			if(!compiled)
			{
				return;
			}
			GL.UseProgram(sp);
			if(!uniforms.ContainsKey(name))
			{
				uniforms.Add(name, GL.GetUniformLocation(sp, name));
			}
			int intValue = value ? 1 : 0;
			GL.Uniform1(uniforms[name], intValue);
		}

		/// <summary>
		/// Sets the effects uniform.
		/// </summary>
		/// <param name="name">Uniform name</param>
		/// <param name="doppler">Enable Doppler effect</param>
		/// <param name="relBrightness">Enable relativistic brightness</param>
		/// <param name="relAberration">Enable relativistic aberration</param>
		public void SetUniform(string name, bool doppler, bool relBrightness, bool relAberration)
		{
			if(!compiled)
			{
				return;
			}
			GL.UseProgram(sp);
			if(!uniforms.ContainsKey(name))
			{
				uniforms.Add(name, GL.GetUniformLocation(sp, name));
			}
			int intValue = 0;
			if(relAberration)
			{
				intValue += 1;
			}
			if(relBrightness)
			{
				intValue += 2;
			}
			if(doppler)
			{
				intValue += 4;
			}
			GL.Uniform1(uniforms[name], intValue);
		}

		/// <summary>
		/// Sets a uniform in the specified shader.
		/// </summary>
		/// <param name="name">Uniform name</param>
		/// <param name="value">New value</param>
		public void SetUniform(string name, int value)
		{
			if(!compiled)
			{
				return;
			}
			GL.UseProgram(sp);
			if(!uniforms.ContainsKey(name))
			{
				uniforms.Add(name, GL.GetUniformLocation(sp, name));
			}
			GL.Uniform1(uniforms[name], value);
		}

		/// <summary>
		/// Sets a uniform in the specified shader.
		/// </summary>
		/// <param name="name">Uniform name</param>
		/// <param name="value">New value</param>
		public void SetUniform(string name, uint value)
		{
			if(!compiled)
			{
				return;
			}
			GL.UseProgram(sp);
			if(!uniforms.ContainsKey(name))
			{
				uniforms.Add(name, GL.GetUniformLocation(sp, name));
			}
			GL.Uniform1(uniforms[name], value);
		}

		/// <summary>
		/// Sets a uniform in the specified shader.
		/// </summary>
		/// <param name="name">Uniform name</param>
		/// <param name="value">New value</param>
		public void SetUniform(string name, float value)
		{
			if(!compiled)
			{
				return;
			}
			GL.UseProgram(sp);
			if(!uniforms.ContainsKey(name))
			{
				uniforms.Add(name, GL.GetUniformLocation(sp, name));
			}
			GL.Uniform1(uniforms[name], value);
		}

		/// <summary>
		/// Sets a uniform in the specified shader.
		/// </summary>
		/// <param name="name">Uniform name</param>
		/// <param name="value">New value</param>
		public void SetUniform(string name, double value)
		{
			if(!compiled)
			{
				return;
			}
			GL.UseProgram(sp);
			if(!uniforms.ContainsKey(name))
			{
				uniforms.Add(name, GL.GetUniformLocation(sp, name));
			}
			GL.Uniform1(uniforms[name], value);
		}

		/// <summary>
		/// Sets a uniform in the specified shader.
		/// </summary>
		/// <param name="name">Uniform name</param>
		/// <param name="value">New value</param>
		public void SetUniform(string name, Vector2 value)
		{
			if(!compiled)
			{
				return;
			}
			GL.UseProgram(sp);
			if(!uniforms.ContainsKey(name))
			{
				uniforms.Add(name, GL.GetUniformLocation(sp, name));
			}
			GL.Uniform2(uniforms[name], value);
		}

		/// <summary>
		/// Sets a uniform in the specified shader.
		/// </summary>
		/// <param name="name">Uniform name</param>
		/// <param name="value">New value</param>
		public void SetUniform(string name, Vector3 value)
		{
			if(!compiled)
			{
				return;
			}
			GL.UseProgram(sp);
			if(!uniforms.ContainsKey(name))
			{
				uniforms.Add(name, GL.GetUniformLocation(sp, name));
			}
			GL.Uniform3(uniforms[name], value);
		}

		/// <summary>
		/// Sets a uniform in the specified shader.
		/// </summary>
		/// <param name="name">Uniform name</param>
		/// <param name="value">New value</param>
		public void SetUniform(string name, Vector4 value)
		{
			if(!compiled)
			{
				return;
			}
			GL.UseProgram(sp);
			if(!uniforms.ContainsKey(name))
			{
				uniforms.Add(name, GL.GetUniformLocation(sp, name));
			}
			GL.Uniform4(uniforms[name], value);
		}

		/// <summary>
		/// Sets a uniform in the specified shader.
		/// </summary>
		/// <param name="name">Uniform name</param>
		/// <param name="value">New value</param>
		public void SetUniform(string name, Matrix4 value)
		{
			if(!compiled)
			{
				return;
			}
			GL.UseProgram(sp);
			if(!uniforms.ContainsKey(name))
			{
				uniforms.Add(name, GL.GetUniformLocation(sp, name));
			}
			GL.UniformMatrix4(uniforms[name], false, ref value);
		}
		#endregion
	}
}