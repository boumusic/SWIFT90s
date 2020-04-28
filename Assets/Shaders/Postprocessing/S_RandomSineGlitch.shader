Shader "Hidden/Custom/RandomGlitch"
{
	HLSLINCLUDE

	#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

	float4 _ScreenSize;
	float _Distort;
	float _Tiling;
	float _Scroll;
	float _Speed;

	TEXTURE2D_SAMPLER2D(_NoiseTex, sampler_NoiseTex);
	TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);

	float4 Frag(VaryingsDefault i) : SV_Target
	{
		float4 noise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, _Time.y * _Speed);
		float4 intensity = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, 0.7 * floor(_Time.y * 0.7) + i.texcoord);
		float sine = sin(i.texcoord.y * _Tiling + _Time.y * _Scroll);
		float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord + sine * _Distort * noise.r * intensity);
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