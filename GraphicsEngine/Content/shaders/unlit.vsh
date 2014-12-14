#version 120
uniform float worldTime;
uniform int effects;
uniform float b;
uniform vec3 vdir;
uniform vec3 cpos;
uniform mat4 crot;

void main()
{
    gl_Position = ftransform();
	gl_TexCoord[0] = gl_MultiTexCoord0;
	gl_FrontColor = gl_Color;
}