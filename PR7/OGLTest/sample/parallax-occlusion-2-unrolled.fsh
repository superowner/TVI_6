//
//  Unrolled Parallax Occlusion mapping fragment shader
//

varying	vec3 et;
varying	vec3 lt;
varying	vec3 ht;

uniform sampler2D decalMap;
uniform sampler2D heightMap;

void main (void)
{
	const float numSteps  = 5.0;
	const float	bumpScale = 0.08;
	const float	specExp   = 80.0;


	float	step   = 1.0 / numSteps;
	vec2	dtex   = -et.xy * bumpScale / ( numSteps * et.z );	// adjustment for one layer
	float	height = 1.0;										// height of the layer
	vec2	tex    = gl_TexCoord [0].xy;						// our initial guess
	float	h      = 1.0 - texture2D ( heightMap, tex ).a;			// get height

	if ( h < height )
	{
		height -= step;
		tex    += dtex;
		h       = 1.0 - texture2D ( heightMap, tex ).a;

		if ( h < height )
		{
			height -= step;
			tex    += dtex;
			h       = 1.0 - texture2D ( heightMap, tex ).a;

			if ( h < height )
			{
				height -= step;
				tex    += dtex;
				h       = 1.0 - texture2D ( heightMap, tex ).a;

				if ( h < height )
				{
					height -= step;
					tex    += dtex;
					h       = 1.0 - texture2D ( heightMap, tex ).a;

					if ( h < height )
					{
						height -= step;
						tex    += dtex;
						h       = 1.0 - texture2D ( heightMap, tex ).a;
					}
				}
			}
		}
	}
									// now find point via linear interpolation
	vec2	prev   = tex - dtex;	// previous point
	float	hPrev  = 1.0 - texture2D ( heightMap, prev ).a - (height + step); // < 0
	float	hCur   = h - height;	// > 0
	float	weight = hCur / (hCur - hPrev );
	
	tex = weight * prev + (1.0 - weight) * tex;	// -> lerp/mix
	
	vec3	color = texture2D ( decalMap, tex ).rgb;
	vec3	n     = normalize ( texture2D ( heightMap, tex ).rgb * 2.0 - vec3 ( 1.0 ) );
	float	diff  = max ( dot ( n, normalize ( lt ) ), 0.0 );
	float	spec  = pow ( max ( dot ( n, normalize ( ht ) ), 0.0 ), specExp );

													// now compute shadow
	vec3	dl   = step;	//1.0 / numSteps;
	vec2	ltex = -bumpScale * dl * lt.xy / lt.z;	// adjustment for one layer
	float	vis  = 1.0;								// light visibility
	
	if ( dot ( n, lt ) > 0 )						// no sense checking other case
	{
		height  = 1.0 - texture2D ( heightMap, tex ).a;// + dl;
		tex    -= ltex;
		
		if ( 1.0 - texture2D ( heightMap, tex ).a > height )
			vis -= 0.1;
						
		height += dl;
		tex    -= ltex;
		
		if ( 1.0 - texture2D ( heightMap, tex ).a > height )
			vis -= 0.1;

		height += dl;
		tex    -= ltex;
		
		if ( 1.0 - texture2D ( heightMap, tex ).a > height )
			vis -= 0.1;

		height += dl;
		tex    -= ltex;
		
		if ( 1.0 - texture2D ( heightMap, tex ).a > height )
			vis -= 0.1;

		height += dl;
		tex    -= ltex;
		
		if ( 1.0 - texture2D ( heightMap, tex ).a > height )
			vis -= 0.1;
	}
			
	gl_FragColor = vec4 ( vis * ( color * diff + vec3 ( 0.7 * spec ) ), 1.0 );
}
