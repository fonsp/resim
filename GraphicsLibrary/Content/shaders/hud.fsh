#version 120
uniform float worldTime;
uniform sampler2D tex;

void main()
{
	gl_FragColor = texture2D(tex, gl_TexCoord[0].xy) * gl_Color;
}