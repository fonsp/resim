#version 120
uniform float worldTime;
uniform int effects;
uniform float b;
uniform vec3 vdir;
uniform vec3 cpos;
uniform mat4 crot;
uniform sampler2D tex;

void main()
{
	gl_FragColor = vec4(0.5, 0.0, 0.0, 0.05);
}