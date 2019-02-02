Shader "MiDaEm/NeonAnimated"
{
	Properties
	{
		_DIF("DIF", 2D) = "white" {}
		_Em_Power("Em_Power", Float) = 1.5
		_ScanLines("ScanLines", Float) = 50
		_SpeedScanLines("SpeedScanLines", Float) = 5
		_HardnessScanline("HardnessScanline", Float) = 1
		_MinAlphaScanlines("MinAlphaScanlines", Range( 0 , 1)) = 0.6
		_MaxAlphaScanlines("MaxAlphaScanlines", Range( 0 , 1)) = 1
		_BlinkSpeed("BlinkSpeed", Float) = 1.5
		_MinAlphaBlink("MinAlphaBlink", Range( 0 , 1)) = 0.5
		_MaxAlphaBlink("MaxAlphaBlink", Range( 0 , 1)) = 0.5
		_RandomBlinkTexture("RandomBlinkTexture", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard alpha:fade keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _DIF;
		uniform float4 _DIF_ST;
		uniform float _Em_Power;
		uniform float _MinAlphaScanlines;
		uniform float _MaxAlphaScanlines;
		uniform float _ScanLines;
		uniform float _SpeedScanLines;
		uniform float _HardnessScanline;
		uniform float _MinAlphaBlink;
		uniform float _MaxAlphaBlink;
		uniform sampler2D _RandomBlinkTexture;
		uniform float _BlinkSpeed;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_DIF = i.uv_texcoord * _DIF_ST.xy + _DIF_ST.zw;
			o.Emission = ( tex2D( _DIF, uv_DIF ) * _Em_Power ).rgb;
			float clampResult23 = clamp( pow( (0.0 + (sin( ( _ScanLines * ( (i.uv_texcoord).y - ( _Time.x * _SpeedScanLines ) ) * 6.28318548202515 ) ) - -1.0) * (1.0 - 0.0) / (1.0 - -1.0)) , _HardnessScanline ) , 0.0 , 1.0 );
			float lerpResult8 = lerp( _MinAlphaScanlines , _MaxAlphaScanlines , clampResult23);
			float temp_output_16_0 = ( _BlinkSpeed * ( _Time.y / 15.0 ) );
			float2 appendResult14 = (float2(temp_output_16_0 , temp_output_16_0));
			float lerpResult9 = lerp( _MinAlphaBlink , _MaxAlphaBlink , tex2D( _RandomBlinkTexture, appendResult14 ).r);
			o.Alpha = ( lerpResult8 * lerpResult9 );
		}

		ENDCG
	}
}
