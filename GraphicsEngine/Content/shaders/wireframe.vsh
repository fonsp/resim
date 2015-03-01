#version 120

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
    gl_Position = gl_ProjectionMatrix * (crot * (gl_ModelViewMatrix * v));
	gl_FrontColor = gl_Color;
	gl_TexCoord[0] = gl_MultiTexCoord0;
}