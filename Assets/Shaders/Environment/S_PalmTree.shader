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
		_Framerate("Framerate", Float) = 5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float _Power;
		uniform float _Framerate;
		uniform float _Speed;
		uniform float _Strength;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float mulTime3 = _Time.y * _Framerate;
			v.vertex.xyz += ( pow( ase_vertex3Pos.y , _Power ) * sin( ( floor( mulTime3 ) * _Speed ) ) * _Strength * float3(1,0,0) );
		}

		void surf( Input i , inout SurfaceOutputStandard o )
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
-154;486;1906;587;1882.92;-188.2148;1;True;False
Node;AmplifyShaderEditor.RangedFloatNode;14;-1042.92,407.2148;Float;False;Property;_Framerate;Framerate;4;0;Create;True;0;0;False;0;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;3;-769.2825,469.0021;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FloorOpNode;11;-545.8463,468.0501;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-623.7076,591.9926;Float;False;Property;_Speed;Speed;3;0;Create;True;0;0;False;0;0;0.8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;6;-500.0128,161.3032;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;10;-477.0128,315.3032;Float;False;Property;_Power;Power;2;0;Create;True;0;0;False;0;0.2;2.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-390.1238,512.5422;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-225.0128,377.3032;Float;False;Property;_Strength;Strength;1;0;Create;True;0;0;False;0;0.2;0.001;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;8;-67.01282,475.3032;Float;False;Constant;_Vector0;Vector 0;2;0;Create;True;0;0;False;0;1,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SinOpNode;2;-369.3976,399.8421;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;9;-264.0128,248.3032;Float;False;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-448,-87.5;Float;True;Property;_MainTex;MainTex;0;0;Create;True;0;0;False;0;None;16aee854080cc6346b15a4d42e490188;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;21.98718,210.3032;Float;False;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;271,-26;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Palmtree;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;3;0;14;0
WireConnection;11;0;3;0
WireConnection;12;0;11;0
WireConnection;12;1;13;0
WireConnection;2;0;12;0
WireConnection;9;0;6;2
WireConnection;9;1;10;0
WireConnection;5;0;9;0
WireConnection;5;1;2;0
WireConnection;5;2;7;0
WireConnection;5;3;8;0
WireConnection;0;0;1;0
WireConnection;0;11;5;0
ASEEND*/
//CHKSM=55AF49F8BF5749715FEEF892B679DFD37B3B3366