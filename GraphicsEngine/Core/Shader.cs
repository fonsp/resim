using System;
using System.Collections.Generic;
using System.Diagnostics;
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

		public int vertexShaderObject
		{
			get { return vso; }
		}

		public int fragmentShaderObject
		{
			get { return fso; }
		}

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

		#region Default shaders

		public static Shader diffuseShader
		{
			get
			{
				return new Shader
				{
					vertexShader = @"
#version 120
uniform float time;
varying float intensity;

void main()
{
	intensity = (dot(gl_LightSource[0].position.xyz, gl_Normal)+1.0)/2.0;
	
	/*vec4 v =  gl_ModelViewMatrix * gl_Vertex;
	v.z = sin(0.02 * v.x + time) * 20.0 + v.z;
    gl_Position = gl_ProjectionMatrix * v;*/
	gl_Position = ftransform();
	
	gl_FrontColor = vec4(gl_Color.xyz, 1.0 / (gl_Position.w / 2000.0 + 1.0));
	gl_TexCoord[0] = gl_MultiTexCoord0;
}",
					fragmentShader = @"
#version 120
uniform float time;
uniform sampler2D tex;
varying float intensity;

void main()
{
	float a = sqrt(intensity);
	gl_FragColor = vec4(vec3(gl_Color.w), 1.0) * (texture2D(tex, gl_TexCoord[0].xy) * (vec4(a, a, a, 1.0) * gl_LightSource[0].diffuse + vec4(1.0-a, 1.0-a, 1.0-a, 1.0) * gl_LightSource[0].ambient));
}"
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
					vertexShader = @"
#version 120
uniform float time;

void main()
{
    gl_Position = ftransform();
	gl_TexCoord[0] = gl_MultiTexCoord0;
	gl_FrontColor = gl_Color;
}",
					fragmentShader = @"
#version 120
uniform float time;
uniform sampler2D tex;

void main()
{
	gl_FragColor = texture2D(tex, gl_TexCoord[0].xy) * gl_Color;
}"
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

		public static Shader depthShader
		{
			get
			{
				return new Shader
				{
					vertexShader = @"
#version 120
uniform float time;

void main()
{
    gl_Position = ftransform();
	gl_TexCoord[0] = gl_MultiTexCoord0;
	gl_FrontColor = vec4(gl_Color.xyz, 1.0 / (gl_Position.w / 2000.0 + 1.0));
}",
					fragmentShader = @"
#version 120
uniform float time;
uniform sampler2D tex;

void main()
{
	gl_FragColor = texture2D(tex, gl_TexCoord[0].xy) * vec4(gl_Color.xyz, 1.0) * vec4(vec3(gl_Color.w), 1.0);
}"
				};
			}
		}

		private static Shader depthShaderCompiledi;
		public static Shader depthShaderCompiled
		{
			get
			{
				if(depthShaderCompiledi == null)
				{
					depthShaderCompiledi = depthShader;
				}
				if(depthShaderCompiledi.Compiled == false)
				{
					depthShaderCompiledi.GenerateShaders();
				}
				return depthShaderCompiledi;
			}
		}

		public static Shader wireframeShader
		{
			get
			{
				return new Shader
				{
					vertexShader = @"
#version 120
uniform float time;

void main()
{
	gl_FrontColor = gl_Color;
    gl_Position = ftransform();
	gl_TexCoord[0] = gl_MultiTexCoord0;
}",
					fragmentShader = @"
#version 120
uniform float time;
uniform sampler2D tex;

void main()
{
	gl_FragColor = texture2D(tex, gl_TexCoord[0].xy) * gl_Color * 0.5 + vec4(0.5, 0.0, 0.0, 0.5);
}"
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
					vertexShader = @"
#version 120
uniform float time;

void main()
{
    gl_Position = ftransform();
}",
					fragmentShader = @"
#version 120
uniform float time;
uniform sampler2D tex;

void main()
{
	gl_FragColor = vec4(0.5, 0.0, 0.0, 0.05);
}"
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
					vertexShader = @"
#version 120
uniform float time;

void main()
{
	gl_FrontColor = gl_Color;
    gl_Position = ftransform();
	gl_TexCoord[0] = gl_MultiTexCoord0;
}",
					fragmentShader = @"
#version 120
uniform float time;
uniform sampler2D tex;

void main()
{
	gl_FragColor = texture2D(tex, gl_TexCoord[0].xy) * gl_Color;
}"
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

		#endregion
		#region Uniform management
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
		#endregion
	}
}