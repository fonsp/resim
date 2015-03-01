#version 120
uniform float worldTime;
uniform sampler2D tex;

void main()
{
	/*vec4 main = texture2D(tex, gl_TexCoord[0].xy) * gl_Color;
	vec3 gray = vec3(dot(vec3(0.2126,0.7152,0.0722), main.xyz));
	gl_FragColor = vec4(vec3(mix(main.xyz, gray, gray.r * -2.0 + 0.5)), main.w);*/
	gl_FragColor = texture2D(tex, gl_TexCoord[0].xy) * gl_Color;
}