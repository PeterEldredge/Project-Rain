/*
	E-mail : thomas.ir.rasor@gmail.com
	This script uses v1.5 Texture Swapping to swap between textures in a list.
	This script also includes functionality for toggling the material On and Off entirely,
	accessible through method calls or via the custom inspector.

	Methods in this script that should be called externally:
		SwapTexture()
		SwapTexture( int texture_id )
		ToggleState()
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if ( UNITY_EDITOR )
using UnityEditor;
using gui = UnityEngine.GUILayout;
using egui = UnityEditor.EditorGUILayout;
#endif

namespace SSFS
{
	public enum TextureSwapMode { Manual, Automatic, External }

	public class TextureSwapper : MonoBehaviour
	{
		public bool off = false;
		public TextureSwapMode mode = TextureSwapMode.Automatic;
		public bool remote = false;
		public Renderer targetRenderer = null;
		[System.NonSerialized]
		Renderer _r;
		public Renderer r { get { if ( remote ) _r = targetRenderer; if ( _r == null ) _r = GetComponent<Renderer>(); return _r; } }
		[System.NonSerialized]
		Material _m = null;
		public Material m
		{
			get
			{
				if ( _m == null && r != null && r.material != null )
				{
					_m = new Material( r.material );
					_m.name = "Temp SSFS Material";
					r.sharedMaterial = _m;
				}
				return _m;
			}
		}
		public List<Texture> textures = new List<Texture>();
		[Range( 0.5f , 5f )]
		public float transitionSpeed = 1f;
		public float transitionDelay = 5f;
		public KeyCode swapKey = KeyCode.Space;
		public bool randomOrder = false;
		bool _swapping = false;
		public bool swapping { get { return _swapping; } }

		float t = 0.1f;
		float tr = 1f;
		int i = 0;
		int ni = 0;



		public void Update()
		{
			t = Time.deltaTime * transitionSpeed;
			if ( mode == TextureSwapMode.Automatic )
			{
				tr -= t;
				if ( tr <= 0f )
				{
					SwapTexture();
					tr = transitionDelay;
				}
			}
			else if ( mode == TextureSwapMode.Manual && Input.GetKeyDown( swapKey ) )
				SwapTexture();
		}

		//Call this method from outside this script. Provide an ID to use a specific texture in textures.
		//If no ID or a negative id is provided, the script will choose either a random ID or the next ID in textures based on the randomOrder boolean.
		public void SwapTexture( int id = -1 )
		{
			if ( textures.Count < 2 ) return;
			int nextIndex = ( id < 0 ) ? ( randomOrder ? Random.Range( 0 , textures.Count ) : ( int )Mathf.Repeat( i + 1f , textures.Count ) ) : Mathf.Clamp( id , 0 , textures.Count );
			StartCoroutine( Transition( nextIndex ) );
		}

		public void ToggleState ()
		{
			StartCoroutine( Toggle() );
		}

		IEnumerator Transition( int nextIndex )
		{
			if ( m != null && textures.Count > 1 && !_swapping && !off )
			{
				_swapping = true;
				float p = 1f;
				m.SetFloat( "_Phase" , p );

				ni = nextIndex;
				m.SetTexture( "_MainTex" , textures[ i ] );
				m.SetTexture( "_MainTex2" , textures[ ni ] );
				m.EnableKeyword( "TEXTURE_SWAP" );

				while ( p > 0f )
				{
					m.SetFloat( "_Phase" , p );
					p -= t;
					yield return new WaitForEndOfFrame();
				}

				i = ni;
				m.SetTexture( "_MainTex" , textures[ i ] );
				m.SetFloat( "_Phase" , 1f );
				_swapping = false;
			}
		}

		IEnumerator Toggle ()
		{
			if ( m != null && !_swapping )
			{
				_swapping = true;
				m.SetTexture( "_MainTex2" , null );
				m.DisableKeyword( "TEXTURE_SWAP" );
				float p = off ? 0f : 1f;
				while ( off ? p < 1f : p > 0f )
				{
					p += off ? t : -t;
					m.SetFloat( "_Phase" , p );
					yield return new WaitForEndOfFrame();
				}
				off = !off;
				_swapping = false;
			}
		}
	}


#if ( UNITY_EDITOR )
	[CustomEditor( typeof( TextureSwapper ) )]
	public class SSFS_TextureSwapper_Editor : Editor
	{
		TextureSwapper ts;

		GUIStyle _button = null;
		public GUIStyle button
		{
			get
			{
				if ( _button == null )
				{
					_button = new GUIStyle( GUI.skin.box );
					_button.fixedHeight = 20f;
					_button.margin = new RectOffset( 0 , 0 , 0 , 0 );
				}
				return _button;
			}
		}

		public override void OnInspectorGUI()
		{
			ts = ( TextureSwapper )target;

			egui.Space();
			gui.BeginHorizontal();
			ts.remote = egui.ToggleLeft( "External Renderer" , ts.remote );
			if ( ts.remote ) ts.targetRenderer = ( Renderer )egui.ObjectField( ts.targetRenderer , typeof( Renderer ) , true);
			gui.EndHorizontal();

			GUIContent mode = new GUIContent( "Mode" );
			switch(ts.mode)
			{
				case TextureSwapMode.Automatic:
					mode.tooltip = "Textures are swapped automatically over time.";
					break;
				case TextureSwapMode.Manual:
					mode.tooltip = "Textures are swapped by key press.";
					break;
				case TextureSwapMode.External:
					mode.tooltip = "Textures are swapped when SwapTexture() is called from script.";
					break;
			}
			ts.mode = ( TextureSwapMode )egui.EnumPopup( mode , ts.mode );
			gui.BeginVertical( GUI.skin.box );
			if ( ts.mode == TextureSwapMode.Automatic )
				ts.transitionDelay = egui.Slider( "Swap Delay" , ts.transitionDelay , 0.2f , 10f );
			else if ( ts.mode == TextureSwapMode.Manual )
				ts.swapKey = ( KeyCode )egui.EnumPopup( "Swap Key" , ts.swapKey );
			if ( ts.mode != TextureSwapMode.External )
				ts.randomOrder = egui.ToggleLeft( "Random Order" , ts.randomOrder );

			ts.transitionSpeed = egui.Slider( "Swap Speed" , ts.transitionSpeed , 0.5f , 10f );
			gui.EndVertical();
			egui.Space();

			SerializedObject o = new SerializedObject( target );
			egui.PropertyField( o.FindProperty( "textures" ) , new GUIContent( "Textures" ) , true );
			o.ApplyModifiedProperties();
			egui.Space();

			if ( Application.isPlaying )
			{
				gui.BeginHorizontal();
				if ( gui.Button( "Switch Texture" ) ) ts.SwapTexture();
				if ( gui.Button( " Toggle State " ) ) ts.ToggleState();
				gui.EndHorizontal();
			}
		}
	}
#endif
}