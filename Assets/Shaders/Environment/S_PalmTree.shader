// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Palmtree"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_Strength("Strength", Float) = 0.2
		_Power("Power", Float) = 0.2
		_Speed("Speed", Float) = 0
		_FramerateLeaf("FramerateLeaf", Float) = 0
		_Framerate("Framerate", Float) = 5
		_Noise("Noise", 2D) = "white" {}
		_LeafStrength("LeafStrength", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Lambert keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _Power;
		uniform float _Framerate;
		uniform float _Speed;
		uniform float _Strength;
		uniform sampler2D _Noise;
		uniform sampler2D _Sampler018;
		uniform float4 _Noise_ST;
		uniform float _FramerateLeaf;
		uniform float _LeafStrength;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float mulTime3 = _Time.y * _Framerate;
			float4 transform37 = mul(unity_WorldToObject,float4( 0,0,0,1 ));
			float temp_output_38_0 = ( transform37.x + transform37.y + transform37.z );
			float4 appendResult35 = (float4(ase_vertex3Pos.x , ase_vertex3Pos.z , 0.0 , 0.0));
			float mulTime24 = _Time.y * _FramerateLeaf;
			v.vertex.xyz += ( ( pow( ase_vertex3Pos.y , _Power ) * sin( ( ( floor( mulTime3 ) * _Speed ) + ( temp_output_38_0 * 3.67 ) ) ) * _Strength * float3(1,0,0) ) + ( float3(0,1,0) * ( tex2Dlod( _Noise, float4( ( ( appendResult35 * float4( _Noise_ST.xy, 0.0 , 0.0 ) ) + float4( ( floor( mulTime24 ) * _Noise_ST.zw ), 0.0 , 0.0 ) + temp_output_38_0 ).xy, 0, 0.0) ).r - 0.5 ) * _LeafStrength * ( 1.0 - v.texcoord.xy.x ) ) );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			o.Albedo = tex2D( _MainTex, uv_MainTex ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15900
98;454;1323;561;2762.538;37.24656;1.629896;True;False
Node;AmplifyShaderEditor.RangedFloatNode;26;-946.1063,920.5396;Float;False;Property;_FramerateLeaf;FramerateLeaf;4;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;15;-1620.539,-81.48065;Float;False;1276.907;596.6894;Trunk;15;14;3;11;13;10;12;6;7;8;2;9;5;38;39;40;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleTimeNode;24;-672.4684,982.3268;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;34;-624.9899,572.3914;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;14;-1593.169,185.3345;Float;False;Property;_Framerate;Framerate;5;0;Create;True;0;0;False;0;5;7;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;35;-365.6105,601.3785;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FloorOpNode;25;-449.0323,981.3749;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureTransformNode;18;-777.324,748.0576;Float;False;17;1;0;SAMPLER2D;_Sampler018;False;2;FLOAT2;0;FLOAT2;1
Node;AmplifyShaderEditor.WorldToObjectTransfNode;37;-2139.659,415.0588;Float;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;3;-1418.135,177.6139;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-221.0665,672.8384;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-1500.483,334.5503;Float;False;Property;_Speed;Speed;3;0;Create;True;0;0;False;0;0;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;38;-1384.771,402.127;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-290.3154,953.9949;Float;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FloorOpNode;11;-1251.612,183.0741;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-1098.478,267.6553;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;21;-123.7494,778.3534;Float;False;3;3;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;-1202.727,422.3842;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;3.67;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;17;13.58987,760.7169;Float;True;Property;_Noise;Noise;6;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;39;-951.1831,345.994;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-1004.632,122.5193;Float;False;Property;_Power;Power;2;0;Create;True;0;0;False;0;0.2;2.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;6;-1027.632,-31.48064;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;32;194.5254,923.268;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SinOpNode;2;-854.6394,303.222;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;29;319.8961,746.1105;Float;False;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;27;108.6961,557.3102;Float;False;Constant;_Vector2;Vector 2;6;0;Create;True;0;0;False;0;0,1,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;7;-752.6318,184.5193;Float;False;Property;_Strength;Strength;1;0;Create;True;0;0;False;0;0.2;0.001;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;33;479.2048,912.6753;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;9;-791.6317,55.51936;Float;False;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;30;291.096,477.3102;Float;False;Property;_LeafStrength;LeafStrength;7;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;8;-594.6318,282.5193;Float;False;Constant;_Vector0;Vector 0;2;0;Create;True;0;0;False;0;1,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-505.6318,17.51936;Float;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;505.496,658.1102;Float;False;4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;16;665.6102,293.5667;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;1;290.1592,-168.6722;Float;True;Property;_MainTex;MainTex;0;0;Create;True;0;0;False;0;None;16aee854080cc6346b15a4d42e490188;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;869.7371,-25.98016;Float;False;True;2;Float;ASEMaterialInspector;0;0;Lambert;Palmtree;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;24;0;26;0
WireConnection;35;0;34;1
WireConnection;35;1;34;3
WireConnection;25;0;24;0
WireConnection;3;0;14;0
WireConnection;36;0;35;0
WireConnection;36;1;18;0
WireConnection;38;0;37;1
WireConnection;38;1;37;2
WireConnection;38;2;37;3
WireConnection;23;0;25;0
WireConnection;23;1;18;1
WireConnection;11;0;3;0
WireConnection;12;0;11;0
WireConnection;12;1;13;0
WireConnection;21;0;36;0
WireConnection;21;1;23;0
WireConnection;21;2;38;0
WireConnection;40;0;38;0
WireConnection;17;1;21;0
WireConnection;39;0;12;0
WireConnection;39;1;40;0
WireConnection;2;0;39;0
WireConnection;29;0;17;1
WireConnection;33;0;32;1
WireConnection;9;0;6;2
WireConnection;9;1;10;0
WireConnection;5;0;9;0
WireConnection;5;1;2;0
WireConnection;5;2;7;0
WireConnection;5;3;8;0
WireConnection;28;0;27;0
WireConnection;28;1;29;0
WireConnection;28;2;30;0
WireConnection;28;3;33;0
WireConnection;16;0;5;0
WireConnection;16;1;28;0
WireConnection;0;0;1;0
WireConnection;0;11;16;0
ASEEND*/
//CHKSM=C81A66064446C5F208E342E469A1F4273E882633