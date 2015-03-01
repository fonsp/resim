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
	gl_FragColor = texture2D(tex, gl_TexCoord[0].xy) * gl_Color * 0.5 + vec4(0.5, 0.0, 0.0, 0.5);
}