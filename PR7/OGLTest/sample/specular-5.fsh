//
// Specular bump mapping fragment shader
//

varying	vec3 et;
varying	vec3 lt;
varying	vec3 ht;

uniform sampler2D decalMap;
uniform sampler2D heightMap;

void main (void)
{
	const float	specExp   = 80.0;

	vec2	tex    = gl_TexCoord [0].xy;						// our initial guess
																// now offset texture coordinates with height
	vec3	color = texture2D ( decalMap, tex ).rgb;
	vec3	n     = normalize ( texture2D ( heightMap, tex ).rgb * 2.0 - vec3 ( 1.0 ) );
	float	diff  = max ( dot ( n, normalize ( lt ) ), 0.0 );
	float	spec  = pow ( max ( dot ( n, normalize ( ht ) ), 0.0 ), specExp );

	gl_FragColor = vec4 ( color * diff + vec3 ( 0.7 * spec ), 1.0 );
}
