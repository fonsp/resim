using System;
using System.Diagnostics;
using OpenTK.Graphics.OpenGL;

namespace GraphicsLibrary.Core
{
	public class Shader
	{
		public string vertexShader, fragmentShader;
		private int vso, fso, sp;
		private bool created;

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
			vertexShader = @"
#version 120

void main()
{
    gl_Position = ftransform();
	gl_TexCoord[0] = gl_MultiTexCoord0;
}";
			fragmentShader = @"
#version 120
uniform sampler2D tex;

void main()
{
	gl_FragColor = texture2D(tex, gl_TexCoord[0].xy);
}";
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
				throw new ApplicationException(info);
			}

			//Load & compile fragment shader
			GL.ShaderSource(fso, fragmentShader);
			GL.CompileShader(fso);
			GL.GetShader(fso, ShaderParameter.CompileStatus, out statusCode);

			if(statusCode != 1)
			{
				GL.GetShaderInfoLog(fso, out info);
				throw new ApplicationException(info);
			}

			sp = GL.CreateProgram();
			GL.AttachShader(sp, fso);
			GL.AttachShader(sp, vso);

			created = true;
		}

		public void Enable()
		{
			if(created)
			{
				GL.LinkProgram(sp);
				GL.UseProgram(sp);
			}
			else
			{
				Debug.WriteLine("WARNING: failed to enable shader: shader does not exist");
			}
		}

		public void Remove()
		{
			if(created)
			{
				GL.DetachShader(sp, vso);
				GL.DetachShader(sp, fso);
				GL.DeleteShader(vso);
				GL.DeleteShader(fso);
				created = false;
			}
			else
			{
				Debug.WriteLine("WARNING: failed to remove shader: shader does not exist");
			}
		}
	}
}