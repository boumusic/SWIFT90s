Shader "Hidden/Custom/Chromab"
{
	HLSLINCLUDE

	#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

	float4 _ScreenSize;
	float _ROffset;
	float _GOffset;
	float _BOffset;

	TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);

	float4 Frag(VaryingsDefault i) : SV_Target
	{
		float2 coordsR = i.texcoord + _ROffset;
		float2 coordsG = i.texcoord + _GOffset;
		float2 coordsB = i.texcoord + _BOffset;

		float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
		float4 red = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, coordsR);
		float4 green = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, coordsG);
		float4 blue = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, coordsB);
		float4 chromab = float4(red.r, green.g, blue.b, 1);
		return chromab;
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