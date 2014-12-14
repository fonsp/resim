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
	
	vec4 output = texture2D(tex, gl_TexCoord[0].xy) * vec4(vec3(gl_Color.w), 1.0) * (vec4(a, a, a, 1.0) * gl_LightSource[0].diffuse + vec4(1.0-a, 1.0-a, 1.0-a, 1.0) * gl_LightSource[0].ambient);
	if((effects / 2) % 2 == 1)
	{
		output = output * vec4(vec3(1.0 + dopp), 1.0);
	}
	if((effects / 4) % 2 == 1)
	{
		vec4 shift = vec4(1.0);
		shift.r = 2 * max(0, 0.5 - abs(dopp + 0.0)) * output.r + 2 * max(0, 0.5 - abs(dopp + 0.5)) * output.g + 2 * max(0, 0.5 - abs(dopp + 1.0)) * output.b;
		shift.g = 2 * max(0, 0.5 - abs(dopp - 0.5)) * output.r + 2 * max(0, 0.5 - abs(dopp + 0.0)) * output.g + 2 * max(0, 0.5 - abs(dopp + 0.5)) * output.b;
		shift.b = 2 * max(0, 0.5 - abs(dopp - 1.0)) * output.r + 2 * max(0, 0.5 - abs(dopp - 0.5)) * output.g + 2 * max(0, 0.5 - abs(dopp + 0.0)) * output.b;
		output = shift;
	}
	gl_FragColor = output;
}