// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Flag"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.98
		_Color("Color", Color) = (1,1,1,0)
		_Speed("Speed", Float) = 2
		_Strength("Strength", Float) = 2
		_Freq("Freq", Float) = 5
		_Emissive("Emissive", Float) = 0
		_Framerate("Framerate", Float) = 0
		_Mask("Mask", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _Speed;
		uniform float _Framerate;
		uniform float _Freq;
		uniform float _Strength;
		uniform float4 _Color;
		uniform float _Emissive;
		uniform sampler2D _Mask;
		uniform sampler2D _Sampler024;
		uniform float4 _Mask_ST;
		uniform float _Cutoff = 0.98;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float mulTime4 = _Time.y * _Framerate;
			float temp_output_30_0 = ( 1.0 - v.texcoord.xy.x );
			v.vertex.xyz += ( sin( ( ( _Speed * floor( mulTime4 ) ) + ( _Freq * temp_output_30_0 ) ) ) * float3(0,0.5,1) * _Strength * temp_output_30_0 );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Albedo = ( _Color * _Emissive ).rgb;
			o.Alpha = 1;
			float2 temp_output_28_0 = ( floor( _Time.y ) * _Mask_ST.zw );
			float2 uv_TexCoord23 = i.uv_texcoord * _Mask_ST.xy + temp_output_28_0;
			clip( tex2D( _Mask, ( uv_TexCoord23 + temp_output_28_0 ) ).r - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15900
14;426;1906;593;1294.525;-601.1011;1.33209;True;False
Node;AmplifyShaderEditor.RangedFloatNode;21;-792.6058,1141.351;Float;False;Property;_Framerate;Framerate;6;0;Create;True;0;0;False;0;0;7.65;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;4;-615.0369,891.3568;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;27;-596.6431,212.9711;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;8;-475.3429,1202.623;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FloorOpNode;29;-331.9924,280.5977;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureTransformNode;24;-481.996,0.3639039;Float;False;22;1;0;SAMPLER2D;_Sampler024;False;2;FLOAT2;0;FLOAT2;1
Node;AmplifyShaderEditor.OneMinusNode;30;-146.038,1029.163;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-576.6957,657.6516;Float;False;Property;_Speed;Speed;2;0;Create;True;0;0;False;0;2;6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FloorOpNode;19;-428.5269,768.149;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-410.489,921.3788;Float;False;Property;_Freq;Freq;4;0;Create;True;0;0;False;0;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-234.4836,827.8281;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;-244.8476,659.096;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-171.3911,208.4114;Float;False;2;2;0;FLOAT;0;False;1;FLOAT2;2,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;7;21.52995,745.0081;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;23;-139.7247,-16.67522;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector3Node;13;200.4446,1121.985;Float;False;Constant;_Vector0;Vector 0;3;0;Create;True;0;0;False;0;0,0.5,1;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;18;64.81313,-207.2382;Float;False;Property;_Emissive;Emissive;5;0;Create;True;0;0;False;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;26;147.4695,131.4729;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;11;288.4999,621.1876;Float;False;Property;_Strength;Strength;3;0;Create;True;0;0;False;0;2;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinOpNode;14;150.0636,806.7164;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1;-72.42175,-351.7595;Float;False;Property;_Color;Color;1;0;Create;True;0;0;False;0;1,1,1,0;0.7547169,0.006578746,0,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NormalVertexDataNode;9;157.3668,1015.306;Float;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;17;283.7292,-195.6319;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;10;526.2449,698.1382;Float;False;4;4;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;22;482.9257,113.9633;Float;True;Property;_Mask;Mask;7;0;Create;True;0;0;False;0;None;44f5c1b9ca3453641a4a11f2cc1d7afc;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;805.305,-80.41805;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Flag;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.98;True;True;0;False;TransparentCutout;;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;4;0;21;0
WireConnection;29;0;27;0
WireConnection;30;0;8;1
WireConnection;19;0;4;0
WireConnection;15;0;16;0
WireConnection;15;1;30;0
WireConnection;20;0;5;0
WireConnection;20;1;19;0
WireConnection;28;0;29;0
WireConnection;28;1;24;1
WireConnection;7;0;20;0
WireConnection;7;1;15;0
WireConnection;23;0;24;0
WireConnection;23;1;28;0
WireConnection;26;0;23;0
WireConnection;26;1;28;0
WireConnection;14;0;7;0
WireConnection;17;0;1;0
WireConnection;17;1;18;0
WireConnection;10;0;14;0
WireConnection;10;1;13;0
WireConnection;10;2;11;0
WireConnection;10;3;30;0
WireConnection;22;1;26;0
WireConnection;0;0;17;0
WireConnection;0;10;22;1
WireConnection;0;11;10;0
ASEEND*/
//CHKSM=38997B5AA34BC6B492D51AF8203B9081BD33BCC0