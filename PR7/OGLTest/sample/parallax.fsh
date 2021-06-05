//
// Simple parallax effect fragment shader
// 

varying	vec3 et;
varying	vec3 lt;

uniform sampler2D decalMap;
uniform sampler2D heightMap;
uniform	float	  scale;
uniform	float	  bias;
	
void main (void)
{
																// get corrected height
	float	h    = scale * (1.0-texture2D ( heightMap, gl_TexCoord [0].xy ).b) + bias;
	
																// now offset texture coordinates with height
	vec2	tex  = gl_TexCoord [0].xy - et.xy * h / et.z;
															
	gl_FragColor = vec4 ( texture2D ( decalMap, tex ).rgb, 1.0 );
}
