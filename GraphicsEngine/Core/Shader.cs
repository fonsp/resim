﻿using System;
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

		#region Default geometry shaders

		public static Shader diffuseShader
		{
			get
			{
				return new Shader
				{
					vertexShader = @"
#version 120
uniform float time;
uniform float b;
uniform vec3 vdir;
uniform vec3 cpos;
uniform mat4 crot;

varying float intensity;

void main()
{
	intensity = (dot(gl_LightSource[0].position.xyz, gl_Normal)+1.0)/2.0;
	
	vec4 v = gl_Vertex;
	v.xyz = v.xyz - cpos;
	v = gl_ModelViewMatrix * v;
	if(b > 0)
	{
		float oldlength = length(v.xyz);
		v.xyz = v.xyz + vdir * b * length(v.xyz);
		v.xyz = v.xyz * (oldlength / length(v.xyz));
	}
	v = crot * v;
    gl_Position = gl_ProjectionMatrix * v;
	
	gl_FrontColor = vec4(gl_Color.xyz, 1.0 / (gl_Position.w / 2000.0 + 1.0));
	gl_TexCoord[0] = gl_MultiTexCoord0;
}",
					fragmentShader = @"
#version 120
uniform float time;
uniform float b;
uniform vec3 vdir;
uniform vec3 cpos;
uniform mat4 crot;
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
uniform float b;
uniform vec3 vdir;
uniform vec3 cpos;
uniform mat4 crot;

void main()
{
    gl_Position = ftransform();
	gl_TexCoord[0] = gl_MultiTexCoord0;
	gl_FrontColor = gl_Color;
}",
					fragmentShader = @"
#version 120
uniform float time;
uniform float b;
uniform vec3 vdir;
uniform vec3 cpos;
uniform mat4 crot;
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
uniform float b;
uniform vec3 vdir;
uniform vec3 cpos;
uniform mat4 crot;
varying float dopp;

void main()
{
	vec4 v = gl_Vertex;
	v.xyz = v.xyz - cpos;
	v = gl_ModelViewMatrix * v;
	dopp = 0.0;
	if(b > 0)
	{
		dopp = dot(v.xyz, vdir) * b / length(v.xyz);
		float oldlength = length(v.xyz);
		v.xyz = v.xyz + vdir * b * length(v.xyz);
		v.xyz = v.xyz * (oldlength / length(v.xyz));
	}
	v = crot * v;
    gl_Position = gl_ProjectionMatrix * v;
	gl_TexCoord[0] = gl_MultiTexCoord0;
	gl_FrontColor = vec4(gl_Color.xyz, 1.0 / (gl_Position.w / 2000.0 + 1.0));
}",
					fragmentShader = @"
#version 120
uniform float time;
uniform float b;
uniform vec3 vdir;
uniform vec3 cpos;
uniform mat4 crot;
uniform sampler2D tex;
varying float dopp;

void main()
{
	
	vec4 shift = vec4(1.0);
	shift.r = 2 * max(0, 0.5 - abs(dopp + 0.0)) + 2 * max(0, 0.5 - abs(dopp + 0.5)) + 2 * max(0, 0.5 - abs(dopp + 1.0));
	shift.g = 2 * max(0, 0.5 - abs(dopp - 0.5)) + 2 * max(0, 0.5 - abs(dopp + 0.0)) + 2 * max(0, 0.5 - abs(dopp + 0.5));
	shift.b = 2 * max(0, 0.5 - abs(dopp - 1.0)) + 2 * max(0, 0.5 - abs(dopp - 0.5)) + 2 * max(0, 0.5 - abs(dopp + 0.0));
	gl_FragColor = texture2D(tex, gl_TexCoord[0].xy) * vec4(vec3(0.5 + dopp / 2.0), 1.0) * vec4(vec3(gl_Color.w), 1.0) * shift;
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
uniform float b;
uniform vec3 vdir;
uniform vec3 cpos;
uniform mat4 crot;

void main()
{
    vec4 v = gl_Vertex;
	v.xyz = v.xyz - cpos;
    gl_Position = gl_ProjectionMatrix * (crot * (gl_ModelViewMatrix * v));
	gl_FrontColor = gl_Color;
	gl_TexCoord[0] = gl_MultiTexCoord0;
}",
					fragmentShader = @"
#version 120
uniform float time;
uniform float b;
uniform vec3 vdir;
uniform vec3 cpos;
uniform mat4 crot;
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
uniform float b;
uniform vec3 vdir;
uniform vec3 cpos;
uniform mat4 crot;

void main()
{
	vec4 v = gl_Vertex;
	v.xyz = v.xyz - cpos;
	gl_Position = gl_ProjectionMatrix * (crot * (gl_ModelViewMatrix * v));
}",
					fragmentShader = @"
#version 120
uniform float time;
uniform float b;
uniform vec3 vdir;
uniform vec3 cpos;
uniform mat4 crot;
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
		#region Default gfx shaders

		public static Shader blurShader
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
uniform sampler2D depthTex;
uniform float focalDist;

float asdf = 0.0;

void main()
{
	float dx = 2.0 / 1280.0;
	float dy = 2.0 / 0720.0;
	
	/*gl_FragColor = (texture2D(tex, gl_TexCoord[0].xy + vec2(-2.0 * dx, 0)) * 1.0 + 
					texture2D(tex, gl_TexCoord[0].xy + vec2(-1.0 * dx, 0)) * 4.0 + 
					texture2D(tex, gl_TexCoord[0].xy + vec2(00.0 * dx, 0)) * 6.0 + 
					texture2D(tex, gl_TexCoord[0].xy + vec2(01.0 * dx, 0)) * 4.0 + 
					texture2D(tex, gl_TexCoord[0].xy + vec2(02.0 * dx, 0)) * 1.0
					) / 16.0;*/
	float[] pascal = float[7](1.0, 6.0, 15.0, 20.0, 15.0, 6.0, 1.0);
	vec4 sum = vec4(0.0);
	for(int x = 0; x < 7; x++)
	{
		for(int y = 0; y < 7; y++)
		{
			sum = sum + texture2D(tex, gl_TexCoord[0].xy + vec2((x - 3) * dx, (y - 3) * dy)) * pascal[x] * pascal[y];
		}
	}
	sum = sum / 4096.0;
	gl_FragColor = sum;
	float depth = texture2D(depthTex, gl_TexCoord[0].xy).x;
	float n = 1.0; // camera z near
	float f = 100000.0; // camera z far
	depth = 60.0 * (2.0 * n) / (f + n - depth * (f - n));

	float focalDist2 = texture2D(depthTex, vec2(0.5, 0.5)).x;
	focalDist2 = 60.0 * (2.0 * n) / (f + n - focalDist2 * (f - n));

	depth = abs(1.0 - depth / focalDist2);
	depth = clamp(depth, 0.0, 1.0);
	gl_FragColor = sum * depth + texture2D(tex, gl_TexCoord[0].xy) * (1.0 - depth);
	//gl_FragColor = vec4(vec3(depth), 1.0);
}"
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

		public static Shader crtShader
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
	/*vec2 rCoord = (gl_TexCoord[0].xy - vec2(0.5, 0.5)) * 0.995 + vec2(0.5, 0.5);

	float r = texture2D(tex, rCoord).r;
	float g = texture2D(tex, gl_TexCoord[0].xy).g;
	float b = texture2D(tex, gl_TexCoord[0].xy).b;
	gl_FragColor = vec4(r, g, b, 1.0); chromatic aberration*/
	/*gl_FragColor = vec4(texture2D(tex, gl_TexCoord[0].xy).xyz - texture2D(tex, gl_TexCoord[0].xy + vec2(1.0/1280.0, 1.0/720.0)).xyz, 1.0); diff*/
	float r = texture2D(tex, (gl_TexCoord[0].xy - vec2(0.5, 0.5)) * 0.992 + vec2(0.5, 0.5)).r;
	float g = texture2D(tex, gl_TexCoord[0].xy).g;
	float b = texture2D(tex, (gl_TexCoord[0].xy - vec2(0.5, 0.5)) * 0.998 + vec2(0.5, 0.5)).b;
	gl_FragColor = vec4(vec3(r, g, b) * (mod(int(gl_FragCoord.y), 2) + 1.0) * 0.5, 1.0);
}"
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