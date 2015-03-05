#version 120
#extension GL_EXT_gpu_shader4 : enable

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

varying float intensity;

void main()
{
	float a = sqrt(intensity);
	
	vec4 fragout = texture2D(tex, gl_TexCoord[0].xy) * vec4(vec3(gl_Color.w), 1.0) * vec4(gl_Color.xyz, 1.0) * (vec4(a, a, a, 1.0) * gl_LightSource[0].diffuse + vec4(1.0-a, 1.0-a, 1.0-a, 1.0) * gl_LightSource[0].ambient);
	if((effects / 2) % 2 == 1)
	{
		fragout = fragout * vec4(vec3(1.0 / sqrt(1.0 - bL * bL) + dopp), 1.0);
	}
	if((effects / 4) % 2 == 1)
	{
		vec4 shift = vec4(1.0);
		shift.r = 2 * max(0.0, 0.5 - abs(dopp + 0.0)) * fragout.r + 2 * max(0.0, 0.5 - abs(dopp + 0.5)) * fragout.g + 2 * max(0.0, 0.5 - abs(dopp + 1.0)) * fragout.b;
		shift.g = 2 * max(0.0, 0.5 - abs(dopp - 0.5)) * fragout.r + 2 * max(0.0, 0.5 - abs(dopp + 0.0)) * fragout.g + 2 * max(0.0, 0.5 - abs(dopp + 0.5)) * fragout.b;
		shift.b = 2 * max(0.0, 0.5 - abs(dopp - 1.0)) * fragout.r + 2 * max(0.0, 0.5 - abs(dopp - 0.5)) * fragout.g + 2 * max(0.0, 0.5 - abs(dopp + 0.0)) * fragout.b;
		fragout = shift;
	}

	a = max(0.0, fragout.r - 1.0);
	fragout.g += a;
	fragout.b += a;
	a = max(0.0, fragout.g - 1.0);
	fragout.r += a;
	fragout.b += a;
	a = max(0.0, fragout.b - 1.0);
	fragout.r += a;
	fragout.g += a;

	fragout += vec4(vec3(a), 0.0);
	gl_FragColor = fragout;
}