// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Destructible"
{
	Properties
	{
		_Tint("Tint", Color) = (0,0,0,0)
		_Albedo("Albedo", 2D) = "white" {}
		_Alphamask("Alpha mask", 2D) = "white" {}
		_AlphaMask1("Alpha Mask 1", 2D) = "white" {}
		_AlphaMask2("Alpha Mask 2", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform float4 _Tint;
		uniform sampler2D _Alphamask;
		uniform float4 _Alphamask_ST;
		uniform sampler2D _AlphaMask1;
		uniform float4 _AlphaMask1_ST;
		uniform sampler2D _AlphaMask2;
		uniform float4 _AlphaMask2_ST;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float2 uv_Alphamask = i.uv_texcoord * _Alphamask_ST.xy + _Alphamask_ST.zw;
			float4 lerpResult3 = lerp( tex2D( _Albedo, uv_Albedo ) , _Tint , tex2D( _Alphamask, uv_Alphamask ).a);
			float2 uv_AlphaMask1 = i.uv_texcoord * _AlphaMask1_ST.xy + _AlphaMask1_ST.zw;
			float4 lerpResult6 = lerp( lerpResult3 , _Tint , tex2D( _AlphaMask1, uv_AlphaMask1 ).a);
			float2 uv_AlphaMask2 = i.uv_texcoord * _AlphaMask2_ST.xy + _AlphaMask2_ST.zw;
			float4 lerpResult7 = lerp( lerpResult6 , _Tint , tex2D( _AlphaMask2, uv_AlphaMask2 ).a);
			o.Albedo = lerpResult7.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16100
7;1;1266;954;2031.741;660.0797;1.668738;True;False
Node;AmplifyShaderEditor.SamplerNode;2;-1314.581,-408.5722;Float;True;Property;_Albedo;Albedo;1;0;Create;True;0;0;False;0;None;e639b1249d489c24687ec863f866a03c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-1305.803,-15.9306;Float;True;Property;_Alphamask;Alpha mask;2;0;Create;True;0;0;False;0;None;f2988bafebaa7924d8acb3d759190cf8;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;8;-1511.246,-214.1135;Float;False;Property;_Tint;Tint;0;0;Create;True;0;0;False;0;0,0,0,0;0,0.6861651,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;3;-948.5814,-272.5722;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;4;-1069.001,219.5083;Float;True;Property;_AlphaMask1;Alpha Mask 1;3;0;Create;True;0;0;False;0;None;1e5136f3839ea234dbaac75cf7e9a7a7;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;6;-679.9272,-13.28967;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;5;-1069.001,464.5083;Float;True;Property;_AlphaMask2;Alpha Mask 2;4;0;Create;True;0;0;False;0;None;c49a79eb0b6ec0246b877c54c070806b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;7;-445.9272,209.7103;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Destructible;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;3;0;2;0
WireConnection;3;1;8;0
WireConnection;3;2;1;4
WireConnection;6;0;3;0
WireConnection;6;1;8;0
WireConnection;6;2;4;4
WireConnection;7;0;6;0
WireConnection;7;1;8;0
WireConnection;7;2;5;4
WireConnection;0;0;7;0
ASEEND*/
//CHKSM=5C41D01E9FD023CE643EC5E2E49FB92B65298CDE