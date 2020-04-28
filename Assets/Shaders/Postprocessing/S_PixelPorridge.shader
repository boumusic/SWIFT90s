Shader "Hidden/Custom/PixelPorridge"
{
	HLSLINCLUDE

	#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

	float _Res;
	float4 _ScreenSize;
	//TEXTURE2D(_GrungeTex);
	TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);

	float4 Frag(VaryingsDefault i) : SV_Target
	{
		float2 coords = i.texcoord * _Res * float2(_ScreenSize.x / 1000, _ScreenSize.y / 1000);
		float2 floored = floor(coords) / _Res / float2(_ScreenSize.x / 1000, _ScreenSize.y / 1000);
		float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, floored);
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