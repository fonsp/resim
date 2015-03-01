#version 120
uniform float worldTime;
uniform int effects;
uniform float b;
uniform vec3 vdir;
uniform vec3 cpos;
uniform mat4 crot;

void main()
{
	vec4 v = gl_Vertex;
	v.xyz = v.xyz - cpos;
	gl_Position = gl_ProjectionMatrix * (crot * (gl_ModelViewMatrix * v));
}