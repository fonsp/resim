#version 120
uniform float worldTime;
uniform sampler2D tex;
uniform sampler2D depthTex;

void main()
{
	/*vec4 main = texture2D(tex, gl_TexCoord[0].xy) * gl_Color;
	vec3 gray = vec3(dot(vec3(0.2126,0.7152,0.0722), main.xyz));
	gl_FragColor = vec4(vec3(mix(main.xyz, gray, gray.r * -2.0 + 0.5)), main.w);*/
	gl_FragColor = texture2D(tex, gl_TexCoord[0].xy) * gl_Color;
	
	/*float depth = texture2D(depthTex, gl_TexCoord[0].xy).x;
	float n = 1.0; // camera z near
	float f = 100000.0; // camera z far
	depth = 200.0 * (2.0 * n) / (f + n - depth * (f - n)); // in meters

	gl_FragColor.w = depth;*/
}