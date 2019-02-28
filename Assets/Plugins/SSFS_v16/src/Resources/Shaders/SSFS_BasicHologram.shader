//	This is part of the Sinuous Sci-Fi Signs v1.5 package
//	Copyright (c) 2014-2018 Thomas Rasor
//	E-mail : thomas.ir.rasor@gmail.com

Shader "Sci-Fi/SSFS/Simple Hologram"
{
	CGINCLUDE
#ifdef SHADER_API_D3D11
	#pragma target 5.0
#else
	#pragma target 3.0
#endif
	#include "UnityCG.cginc"
	#pragma vertex vert
	#pragma fragment frag

	struct appdata
	{
		float4 vertex : POSITION;
		float3 normal : NORMAL;
		float2 uv : TEXCOORD0;
#ifdef SHADER_API_D3D11
		//do instancing stuff
		UNITY_VERTEX_INPUT_INSTANCE_ID
#endif
	};

	struct v2f
	{
		float4 vertex : SV_POSITION;
		float3 normal : NORMAL;
		float2 uv : TEXCOORD0;
		float2 depth : TEXCOORD1;
		float Glitch : TEXCOORD2;
		float4 scrpos : TEXCOORD3;
		float3 pos : TEXCOORD4;
	};

	half4 _Color,_GlitchColor;
	half _Aberration, _Glitch, _GlitchSpeed, _GlitchResolution,_ScreenTexWeight;
	half _ScanlineStrength , _Intensity;
	sampler2D _MainTex;
	float4	_MainTex_ST;
	sampler2D _ScreenTex;
	float4	_ScreenTex_ST;
	sampler2D _Noise;
	uniform sampler2D _CameraDepthTexture;

	float4 noise(float2 uv) 
	{ 
		return -1.0 + 2.0 * tex2Dlod(_Noise, float4(uv,0.0,0.0)); 
	}

	float4 Glitch(float4 vpos, fixed amount, out float r)
	{
        float glitchspeed = _GlitchSpeed * 0.01;

		float4 pscrpos = ComputeScreenPos(UnityObjectToClipPos(vpos));
		float2 pscruv = pscrpos.xy / pscrpos.w;

		half4 n = 0.5 + 0.5 * noise( _Time.y * glitchspeed * float2( _ScreenParams.x / _ScreenParams.y , 1.0));
		r = n.r * n.g * n.b;
		r = 0.5 + 0.5 * sin(6.28318 * (r + _Time.y * glitchspeed));
		r = smoothstep(0.4, 0.6, r);
		r *= smoothstep(n.g - 0.001, n.g + 0.001, pscruv.y);
		r *= smoothstep(n.b + 0.001, n.b - 0.001, pscruv.y);
		float res = round(_GlitchResolution * 5.0);
		float3 cubepos = round(vpos.xyz * res) / res;
		vpos.xyz = lerp(vpos.xyz, cubepos, _Glitch * r);
		return vpos;
	}

	float4 aberrate(float4 vpos , fixed amount)
	{
		float3 wpos = mul(unity_ObjectToWorld, vpos).xyz;
		float3 dir = normalize(mul(unity_CameraToWorld, float4(1.0, 1.0, 0.0, 0.0)).xyz);
		dir = normalize(mul(unity_WorldToObject, float4(dir.xyz, 0.0)).xyz);
		vpos.xyz += dir * amount * 0.01 * _Aberration;
		return vpos;
	}

	fixed scanline(float2 scruv)
	{
		float y = scruv.y * _ScreenParams.y * 0.75;
		fixed s = smoothstep(0.5,1.0,2.0 * abs(0.5 - frac(y * 0.25)));
		return s * _ScanlineStrength + (1.0 - _ScanlineStrength);
	}

	float backfacedim(fixed facing)
	{
		return min(1.0,ceil(facing+0.1) + 0.5);
	}

	half4 col(v2f i, fixed3 mask)
	{
		float2 scruv = i.scrpos.xy / i.scrpos.w;
		float2 sqrscruv = scruv.xy * float2(_ScreenParams.x / _ScreenParams.y , 1.0);
		sqrscruv += float2(0.0, 1.0) * (_Time.x + _Glitch * 0.01 * noise(float2(0.5, _Time.y)));
		half4 c = 1.0;

		if (abs(dot(i.normal,float3(0.0,0.0,1.0)) )> 0.5)
			c *= tex2D(_MainTex, i.pos.xy * _MainTex_ST.xy + _MainTex_ST.zw);
		else
			c *= tex2D(_MainTex, i.pos.xz * _MainTex_ST.xy + _MainTex_ST.zw);

		c *= 1.0 - (tex2D(_ScreenTex, sqrscruv.xy * _ScreenTex_ST.xy + _ScreenTex_ST.zw) )* _ScreenTexWeight + (1.0 - _ScreenTexWeight);
		c *= _Color;
		c.rgb *= mask;
		c.rgb += i.Glitch * _Glitch * _GlitchColor.rgb;
		c.rgb *= scanline(scruv);

		float newdepth = i.depth.x;
		float olddepth = LinearEyeDepth(tex2D(_CameraDepthTexture, scruv).r);

		c.rgb *= saturate((abs(newdepth - olddepth)-0.025) * 8.0);

		return _Intensity * c;
	}
	ENDCG

	Properties
	{
		_Color("Color",Color) = (1.0,1.0,1.0,1.0)
		_Intensity("Intensity",Range(1.0,10.0)) = 2.0
		_MainTex ("Model Texture", 2D) = "white" {}
		_ScreenTex ("Screen Space Texture", 2D) = "white" {}
		_ScreenTexWeight("Screen Space Texture Weight",Range(0.0,1.0)) = 0.25
		_Noise ("Noise", 2D) = "gray" {}
		_Aberration("Aberration",Range(0.0,1.0)) = 0.5
		_Glitch("Glitch",Range(0.0,1.0)) = 0.5
		_GlitchSpeed("Glitch Speed",Float) = 0.5
		_GlitchResolution("Glitch Resolution" , Range(1.0,10.0)) = 5.0
		_GlitchColor("Shimmer Color",Color) = (1.0,1.0,1.0,1.0)
		_ScanlineStrength("Scanline Strength",Range(0.0,1.0)) = 0.5
	}
	SubShader
	{
		Tags { "Queue"="Transparent" }
		ZWrite Off
		Cull Off
		Blend One One

		Pass//red
		{
			CGPROGRAM
			v2f vert (appdata v)
			{
				v2f o;
#ifdef SHADER_API_D3D11
				//do instancing stuff
				UNITY_SETUP_INSTANCE_ID(v);
#endif
				v.vertex = Glitch(v.vertex, -1.0, o.Glitch);
				v.vertex = aberrate(v.vertex, -1.0);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = v.normal;
				o.uv = v.uv;
				o.scrpos = ComputeScreenPos(o.vertex);
				o.pos = v.vertex.xyz;
				COMPUTE_EYEDEPTH(o.depth);
				return o;
			}
			
			half4 frag(
				v2f i
#if (SHADER_TARGET > 45)
				, fixed facing : VFACE
#endif
			) : SV_Target
			{
				return col(i , fixed3(1.0,0.0,0.0))
				#if (SHADER_TARGET > 45)
				* backfacedim(facing)
				#endif
				;
			}
			ENDCG
		}

		Pass//blue
		{
			CGPROGRAM
			v2f vert(appdata v)
			{
				v2f o;
#ifdef SHADER_API_D3D11
				//do instancing stuff
				UNITY_SETUP_INSTANCE_ID(v);
#endif
				v.vertex = Glitch(v.vertex, 1.0, o.Glitch);
				v.vertex = aberrate(v.vertex, 1.0);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = v.normal;
				o.uv = v.uv;
				o.scrpos = ComputeScreenPos(o.vertex);
				o.pos = v.vertex.xyz;
				COMPUTE_EYEDEPTH(o.depth);
				return o;
			}

			half4 frag (
				v2f i
#if (SHADER_TARGET > 45)
				, fixed facing : VFACE
#endif
			) : SV_Target
			{
				return col(i , fixed3(0.0,0.0,1.0))
				#if (SHADER_TARGET > 45)
				* backfacedim(facing)
				#endif
				;
			}
			ENDCG
		}

		Pass//green
		{
			CGPROGRAM
			v2f vert (appdata v)
			{
				v2f o;
#ifdef SHADER_API_D3D11
				//do instancing stuff
				UNITY_SETUP_INSTANCE_ID(v);
#endif
				v.vertex = Glitch(v.vertex, -1.0, o.Glitch);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = v.normal;
				o.uv = v.uv;
				o.scrpos = ComputeScreenPos(o.vertex);
				o.pos = v.vertex.xyz;
				COMPUTE_EYEDEPTH(o.depth);
				return o;
			}
			
			half4 frag (
				v2f i
#if (SHADER_TARGET > 45)
				, fixed facing : VFACE
#endif
			) : SV_Target
			{
				return col(i , fixed3(0.0,1.0,0.0))
				#if (SHADER_TARGET > 45)
				* backfacedim(facing)
				#endif
				;
			}
			ENDCG
		}
	}
}
