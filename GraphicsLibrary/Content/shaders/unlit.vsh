#version 120
#extension GL_EXT_gpu_shader4 : enable

uniform float worldTime;
uniform int effects;
uniform float bL;
uniform float bW;
uniform vec3 vdirL;
uniform vec3 vdirW;
uniform vec3 cpos;
uniform mat4 crot;
varying float dopp;
uniform sampler2D tex;

void main()
{
	vec4 v = gl_Vertex;
	v.xyz = v.xyz - cpos;
	v = gl_ModelViewMatrix * v;
	if(bL > 0.001)
	{
		dopp = (dot(v.xyz, vdirL) / length(v.xyz)) * (bL / sqrt(1.0 - bL * bL));
	} else {
		dopp = 0.0;
	}
	if(bW > 0.001)
	{
		if((effects / 1) % 2 == 1)
		{
			float oldlength = length(v.xyz);
			v.xyz = v.xyz + vdirW * bW * length(v.xyz);
			v.xyz = v.xyz * (oldlength / length(v.xyz));
		}
	}
	v = crot * v;
    gl_Position = gl_ProjectionMatrix * v;
	
	gl_FrontColor = gl_Color;
	gl_TexCoord[0] = gl_MultiTexCoord0;
}