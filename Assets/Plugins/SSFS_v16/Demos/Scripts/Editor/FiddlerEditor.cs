/*
	This is part of the Sinuous Sci-Fi Signs v1.5 package
	Copyright (c) 2014-2017 Thomas Rasor
	E-mail : thomas.ir.rasor@gmail.com

	NOTE : 
	This is a script to test the parameters of any material.
*/

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using gui = UnityEngine.GUILayout;
using egui = UnityEditor.EditorGUILayout;

[CustomEditor(typeof(ParameterFiddler))]
public class FiddlerEditor : Editor
{
	ParameterFiddler f;
	Vector2 scr = Vector2.zero;
	int selected = -1;
	int highlighted = -1;
	Color selectColor = new Color( 0.9f , 0.95f , 1f );
	Color highlightColor = new Color( 1f , 0.9f , 0.8f );

	GUIStyle nameStyle , nameStyle2;
	GUIStyle nameBold;
	GUIStyle buttonStyle;
	GUIStyle scrollStyle , scrollBarStyle;
	GUIStyle boxStyle;

	public void SetupGUIStyles()
	{
		nameStyle = new GUIStyle( GUI.skin.box );
		nameStyle.stretchWidth = true;
		nameStyle.margin = new RectOffset( 0 , 0 , 0 , 0 );
		nameStyle.padding = new RectOffset( 4 , 4 , 3 , 3 );
		nameStyle.overflow.bottom = 1;
		nameStyle.wordWrap = false;

		nameStyle2 = new GUIStyle( nameStyle );
		nameStyle.alignment = TextAnchor.LowerLeft;

		nameBold = new GUIStyle( nameStyle );
		nameBold.normal.background = null;
		nameBold.fontStyle = FontStyle.Bold;
		nameBold.alignment = TextAnchor.LowerCenter;

		boxStyle = new GUIStyle( GUI.skin.box );
		boxStyle.margin = new RectOffset( 0 , 0 , 0 , 0 );
		boxStyle.padding = new RectOffset( 20 , 20 , 12 , 12 );
		boxStyle.overflow.top = 1;
		boxStyle.overflow.bottom = 1;

		scrollStyle = new GUIStyle( GUI.skin.scrollView );
		scrollStyle.normal.background = GUI.skin.box.normal.background;
		scrollStyle.border = GUI.skin.box.border;
		scrollStyle.margin = new RectOffset( 8 , 8 , 8 , 8 );
		scrollStyle.overflow = new RectOffset( 8 , 8 , 8 , 8 );
		scrollStyle.padding = new RectOffset( 0 , 0 , 0 , 0 );
		scrollStyle.stretchWidth = true;
		scrollStyle.stretchHeight = false;

		buttonStyle = new GUIStyle( GUI.skin.button );
		buttonStyle.normal.background = GUI.skin.box.normal.background;
		buttonStyle.normal.textColor = Color.black;
		buttonStyle.hover.background = GUI.skin.box.normal.background;
		buttonStyle.hover.textColor = Color.black;
		buttonStyle.active.background = GUI.skin.box.normal.background;
		buttonStyle.active.textColor = Color.black;
	}

	Color baseGUIColor = Color.white;
	public override void OnInspectorGUI()
	{
		baseGUIColor = GUI.color;
		SetupGUIStyles();
		Undo.RecordObject( ( ParameterFiddler )target , "Modify Material Fiddler" );

		f = target as ParameterFiddler;
		f.automatic = egui.Toggle( "Automatic Fiddling" , f.automatic );
		f.showMenu = egui.Toggle( "Menu" , f.showMenu );
		if ( f.automatic )
			f.fiddleTime = egui.Slider( "Fiddle Time" , f.fiddleTime , 0.2f , 5f );

		f.sourceMaterial = (Material)egui.ObjectField( "Target Material" , f.sourceMaterial , typeof( Material ) , false );

		gui.Space( 10 );

		gui.BeginHorizontal();
		gui.FlexibleSpace();
		gui.Label( "Target Parameters" , EditorStyles.boldLabel );
		gui.FlexibleSpace();
		gui.EndHorizontal();

		gui.BeginHorizontal();
		gui.Space( 2 );
		if ( gui.Button( "Clear All" , nameStyle2 , gui.Width( 100f ) ) ) f.parameters.Clear();
		gui.FlexibleSpace();
		if ( gui.Button( "Add New" , nameStyle2 , gui.Width( 100f ) ) ) f.parameters.Add( new MaterialParameter() );
		gui.Space( 6 );
		gui.EndHorizontal();

		gui.BeginHorizontal();
		gui.Space( 10 );

		scr = egui.BeginScrollView( scr , false , false , GUIStyle.none, GUIStyle.none , scrollStyle );
		for ( int i = 0 ; i < f.parameters.Count ; i++ )
		{
			MaterialParameter p = f.parameters[ i ];

			GUI.color = selected == i ? selectColor : ( highlighted == i ? highlightColor : baseGUIColor * 0.9f );

			gui.BeginHorizontal();
			GUIContent buttonLabel = new GUIContent( p.displayName , selected == i ? "Click To Collapse" : (highlighted == i ? "Click To Expand" : "Click To Select" ) );
			if ( gui.Button( buttonLabel , nameStyle ) )
			{
				if ( selected != i )
				{
					if ( highlighted == i )
						selected = i;
					else
						highlighted = i;
				}
				else
					selected = -1;
			}
			if ( highlighted == selected ) highlighted = -1;
			if ( selected == i || highlighted == i )
			{
				GUIContent remove = new GUIContent( "Remove" , "Delete Parameter" );
				if ( gui.Button( remove , nameStyle2 , gui.Width( 60f ) ) )
				{
					if ( selected == i )
					{
						f.parameters.RemoveAt( selected );
						selected = -1;
					}
					else
					{
						f.parameters.RemoveAt( highlighted );
						highlighted = -1;
					}
				}
			}

			GUIContent dupe = new GUIContent( "Dupe" , "Duplicate Parameter" );
			if ( gui.Button( dupe , nameStyle2 , gui.Width( 40f ) ) )
			{
				f.parameters.Insert( i + 1 , new MaterialParameter( f.parameters[ i ] ) );
				highlighted = i + 1;
			}
			GUIContent moveup = new GUIContent( "↑" , "Shift Up" );
			GUIContent movedown = new GUIContent( "↓" , "Shift Down" );
			if ( gui.Button( moveup , nameStyle2 , gui.Width( 16f ) ) )
			{
				if ( i > 0 )
				{
					MaterialParameter temp = f.parameters[ i ];
					f.parameters.RemoveAt( i );
					f.parameters.Insert( i - 1 , temp );
					highlighted = i - 1;
				}
			}
			if ( gui.Button( movedown , nameStyle2 , gui.Width( 16f ) ) )
			{
				if ( i < f.parameters.Count - 1 )
				{
					MaterialParameter temp = f.parameters[ i ];
					f.parameters.RemoveAt( i );
					f.parameters.Insert( i + 1 , temp );
					highlighted = i + 1;
				}
			}
			gui.EndHorizontal();

			if ( selected == i )
			{
				gui.BeginVertical( boxStyle );
				GUI.color = selectColor + baseGUIColor * 0.05f;
				p.displayName = egui.TextField( "Formal Name" , p.displayName );
				p.parameterName = egui.TextField( "Parameter Name" , p.parameterName );

				gui.BeginHorizontal();
				GUIContent typeContent = new GUIContent( "Parameter Type" );
				p.type = ( MaterialParameter.MaterialParamType )egui.EnumPopup( typeContent , p.type );
				gui.EndHorizontal();

				switch(p.type)
				{
					case MaterialParameter.MaterialParamType.number:
						p.min = egui.Slider( "Min" , p.min , 0f , 1f );
						segment( ref p.min );
						p.max = egui.Slider( "Max" , p.max , 0f , 1f );
						segment( ref p.max );
						break;
					case MaterialParameter.MaterialParamType.vector:
						gui.BeginHorizontal();
						egui.PrefixLabel( new GUIContent( "Labels" ) );
						p.label_x = egui.TextField( p.label_x );
						p.label_y = egui.TextField( p.label_y );
						p.label_z = egui.TextField( p.label_z );
						p.label_w = egui.TextField( p.label_w );
						gui.EndHorizontal();

						gui.BeginHorizontal();
						egui.PrefixLabel( new GUIContent( "Min" ) );
						p.min4.x = egui.FloatField( p.min4.x );
						p.min4.y = egui.FloatField( p.min4.y );
						p.min4.z = egui.FloatField( p.min4.z );
						p.min4.w = egui.FloatField( p.min4.w );
						gui.EndHorizontal();

						gui.BeginHorizontal();
						egui.PrefixLabel( new GUIContent( "Max" ) );
						p.max4.x = egui.FloatField( p.max4.x );
						p.max4.y = egui.FloatField( p.max4.y );
						p.max4.z = egui.FloatField( p.max4.z );
						p.max4.w = egui.FloatField( p.max4.w );
						gui.EndHorizontal();
						break;
					case MaterialParameter.MaterialParamType.texture:
						TextureList( p );
						break;
				}

				p.repeat = egui.IntField( "Repeats" , p.repeat );

				gui.EndVertical();
			}
			GUI.color = baseGUIColor;
		}
		egui.EndScrollView();

		gui.Space( 10 );
		gui.EndHorizontal();
		gui.Space( 10 );

		gui.BeginHorizontal();
		gui.Space( 2 );
		gui.Label( "Slider Bar" , nameStyle , gui.Width(90f));
		f.sliderBar = ( Texture )egui.ObjectField( f.sliderBar , typeof( Texture ) , false );
		gui.Space( 6 );
		gui.EndHorizontal();

		gui.BeginHorizontal();
		gui.Space( 2 );
		gui.Label( "Slider Thumb" , nameStyle , gui.Width( 90f ) );
		f.sliderThumb = ( Texture )egui.ObjectField( f.sliderThumb , typeof( Texture ) , false );
		gui.Space( 6 );
		gui.EndHorizontal();

		gui.Space( 10 );
	}

	void TextureList ( MaterialParameter p )
	{
		gui.Space( 10 );

		gui.BeginHorizontal();
		gui.Label( "Textures" , nameBold );
		List<Texture> selectedTextures = new List<Texture>();
		foreach ( Object o in Selection.objects )
		{
			if ( o is Texture )
				selectedTextures.Add( o as Texture );
		}

		if ( selectedTextures.Count > 0 )
		{
			if ( gui.Button( "Add Selected" , nameStyle2 , gui.MaxWidth( 100f ) ) )
			{
				p.textures.AddRange( selectedTextures );
			}
		}
		if ( gui.Button( "Add New" , nameStyle2 , gui.MaxWidth( 102f ) ) ) p.textures.Add( null );
		gui.Space( boxStyle.padding.right );
		gui.EndHorizontal();

		gui.BeginVertical( boxStyle );
		for ( int i = 0 ; i < p.textures.Count ; i++ )
		{
			gui.BeginHorizontal();
			p.textures[ i ] = ( Texture )egui.ObjectField( p.textures[ i ] , typeof( Texture ) , false , gui.MaxHeight( 19 ) );
			if ( gui.Button( "Remove" , nameStyle2 , gui.Width( 70f ) ) ) p.textures.RemoveAt( i );
			GUIContent moveup = new GUIContent( "↑" , "Shift Up" );
			GUIContent movedown = new GUIContent( "↓" , "Shift Down" );
			if ( gui.Button( moveup , nameStyle2 , gui.Width( 16f ) ) )
			{
				if ( i > 0 )
				{
					Texture temp = p.textures[ i ];
					p.textures.RemoveAt( i );
					p.textures.Insert( i - 1 , temp );
				}
			}
			if ( gui.Button( movedown , nameStyle2 , gui.Width( 16f ) ) )
			{
				if ( i < p.textures.Count - 1 )
				{
					Texture temp = p.textures[ i ];
					p.textures.RemoveAt( i );
					p.textures.Insert( i + 1 , temp );
				}
			}
			gui.EndHorizontal();
		}

		gui.EndVertical();
		gui.Space( 10 );
	}

	void segment ( ref float v , int segments = 20 )
	{
		v = Mathf.Floor( v * segments ) / segments;
	}
}