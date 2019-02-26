/*
	E-mail : thomas.ir.rasor@gmail.com
	This script anazlyzes a material's textures and uses the colors for an attached light source.
	THIS SCRIPT IS NOT MEANT TO BE FAST. OPERATIONS LIKE THESE SHOULD BE CACHED FOR OPTIMIZATION PURPOSES
	Analyzing the color of pixels in a texture is slow on the CPU
*/

using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
class LightColorFromTexture : MonoBehaviour
{
	public Renderer r = null;
	[Range( 0f , 2f )]
	public float colorIntensity = 1f;

	[ System.NonSerialized]
	Light _l = null;
	Light l { get { if ( _l == null ) _l = GetComponent<Light>(); return _l; } }

	[System.NonSerialized]
	Material m = null;
	[System.NonSerialized]
	Texture2D t1 = null, t2 = null;
	[System.NonSerialized]
	Color c1, c2;
	bool hasSwap { get { return t2 != null; } }
	Dictionary<Texture , Color> cachedEvaluations = new Dictionary<Texture , Color>();

	void Update()
	{
		if ( CheckUpdate() )
		{
			Color c = c1;
			if ( m.HasProperty( "_Phase" ) )
			{
				if ( hasSwap )
					c = Color.Lerp( c2 , c , m.GetFloat( "_Phase" ) );
				else
					c *= m.GetFloat( "_Phase" );
			}
			if ( m.HasProperty( "_Color" ) ) c *= m.GetColor( "_Color" );
			Intensify( ref c );
			if ( l != null ) l.color = c;
		}
	}

	public bool FindTexture( string propertyName , ref Texture2D texture )
	{
		if ( m.HasProperty( propertyName ) )
		{
			Texture2D tmp = ( Texture2D )m.GetTexture( propertyName );
			if (tmp != texture )
			{
				texture = tmp;
				return ( tmp != null );//returns true if we needed to update
			}
			return false;
		}
		return false;
	}

	public bool CheckUpdate ()
	{
		if ( r == null ) return false;
		if ( r.sharedMaterial != m ) m = r.sharedMaterial;
		if ( m == null ) return false;
		if ( FindTexture( "_MainTex" , ref t1 ) ) c1 = EvaluateColor( t1 );
		if ( t1 == null ) return false;
		if ( FindTexture( "_MainTex2" , ref t2 ) ) c2 = EvaluateColor( t2 );
		return true;
	}

	public void Intensify( ref Color c )
	{
		float h, s, v;
		Color.RGBToHSV( c , out h , out s , out v );
		c = Color.HSVToRGB( h , s * colorIntensity , Mathf.Clamp01( v * colorIntensity ) );
	}

	public Color EvaluateColor( Texture2D texture )
	{
		if ( texture == null ) return Color.clear;
		if ( cachedEvaluations.ContainsKey( texture ) ) return cachedEvaluations[ texture ];

		Color32[] pxls;
		try
		{
			pxls = texture.GetPixels32();
		}
		catch
		{
			return Color.clear;
		}
		Color sum = Color.black;
		int hits = 0;//we only take into account pixels that aren't transparent
		//a 1024x1024 image has over a million pixels to analyze
		//only sample every nth pixel... a high step size is usually good enough for determining general color
		int step = 100;

		for ( int i = 0 ; i < pxls.Length ; i += step )
		{
			if ( pxls[ i ].a > 0 )
			{
				sum += pxls[ i ];
				hits++;
			}
		}

		Color c = sum / hits;
		c.r = Mathf.Clamp01( c.r );
		c.g = Mathf.Clamp01( c.g );
		c.b = Mathf.Clamp01( c.b );
		c.a = 1f;
		cachedEvaluations.Add( texture , c );
		return c;
	}
}