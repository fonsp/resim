#version 120
#extension GL_EXT_gpu_shader4 : enable
uniform float worldTime;
uniform int effects;
uniform float b;
uniform vec3 vdir;
uniform vec3 cpos;
uniform mat4 crot;
uniform sampler2D tex;
varying float dopp;

void main()
{
	vec4 fragout = texture2D(tex, gl_TexCoord[0].xy) * vec4(vec3(gl_Color.w), 1.0);
	if((effects / 2) % 2 == 1)
	{
		fragout = fragout * vec4(vec3(0.5 + dopp / 2.0), 1.0);
	}
	if((effects / 4) % 2 == 1)
	{
		vec4 shift = vec4(1.0);
		shift.r = 2 * max(0, 0.5 - abs(dopp + 0.0)) + 2 * max(0, 0.5 - abs(dopp + 0.5)) + 2 * max(0, 0.5 - abs(dopp + 1.0));
		shift.g = 2 * max(0, 0.5 - abs(dopp - 0.5)) + 2 * max(0, 0.5 - abs(dopp + 0.0)) + 2 * max(0, 0.5 - abs(dopp + 0.5));
		shift.b = 2 * max(0, 0.5 - abs(dopp - 1.0)) + 2 * max(0, 0.5 - abs(dopp - 0.5)) + 2 * max(0, 0.5 - abs(dopp + 0.0));
		fragout = fragout * shift;
	}
	gl_FragColor = fragout;
}