#version 120
uniform float worldTime;
uniform sampler2D tex;
uniform sampler2D depthTex;

uniform float focalDist;

void main()
{
	float dx = 2.0 / 1280.0;
	float dy = 2.0 / 0720.0;
	
	float depth = texture2D(depthTex, gl_TexCoord[0].xy).x;
	float n = 1.0; // camera z near
	float f = 100000.0; // camera z far
	depth = 2000.0 * (2.0 * n) / (f + n - depth * (f - n));

	
	float focalDist2 = 2000.0 * (2.0 * n) / (f + n - focalDist * (f - n));

	depth = abs(1.0 - sqrt(depth / focalDist2));
	depth = clamp(depth, 0.0, 1.0);
	vec4 sum = vec4(0.0);
	if(depth >= 0.5)
	{
		float[] pascal = float[7](1.0, 6.0, 15.0, 20.0, 15.0, 6.0, 1.0);
		
		for(int x = 0; x < 7; x++)
		{
			for(int y = 0; y < 7; y++)
			{
				sum = sum + texture2D(tex, gl_TexCoord[0].xy + vec2((x - 3) * dx, (y - 3) * dy)) * pascal[x] * pascal[y];
			}
		}
		sum = sum / 4096.0;
		//sum.r = 1.0;
		depth = 1.0;
	} else {
		float[] pascal = float[5](1.0, 4.0, 6.0, 4.0, 1.0);
		
		for(int x = 0; x < 5; x++)
		{
			for(int y = 0; y < 5; y++)
			{
				sum = sum + texture2D(tex, gl_TexCoord[0].xy + vec2((x - 2) * dx, (y - 2) * dy)) * pascal[x] * pascal[y];
			}
		}
		sum = sum / 256.0;
		//sum.g = 1.0;
		depth = depth * 2;
	}
	
	vec4 diff = texture2D(tex, gl_TexCoord[0].xy);
	//diff.b = 1.0;
	gl_FragColor = sum * depth + diff * (1.0 - depth);
}