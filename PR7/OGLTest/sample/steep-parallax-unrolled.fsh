//
// Unrolled steep parallax mapping fragment shader
// 

varying	vec3 et;
varying	vec3 lt;

uniform sampler2D decalMap;
uniform sampler2D heightMap;

void main (void)
{
	const float numSteps  = 5.0;
	const float	bumpScale = 0.03;
	
																
	float	step   = 1.0 / numSteps;
	vec2	dtex   = -et.xy * bumpScale / ( numSteps * et.z );	// adjustment for one layer
	float	height = 1.0;										// height of the layer
	vec2	tex    = gl_TexCoord [0].xy;						// our initial guess
	float	h      = texture2D ( heightMap, tex ).r;			// get height

	if ( h < height )
	{
		height -= step;
		tex    += dtex;
		h       = texture2D ( heightMap, tex ).r;
		
		if ( h < height )
		{
			height -= step;
			tex    += dtex;
			h       = texture2D ( heightMap, tex ).r;
		
			if ( h < height )
			{
				height -= step;
				tex    += dtex;
				h       = texture2D ( heightMap, tex ).r;
		
				if ( h < height )
				{
					height -= step;
					tex    += dtex;
					h       = texture2D ( heightMap, tex ).r;
				
					if ( h < height )
					{
						height -= step;
						tex    += dtex;
						h       = texture2D ( heightMap, tex ).r;			
					}		
				}		
			}
		}	
	}	
																// now offset texture coordinates with height
	gl_FragColor = vec4 ( texture2D ( decalMap, tex ).rgb, 1.0 );
}
