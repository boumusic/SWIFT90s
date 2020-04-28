
Shader "QuikFX/VFX"
{
    Properties
    {
		[Header(Textures)]
        [NoScaleOffset] _MainTex ("Main Texture", 2D) = "white" {}
		[PackedVectorDrawer(Tiling, ScrollingSpeed)] _TilingSpeedA("Tiling (XY) - Scrolling Speed (ZW)", Vector) = (1, 1, 0, 0)
		_SpeedMultiplier("Speed Multiplier", Range(0, 10)) = 1
		_FresnelColorPower("Fresnel Color Power", Range(0, 10)) = 0
		_FresnelColor ("Fresnel Color", Color) = (1,1,1,1)

		[Toggle(USE_SECONDARY_TEX)] _UseSecondaryTex ("Use Secondary Texture", Float) = 0
		_BlendFactor ("Blend Factor", Range(0,1)) = 0.5
        [NoScaleOffset] _SecondaryTex ("Secondary Texture", 2D) = "white" {}
		[PackedVectorDrawer(Tiling, ScrollingSpeed)] _TilingSpeedB("Tiling (XY) - Scrolling Speed (ZW)", Vector) = (1, 1, 0, 0)

		[Toggle(USE_GRADIENT_MAP)] _UseGradientMap ("Use Gradient Map", Float) = 0
		[NoScaleOffset] _GradientTex("Gradient Texture", 2D) = "white"{}
		[Toggle] _FlipGradientMap ("Flip Gradient Map", Float) = 0
		[MinMaxSliderDrawer(0, 1, Remap)] _GradientRemap("Gradient Remap", Vector) = (0, 1, 0, 0)
		_ScrollingSpeedGrad("Scrolling speed Gradient", Range(-5, 5)) = 0

		[Toggle] _UseCustomData("Use Custom Data", float) = 0

		_Posterize ("Posterize", Range(0, 20)) = 0
		_Pixellise ("Pixellise", Range(0, 100)) = 0

		[Toggle(USE_DISTORTION)] _UseDistortion ("Use Distortion", Float) = 0
		[NoScaleOffset] _DistortionTex("Distortion Texture", 2D) = "white" {}
		_DistortionStrengthX("Distortion Strength X", Range(0, 1)) = 0
		_DistortionStrengthY("Distortion Strength Y", Range(0, 1)) = 0
		[PackedVectorDrawer(TilingDistortion, ScrollingSpeedDistortion)] _TilingSpeedDistortion("Tiling (XY) - Scrolling Speed (ZW)", Vector) = (1, 1, 0, 0)

		[Toggle(USE_DISPLACEMENT)] _UseDisplacement ("Use Displacement", Float) = 0
		[NoScaleOffset] _DisplacementTex("Displacement Texture", 2D) = "white" {}
		[PackedVectorDrawer(TilingDisplacement, ScrollingSpeedDisplacement)] _TilingSpeedDisplacement("Tiling (XY) - Scrolling Speed (ZW)", Vector) = (1, 1, 0, 0)
		_DisplacementStrength("Displacement Strength", Range(-10,10)) = 0


		[Toggle] _UseEmissive ("Use Emissive", Float) = 0
		[MinMaxSliderDrawer(0, 1, 0, MinMax, 0, 1, 1, Remap)] _Emissive("Emissive : X = Min, Y = Max, Z = Noise Remap Low, W = Noise Remap High", Vector) = (1, 1, 0, 1)
		_EmissiveColor("Emissive Color", Color) = (1,1,1,1)

		[Toggle] _UseEdgeBurn ("Use Edge Burn", Float) = 0
		[Toggle] _FlipEdgeBurn ("Flip Edge Burn", Float) = 0
		_EdgeBurnThreshold("Edge Burn Threshold", Range(-.5, .5)) = 0
		_EdgeBurnEmissive("Edge Burn Emissive", Range(0, 250)) = 2
		_EdgeBurnSharpness("Edge Burn Sharpness", Range(0,1)) = 0.9
		_EdgeBurnColor("Edge Burn Color", Color) = (1,1,1,1)

		[Toggle] _UseAlpha ("Use Alpha", Float) = 0
		_AlphaSource("Alpha Source (MainTex / SecondaryTex)", Range(0,1)) = 0
		_Erosion("Alpha Erosion", Range(0,1)) = 0
		_Sharpness("Alpha Sharpness", Range(0,1)) = 0.5
			
		[PackedVectorDrawer(ThresholdU, SharpnessU)] _EdgeFadeU("Edge Fade U : X = U threshold top, Y = U threshold bottom, Z = U sharpness top, W = V sharpness bottom", Vector) = (0, 1, 1, 1)
		[PackedVectorDrawer(ThresholdV, SharpnessV)]  _EdgeFadeV("Edge Fade V : X = V threshold top, Y = U threshold bottom, Z = V sharpness top, W = V sharpness bottom", Vector) = (0, 1, 1, 1)
		_FresnelAlphaPower("Fresnel Alpha Power", Range(-20, 20)) = 0

		_OffsetX ("Offset U", Range(-1,1)) = 0
		_OffsetY ("Offset V", Range(-1,1)) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		Cull Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			#pragma shader_feature_local USE_SECONDARY_TEX
			#pragma shader_feature_local USE_GRADIENT_MAP
			#pragma shader_feature_local USE_DISTORTION
			#pragma shader_feature_local USE_DISPLACEMENT

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
				float3 normal : NORMAL;
				float4 customData : TEXCOORD1;
				float4 color : COLOR;
				float3 viewDir : TEXCOORD2;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 uv : TEXCOORD0;
				float4 customData : TEXCOORD1;
				float4 color : COLOR;
				float3 normal : NORMAL;
				float3 viewDir : TEXCOORD2;
            };

            sampler2D _MainTex;
            sampler2D _SecondaryTex;
            sampler2D _GradientTex;
            sampler2D _DistortionTex;
            sampler2D _DisplacementTex;
            float4 _MainTex_ST;

			float4 _TilingSpeedA;
			float4 _TilingSpeedB;
			float4 _TilingSpeedDistortion;
			float4 _TilingSpeedDisplacement;
			float4 _GradientRemap;

			float4 _EdgeFadeU;
			float4 _EdgeFadeV;
			float4 _Emissive;
			fixed4 _EdgeBurnColor;
			fixed4 _FresnelColor;

			float _BlendFactor;
			float _ScrollingSpeedGrad;
			float _Posterize;
			float _Pixellise;
			float _Erosion;
			float _Sharpness;
			float _EdgeBurnThreshold;
			float _EdgeBurnEmissive;
			float _EdgeBurnSharpness;
			float _DistortionStrengthX;
			float _DistortionStrengthY;
			float _DisplacementStrength;
			float _SpeedMultiplier;
			float _FresnelAlphaPower;
			float _FresnelColorPower;

			float _UseSecondaryTex;
			float _UseGradientMap;
			float _UseDistortion;
			float _UseCustomData;
			float _FlipGradientMap;
			float _UseEmissive;
			float _UseAlpha;
			float _UseEdgeBurn;
			float _FlipEdgeBurn;
			float _UseDisplacement;
			float _AlphaSource;
			float _OffsetX;
			float _OffsetY;
			float4 _Reveil;

            v2f vert (appdata v)
            {
                v2f o;
                o.uv.xy = TRANSFORM_TEX(v.uv.xy, _MainTex);
				o.uv.zw = v.uv.zw;

				#if USE_DISPLACEMENT
					float speedMul = _SpeedMultiplier + (v.customData.y * _UseCustomData);
					float2 uvDisp = o.uv * _TilingSpeedDisplacement.xy + _Time.y * _TilingSpeedDisplacement.zw * speedMul;
					float2 uvDispB = o.uv * _TilingSpeedDisplacement.xy * 0.5 + _Time.y * _TilingSpeedDisplacement.zw * speedMul * 0.5;
					float4 displacement = tex2Dlod(_DisplacementTex, float4(uvDisp, 0, 0));
					float4 displacementB = tex2Dlod(_DisplacementTex, float4(uvDispB, 0, 0));

					float lerpDisp = lerp(displacement, displacementB, 0.5);

					float dispStrength = (lerpDisp - 0.5) * _DisplacementStrength;
					v.vertex.xyz += v.normal * dispStrength; 
				#endif

				o.viewDir = WorldSpaceViewDir(v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.customData = v.customData;
				o.color = v.color;
				o.normal = v.normal;
				return o;
            }
			
			float fresneleffect(float3 Normal, float3 ViewDir, float Power)
			{
				float3 normNormal = normalize(Normal);
				float3 normViewDir = normalize(ViewDir);
				float dotProduct = dot(normNormal, normViewDir);
				float remappedDot = (dotProduct + 1) / 2;
				float powered = pow(remappedDot, Power);
				return powered;
			}

			float4 posterize(float4 input, float steps)
			{
				return floor(input * steps) / steps;
			}

            fixed4 frag (v2f i) : SV_Target
            {		
				float2 distort = float2(0,0);
				float speedMul = _SpeedMultiplier;
				float offsetX = _OffsetX + (i.uv.w - 0.5) * _UseCustomData;
				float offsetY = _OffsetY + (i.customData.x - 0.2) * _UseCustomData;
				float usingPosterize = saturate(ceil(_Posterize));
				float usingPixellise = saturate(ceil(_Pixellise));
				float2 pixelUv = lerp(i.uv.xy, posterize(float4(i.uv.xy, 0, 0), _Pixellise + 1).xy, usingPixellise);
				float4 secondaryTex = float4(1,1,1,1);

				#if USE_DISTORTION
					float2 uvDistortion =  i.uv * _TilingSpeedDistortion.xy + _Time.y * _TilingSpeedDistortion.zw * speedMul;
					float4 distortionTex = tex2D(_DistortionTex, uvDistortion + _Reveil);
					distort = float2((distortionTex.x - 0.5) * _DistortionStrengthX, (distortionTex.y - 0.5) * _DistortionStrengthY);
				#endif

				float2 uvA = (pixelUv * _TilingSpeedA.xy + _Time.y * _TilingSpeedA.zw * speedMul) + distort + _Reveil;
				float4 sampleMain = tex2D(_MainTex, uvA);
				float4 posterizedMain = lerp(sampleMain, posterize(sampleMain, _Posterize + 1), usingPosterize);
				float4 mainTex = lerp(posterizedMain, sampleMain, _UseSecondaryTex);
				float4 lerpedTex = mainTex;
				float4 grad = float4(0, 0, 0, 1);
				
				float4 col = mainTex;

				#if USE_SECONDARY_TEX
					float2 uvB = (pixelUv * _TilingSpeedB.xy + _Time.y * _TilingSpeedB.zw * speedMul) + distort + _Reveil;
					secondaryTex = tex2D(_SecondaryTex, uvB);
					lerpedTex = lerp(mainTex, secondaryTex, _BlendFactor);
					posterizedMain = lerp(lerpedTex, posterize(lerpedTex, _Posterize + 1), usingPosterize);
					col = lerp(lerpedTex, posterizedMain, usingPosterize);
				#endif

				#if USE_GRADIENT_MAP
					float remappedlerpedTexGradientMap = smoothstep(_GradientRemap.x, _GradientRemap.y, posterizedMain);
					float invertGrad = abs(_FlipGradientMap - remappedlerpedTexGradientMap);
					grad = tex2D(_GradientTex, invertGrad + _ScrollingSpeedGrad * _Time.y);
					col = grad;
				#endif
				
				float fadeTopU = smoothstep(_EdgeFadeU.x, _EdgeFadeU.x + (1 - _EdgeFadeU.z) , i.uv.x - offsetX);
				float fadeBottomU = smoothstep(1 - _EdgeFadeU.y, (1 - _EdgeFadeU.y) + (1 - _EdgeFadeU.w) , 1 - i.uv.x + offsetX);

				float fadeTopV = smoothstep((1 - _EdgeFadeV.x), (1 - _EdgeFadeV.x) - (1 - _EdgeFadeV.z), i.uv.y - offsetY);
				float fadeBottomV = smoothstep(_EdgeFadeV.y, _EdgeFadeV.y - (1 - _EdgeFadeV.w), 1 - i.uv.y + offsetY);
				float edgeFade = fadeBottomU * fadeTopU * fadeTopV * fadeBottomV;

				float alphaFresnel = fresneleffect(i.normal, i.viewDir, _FresnelAlphaPower);

				float evaluateSource = lerp(0, _AlphaSource, _UseSecondaryTex);
				float4 alphaSource = lerp(mainTex, secondaryTex, evaluateSource);

				float erosion = _Erosion + (i.uv.z * _UseCustomData);

				float erodedAlpha = (alphaSource - (erosion - 0.5) * 2) * edgeFade * alphaFresnel;
				float smoothstepAlpha = smoothstep(_Sharpness / 2, 1 - _Sharpness / 2, erodedAlpha);

				float finalAlpha = saturate(smoothstepAlpha) * mainTex.a * secondaryTex.a;
				col.a = lerp(1, finalAlpha, _UseAlpha) * i.color.a;				
				
				float remappedNoiseEmissive = smoothstep(_Emissive.z, _Emissive.w, lerpedTex);
				float emissive = lerp(_Emissive.x, _Emissive.y, remappedNoiseEmissive) + i.customData.y;	

				float colorFresnel =  fresneleffect(i.normal, i.viewDir, _FresnelColorPower);

				float erodedBurn = abs(_FlipEdgeBurn - alphaSource) - ((_EdgeBurnThreshold + erosion) - 0.5) * 2 * edgeFade;
				float smoothstepBurn = smoothstep(_EdgeBurnSharpness / 2, 1 - _EdgeBurnSharpness / 2, 1 - erodedBurn);
				float4 burnEmissive = smoothstepBurn * _EdgeBurnEmissive * _EdgeBurnColor;

				float3 finalEmissive = lerp(col.rgb * emissive, col.rgb + burnEmissive, smoothstepBurn * _UseEdgeBurn);
				float3 useEmissive = lerp(col.rgb, finalEmissive, _UseEmissive);
				col.rgb = useEmissive * i.color.rgb;

                return col;
            }
            ENDCG
        }
    }

	CustomEditor "Pataya.QuikFX.VFXShaderGUI"
}

