#version 120
#extension GL_EXT_gpu_shader4 : enable
uniform float worldTime;
uniform int effects;
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
		if((effects / 1) % 2 == 1)
		{
			float oldlength = length(v.xyz);
			v.xyz = v.xyz + vdir * b * length(v.xyz);
			v.xyz = v.xyz * (oldlength / length(v.xyz));
		}
	}
	v = crot * v;
    gl_Position = gl_ProjectionMatrix * v;
	gl_TexCoord[0] = gl_MultiTexCoord0;
	gl_FrontColor = vec4(gl_Color.xyz, 1.0 / (gl_Position.w / 2000.0 + 1.0));
}