#version 120
uniform float worldTime;
uniform sampler2D tex;

void main()
{
	/*vec2 rCoord = (gl_TexCoord[0].xy - vec2(0.5, 0.5)) * 0.995 + vec2(0.5, 0.5);

	float r = texture2D(tex, rCoord).r;
	float g = texture2D(tex, gl_TexCoord[0].xy).g;
	float b = texture2D(tex, gl_TexCoord[0].xy).b;
	gl_FragColor = vec4(r, g, b, 1.0); chromatic aberration*/
	/*gl_FragColor = vec4(texture2D(tex, gl_TexCoord[0].xy).xyz - texture2D(tex, gl_TexCoord[0].xy + vec2(1.0/1280.0, 1.0/720.0)).xyz, 1.0); diff*/
	float r = texture2D(tex, (gl_TexCoord[0].xy - vec2(0.5, 0.5)) * 0.992 + vec2(0.5, 0.5)).r;
	float g = texture2D(tex, gl_TexCoord[0].xy).g;
	float b = texture2D(tex, (gl_TexCoord[0].xy - vec2(0.5, 0.5)) * 0.998 + vec2(0.5, 0.5)).b;
	gl_FragColor = vec4(vec3(r, g, b) * (mod(int(gl_FragCoord.y), 2) + 1.0) * 0.5, 1.0);
}