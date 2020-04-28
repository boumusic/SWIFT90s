Shader "Hidden/Custom/SineDistort"
{
	HLSLINCLUDE

	#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

	float4 _ScreenSize;
	float _Distort;
	float _Tiling;
	float _Scroll;
	float4 _Color;

	TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);

	float4 Frag(VaryingsDefault i) : SV_Target
	{
		float sine = sin(i.texcoord.y * _Tiling + _Time.y * _Scroll);
		float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord + sine * _Distort) + saturate(sine + 0.5) * _Color;
		return color;
	}

		ENDHLSL

		SubShader
	{
		Cull Off ZWrite Off ZTest Always

			Pass
		{
			HLSLPROGRAM

			#pragma vertex VertDefault
			#pragma fragment Frag

			ENDHLSL
		}
	}
}