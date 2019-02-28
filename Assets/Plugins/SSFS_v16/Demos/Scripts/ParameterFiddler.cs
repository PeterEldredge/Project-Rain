/*
This script plays with a renderer's material parameters.
Included is both an automatic mode as well as a mode that lets
users change parameters.

Email :: thomas.ir.rasor@gmail.com
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using gui = UnityEngine.GUILayout;

[System.Serializable]
public class MaterialParameter
{
	public string parameterName = "_Parameter";
	public string displayName = "Parameter Value";
	public int repeat = 2;

	public enum MaterialParamType { number, color, texture, vector }
	public MaterialParamType type = MaterialParamType.number;

	public float min = 0f;
	public float max = 1f;

	public string label_x = "X", label_y = "Y", label_z = "Z", label_w = "W";
	public Vector4 min4 = Vector4.zero;
	public Vector4 max4 = Vector4.one;

	public List<Texture> textures = new List<Texture>();

	[System.NonSerialized]
	public float value;
	[System.NonSerialized]
	public Color valueC;
	[System.NonSerialized]
	public Vector4 value4;
	public int texID { get { return Mathf.FloorToInt( Mathf.Repeat( value * textures.Count , textures.Count ) ); } }

	public MaterialParameter()
	{
		parameterName = "_Parameter";
		displayName = "Parameter Value";
		repeat = 1;
		type = MaterialParamType.number;
		min = 0f;
		max = 1f;
		min4 = Vector4.zero;
		max4 = Vector4.one;
		textures = new List<Texture>();
	}

	public MaterialParameter( MaterialParameter other )
	{
		parameterName = other.parameterName;
		displayName = other.displayName;
		repeat = other.repeat;
		type = other.type;
		min = other.min;
		max = other.max;
		min4 = other.min4;
		max4 = other.max4;
		textures = new List<Texture>( other.textures );
	}
}

public class ParameterFiddler : MonoBehaviour
{
	public Material sourceMaterial;
    Material material;
	public bool automatic = true;
	public bool showMenu = false;
	public float menuVis = 0f;
	[Range(0.25f,5f)]
	public float fiddleTime = 3f;

	public List<MaterialParameter> parameters = new List<MaterialParameter>();

	public Texture sliderBar , sliderThumb;
	GUIStyle sliderStyle , sliderThumbStyle;

	int currentParameterIndex = 0;
	float t = 0f;
	float tf { get { return t / fiddleTime; } }
	bool fiddling;

	void Start ()
	{
		material = new Material( sourceMaterial );
		GetComponent<Renderer>().material = material;
		if ( parameters.Count > 0 && automatic )
			StartCoroutine( Unfiddle() );
		ManualRead();
	}

	void Update ()
	{
		if ( automatic )
		{
			if ( fiddling )
				t += Time.deltaTime;
			if ( parameters[ currentParameterIndex ].parameterName != "_Phase" )
				material.SetFloat( "_Phase" , 0.5f );
		}
		else
		{
			if (showMenu)
				menuVis = Mathf.Lerp( menuVis , ( Input.mousePosition.x > Screen.width - Screen.width * 0.2f ) ? 1f : 0f , Time.deltaTime * 10f );

			int lastId = currentParameterIndex;
			if ( Input.GetKeyDown( KeyCode.A ) )
				currentParameterIndex = ( int )Mathf.Repeat( currentParameterIndex - 1 , parameters.Count - 1 );
			if ( Input.GetKeyDown( KeyCode.D ) )
				currentParameterIndex = ( int )Mathf.Repeat( currentParameterIndex + 1 , parameters.Count - 1 );
			if ( lastId != currentParameterIndex )
				ManualRead();
			ManualFiddle();
		}
	}

	public void ManualRead ()
	{
		if ( currentParameterIndex < 0 ) return;
		MaterialParameter param = parameters[ currentParameterIndex ];
		if ( material.HasProperty( param.parameterName ) )
		{
			switch ( param.type )
			{
				case MaterialParameter.MaterialParamType.number:
					param.value = material.GetFloat( param.parameterName );
					break;
				case MaterialParameter.MaterialParamType.color:
					param.valueC = material.GetColor( param.parameterName );
					break;
				case MaterialParameter.MaterialParamType.vector:
					param.value4 = material.GetVector( param.parameterName );
					break;
				case MaterialParameter.MaterialParamType.texture:
					break;
				default:break;
			}
		}
	}

	public void ManualFiddle()
	{
		if ( currentParameterIndex < 0 ) return;
		MaterialParameter param = parameters[ currentParameterIndex ];
		if ( material.HasProperty( param.parameterName ) )
		{
			switch ( param.type )
			{
				case MaterialParameter.MaterialParamType.number:
					material.SetFloat( param.parameterName , param.value );
					break;
				case MaterialParameter.MaterialParamType.color:
					material.SetColor( param.parameterName , param.valueC );
					break;
				case MaterialParameter.MaterialParamType.vector:
					material.SetVector( param.parameterName , param.value4 );
					break;
				case MaterialParameter.MaterialParamType.texture:
					if ( param.textures.Count > 0 )
						material.SetTexture( param.parameterName , param.textures[ param.texID ] );
					break;
				default: break;
			}
		}
	}

	public void FiddleParameter()
	{
			MaterialParameter p = parameters[ currentParameterIndex ];
			if ( material.HasProperty( p.parameterName ) )
			{
				switch ( p.type )
				{
					case MaterialParameter.MaterialParamType.number: StartCoroutine( FiddleFloat( p ) ); break;
					case MaterialParameter.MaterialParamType.color: StartCoroutine( FiddleColor( p ) ); break;
					case MaterialParameter.MaterialParamType.texture: StartCoroutine( FiddleTexture( p ) ); break;
					case MaterialParameter.MaterialParamType.vector: StartCoroutine( FiddleVector( p ) ); break;
				}
			}
			else
			{
				Debug.Log( "Material Parameter Not Found: " + p.parameterName );
			}
	}

	public IEnumerator FiddleFloat( MaterialParameter p )
	{ 
		float o = material.GetFloat( p.parameterName );
		p.value = o;

		float i = 0f;
		while ( i < 1f )
		{
			i += Time.deltaTime * 6f;
			p.value = Mathf.Lerp( p.value , 0f , i );
			material.SetFloat( p.parameterName , p.value );
			yield return new WaitForEndOfFrame();
		}

		while ( t < fiddleTime )
		{
			float v = -Mathf.Cos( tf * Mathf.PI * 1.5f * p.repeat ) * 0.5f + 0.5f;
			p.value = Mathf.Lerp( p.min , p.max , v );
			material.SetFloat( p.parameterName , p.value );
			yield return new WaitForEndOfFrame();
		}

		i = 0f;
		while (i < 1f)
		{
			i += Time.deltaTime * 6f;
			p.value = Mathf.Lerp( p.value , o , i );
			material.SetFloat( p.parameterName , p.value );
			yield return new WaitForEndOfFrame();
		}

		currentParameterIndex = currentParameterIndex + 1;
		StartCoroutine( Unfiddle() );
	}

	public IEnumerator FiddleColor( MaterialParameter param )
	{
		Color o = material.GetColor( param.parameterName );
		Color c = o;

		float i = 0f;
		while ( i < 1f )
		{
			i += Time.deltaTime * 6f;
			material.SetColor( param.parameterName , Color.HSVToRGB( 0f , 0.9f , 0.9f ) );
			yield return new WaitForEndOfFrame();
		}

		while ( t < fiddleTime )
		{
			param.valueC = Color.HSVToRGB( Mathf.Repeat( tf * param.repeat , 1f ) , 0.9f , 0.9f );
			material.SetColor( param.parameterName , param.valueC );
			yield return new WaitForEndOfFrame();
		}

		i = 0f;
		while ( i < 1f )
		{
			i += Time.deltaTime * 6f;
			material.SetColor( param.parameterName , Color.Lerp( c , o , i ) );
			yield return new WaitForEndOfFrame();
		}

		material.SetColor( param.parameterName , o );
		currentParameterIndex = currentParameterIndex + 1;
		StartCoroutine( Unfiddle() );
	}

	public IEnumerator FiddleTexture( MaterialParameter param )
	{
		Texture o = material.GetTexture( param.parameterName );
		while ( t < fiddleTime )
		{
			int id = ( int )Mathf.Floor( tf * param.textures.Count );
			param.value = Mathf.Clamp01( (float)id / (float)param.textures.Count );
			material.SetTexture( param.parameterName , param.textures[ id ] );
			yield return new WaitForEndOfFrame();
		}
		material.SetTexture( param.parameterName , o );
		currentParameterIndex = currentParameterIndex + 1;
		StartCoroutine( Unfiddle() );
	}

	public IEnumerator FiddleVector( MaterialParameter param )
	{
		Vector4 o = material.GetVector( param.parameterName );
		while ( t < fiddleTime )
		{
			float v = -Mathf.Cos( tf * Mathf.PI * 1.5f * param.repeat ) * 0.5f + 0.5f;
			param.value4 = Vector4.Lerp( param.min4 , param.max4 , v );
			material.SetVector( param.parameterName , param.value4 );
			yield return new WaitForEndOfFrame();
		}
		material.SetVector( param.parameterName , o );
		currentParameterIndex = currentParameterIndex + 1;
		StartCoroutine( Unfiddle() );
	}

	IEnumerator Unfiddle ()
	{
		fiddling = false;
		yield return new WaitForSeconds( 1f );
		t = 0f;
		fiddling = true;
		FiddleParameter();
	}

	GUIStyle nameStyle, nameStyle2;
	GUIStyle buttonStyle;
	GUIStyle scrollStyle;
	GUIStyle boxStyle;

	public void SetupGUIStyles()
	{
		nameStyle = new GUIStyle( GUI.skin.box );
		if ( sliderBar != null ) nameStyle.normal.background = (Texture2D)sliderBar;
		nameStyle.normal.textColor = Color.black;
		nameStyle.stretchWidth = true;
		nameStyle.margin = new RectOffset( 0 , 0 , 0 , 0 );
		nameStyle.padding = new RectOffset( 4 , 4 , 3 , 3 );
		nameStyle.overflow.bottom = 1;
		nameStyle.overflow.right = 1;
		nameStyle.wordWrap = false;
		nameStyle.alignment = TextAnchor.LowerLeft;
		nameStyle2 = new GUIStyle( nameStyle );
		nameStyle2.alignment = TextAnchor.LowerCenter;

		boxStyle = new GUIStyle( GUI.skin.box );
		if ( sliderBar != null ) boxStyle.normal.background = ( Texture2D )sliderBar;
		boxStyle.margin = new RectOffset( 0 , 0 , 0 , 0 );
		boxStyle.padding = new RectOffset( 0 , 0 , 0 , 0 );
		boxStyle.overflow.top = 1;
		boxStyle.overflow.right = 1;
		boxStyle.overflow.bottom = 1;

		scrollStyle = new GUIStyle( GUI.skin.scrollView );
		scrollStyle.normal.background = GUI.skin.box.normal.background;
		scrollStyle.border = GUI.skin.box.border;
		scrollStyle.margin = new RectOffset( 0 , 0 , 0 , 0 );
		scrollStyle.padding = new RectOffset( 8 , 8 , 8 , 8 );
		scrollStyle.stretchWidth = true;

		buttonStyle = new GUIStyle( GUI.skin.button );
		buttonStyle.normal.background = GUI.skin.box.normal.background;
		buttonStyle.normal.textColor = Color.black;
		buttonStyle.hover.background = GUI.skin.box.normal.background;
		buttonStyle.hover.textColor = Color.black;
		buttonStyle.active.background = GUI.skin.box.normal.background;
		buttonStyle.active.textColor = Color.black;
	}

	Color baseGUIColor = Color.white;
	void OnGUI()
	{
		baseGUIColor = GUI.color;
		SetupGUIStyles();
		if ( parameters.Count > 0 && currentParameterIndex < parameters.Count && currentParameterIndex > -1 )
		{
			GUIStyle labelStyle = new GUIStyle( GUI.skin.label );
			labelStyle.alignment = TextAnchor.LowerCenter;
			labelStyle.fontStyle = FontStyle.Bold;
			labelStyle.fontSize = 24;
			labelStyle.clipping = TextClipping.Overflow;
			labelStyle.wordWrap = false;

			sliderStyle = new GUIStyle( GUI.skin.horizontalSlider );
			sliderThumbStyle = new GUIStyle( GUI.skin.horizontalSliderThumb );
			if ( sliderBar != null ) sliderStyle.normal.background = ( Texture2D )sliderBar;
			if ( sliderThumb != null )
			{
				sliderThumbStyle.normal.background = ( Texture2D )sliderThumb;
				sliderThumbStyle.hover.background = ( Texture2D )sliderThumb;
				sliderThumbStyle.active.background = ( Texture2D )sliderThumb;
			}

			MaterialParameter p = parameters[ currentParameterIndex ];
			gui.BeginArea( new Rect( 0f , 0f , Screen.width , Screen.height ) );
			gui.Space( 30f );
			gui.BeginHorizontal();
			gui.FlexibleSpace();
			gui.Label( fiddling || !automatic ? p.displayName : "" , labelStyle , gui.Height( 40f ) );
			gui.FlexibleSpace();
			gui.EndHorizontal();
			gui.FlexibleSpace();
			gui.BeginHorizontal();
			gui.Space( Screen.width * 0.25f );
			gui.BeginVertical();
			labelStyle.fontSize = 14;
			if ( automatic )
			{
				switch (p.type)
				{
					case MaterialParameter.MaterialParamType.number:
						Slider( "" , p.value );
						break;
					case MaterialParameter.MaterialParamType.texture:
						Slider( "" , p.value );
						break;
					case MaterialParameter.MaterialParamType.vector:
						Slider( p.value4 , p.min4 , p.max4 );
						break;
					case MaterialParameter.MaterialParamType.color:
						Slider( p.valueC );
						break;
				}
			}
			else
			{
				switch ( p.type )
				{
					case MaterialParameter.MaterialParamType.number:
						Slider( "" , ref p.value , p.min , p.max );
						break;
					case MaterialParameter.MaterialParamType.texture:
						Slider( p.textures[ p.texID ].name , ref p.value );
						break;
					case MaterialParameter.MaterialParamType.vector:
						Slider( ref p.value4 , p.min4 , p.max4 , p.label_x , p.label_y , p.label_z , p.label_w );
						break;
					case MaterialParameter.MaterialParamType.color:
						Slider( ref p.valueC );
						break;
				}
			}
			gui.EndVertical();
			gui.Space( Screen.width * 0.25f );
			gui.EndHorizontal();
			gui.Space( 30f );
			gui.EndArea();
		}


		if ( parameters.Count > 0 )
		{
			float w = 150f;
			float w2 = 8f;
			float w3 = (parameters.Count + 1) * 21f + 1f;
			gui.BeginArea( new Rect( Screen.width - w * menuVis - w2 , Screen.height * 0.5f - w3 * 0.5f , w + w2 , w3 )  );

			gui.BeginHorizontal();
			gui.Label( "" , boxStyle , gui.Width( w2 ) , gui.ExpandHeight( true ) );

			gui.BeginVertical();
			gui.Label( "Parameters" , nameStyle2 );
			for ( int i = 0 ; i < parameters.Count ; i++ )
			{
				GUI.color = currentParameterIndex == i ? new Color( 1f , 1f , 1f , 1f ) : new Color( 1f , 1f , 1f , 0.6f );
				MaterialParameter p2 = parameters[ i ];
				if ( gui.Button( p2.displayName , nameStyle ) )
				{
					currentParameterIndex = currentParameterIndex == i ? -1 : i;
					ManualRead();
				}
				GUI.color = baseGUIColor;
			}
			gui.EndVertical();
			gui.EndHorizontal();

			gui.EndArea();
		}
	}

	public void Slider( Color color )
	{
		GUI.color = color * 0.5f + Color.white * 0.5f;
		Slider( "R" , color.r );
		Slider( "G" , color.g );
		Slider( "B" , color.b );
		Slider( "A" , color.a );
		GUI.color = baseGUIColor;
	}

	public void Slider( ref Color color )
	{
		GUI.color = color * 0.8f + Color.white * 0.2f;
		Slider( "R" , ref color.r );
		Slider( "G" , ref color.g );
		Slider( "B" , ref color.b );
		Slider( "A" , ref color.a );
		GUI.color = baseGUIColor;
	}

	public void Slider( Vector4 v , Vector4 min4 , Vector4 max4 )
	{
		Slider( "X" , v.x , min4.x , max4.x );
		Slider( "Y" , v.y , min4.y , max4.y );
		Slider( "Z" , v.z , min4.z , max4.z );
		Slider( "W" , v.w , min4.w , max4.w );
	}

	public void Slider( ref Vector4 v , Vector4 min4 , Vector4 max4 , string xLabel = "X" , string yLabel = "Y" , string zLabel = "Z" , string wLabel = "W" )
	{
		if ( !string.IsNullOrEmpty( xLabel ) )
			Slider( xLabel , ref v.x , min4.x , max4.x );
		if ( !string.IsNullOrEmpty( yLabel ) )
		Slider( yLabel , ref v.y , min4.y , max4.y );
		if ( !string.IsNullOrEmpty( zLabel ) )
		Slider( zLabel , ref v.z , min4.z , max4.z );
		if ( !string.IsNullOrEmpty( wLabel ) )
		Slider( wLabel , ref v.w , min4.w , max4.w );
	}

	public void Slider( string label , float v , float min = 0f , float max = 1f )
	{
		gui.BeginHorizontal();
		gui.FlexibleSpace();
		if(label.Length > 0)
			gui.Label( label , gui.Width( Screen.width * 0.3f ) );
		gui.HorizontalSlider( v , min , max , sliderStyle , sliderThumbStyle , gui.Width( Screen.width*0.3f ) );
		gui.FlexibleSpace();
		gui.EndHorizontal();
	}

	public void Slider( string label , ref float v , float min = 0f , float max = 1f )
	{
		gui.BeginHorizontal();
		gui.FlexibleSpace();
		if(label.Length > 0)
			gui.Label( label , gui.Width( Screen.width * 0.3f ) );
		v = gui.HorizontalSlider( v , min , max , sliderStyle , sliderThumbStyle , gui.Width( Screen.width * 0.3f ) );
		gui.FlexibleSpace();
		gui.EndHorizontal();
	}
}