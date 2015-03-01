#version 120
uniform float worldTime;
uniform sampler2D tex;
uniform sampler2D ditherTex;

void main()
{
	float r = int(texture2D(ditherTex, gl_FragCoord.xy / 4).r < texture2D(tex, gl_TexCoord[0].xy).r);
	float g = int(texture2D(ditherTex, gl_FragCoord.xy / 4).r < texture2D(tex, gl_TexCoord[0].xy).g);
	float b = int(texture2D(ditherTex, gl_FragCoord.xy / 4).r < texture2D(tex, gl_TexCoord[0].xy).b);
	
	gl_FragColor = vec4(r, g, b, 1.0);
}