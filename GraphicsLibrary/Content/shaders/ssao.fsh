#version 120
uniform float worldTime;
uniform float width;
uniform float height;
uniform sampler2D tex;
uniform sampler2D depthTex;

void main()
{
	float dx = 2.0 / 1280.0;
	float dy = 2.0 / 0720.0;
	
	vec4 sum = vec4(0.0);
	vec4 fin;
	float[] pascal = float[7](1.0, 6.0, 15.0, 20.0, 15.0, 6.0, 1.0);
		
	for(int x = 0; x < 7; x++)
	{
		for(int y = 0; y < 7; y++)
		{
			fin = texture2D(tex, gl_TexCoord[0].xy + vec2((x - 3) * dx, (y - 3) * dy)) - vec4(0.9, 0.9, 0.9, 0.0);
			fin.r = max(0.0, fin.r);
			fin.g = max(0.0, fin.g);
			fin.b = max(0.0, fin.b);
			sum = sum + fin * pascal[x] * pascal[y];
		}
	}
	sum = 10.0 * sum / 4096.0;
	
	gl_FragColor = texture2D(tex, gl_TexCoord[0].xy);

	if(gl_FragColor.r < 0.9) {
		gl_FragColor.r += sum.r;
	}
	if(gl_FragColor.g < 0.9) {
		gl_FragColor.g += sum.g;
	}
	if(gl_FragColor.b < 0.9) {
		gl_FragColor.b += sum.b;
	}
	//gl_FragColor = sum;
}