// Made with Amplify Shader Editor v1.9.6.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Cainos/Pixel Art Platformer - Dungeon/Sprite Shadow"
{
	Properties
	{
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[HideInInspector] _EmissionColor("Emission Color", Color) = (1,1,1,1)
		_Color("Color", Color) = (0.3018868,0.1945689,0.1666074,0.5019608)
		[NoScaleOffset]_MainTex("MainTex", 2D) = "white" {}
		_StencilReference("Stencil Reference", Float) = 33
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

		[HideInInspector][NoScaleOffset] unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset] unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
	}

	SubShader
	{
		LOD 0

		

		Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" }

		Cull Off
		HLSLINCLUDE
		#pragma target 2.0
		#pragma prefer_hlslcc gles
		// ensure rendering platforms toggle list is visible

		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
		#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"
		ENDHLSL

		
		Pass
		{
			Name "Sprite Unlit"
			Tags { "LightMode"="Universal2D" }

			Blend DstColor Zero, One OneMinusSrcAlpha
			ZTest LEqual
			ZWrite Off
			Offset 0 , 0
			ColorMask RGBA
			Stencil
			{
				Ref [_StencilReference]
				Comp NotEqual
				Pass Replace
				Fail Keep
				ZFail Keep
			}

			HLSLPROGRAM

			#define ASE_SRP_VERSION 120108


			#pragma vertex vert
			#pragma fragment frag

			#define _SURFACE_TYPE_TRANSPARENT 1
			#define SHADERPASS SHADERPASS_SPRITEUNLIT

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/SurfaceData2D.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging2D.hlsl"

			

			sampler2D _MainTex;
			CBUFFER_START( UnityPerMaterial )
			float4 _Color;
			float _StencilReference;
			CBUFFER_END


			struct VertexInput
			{
				float4 positionOS : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 uv0 : TEXCOORD0;
				float4 color : COLOR;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				float4 texCoord0 : TEXCOORD0;
				float4 color : TEXCOORD1;
				float3 positionWS : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			#if ETC1_EXTERNAL_ALPHA
				TEXTURE2D( _AlphaTex ); SAMPLER( sampler_AlphaTex );
				float _EnableAlphaTexture;
			#endif

			float4 _RendererColor;

			
			VertexOutput vert( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.positionOS.xyz = vertexValue;
				#else
					v.positionOS.xyz += vertexValue;
				#endif
				v.normal = v.normal;
				v.tangent.xyz = v.tangent.xyz;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( v.positionOS.xyz );

				o.texCoord0 = v.uv0;
				o.color = v.color;
				o.positionCS = vertexInput.positionCS;
				o.positionWS = vertexInput.positionWS;

				return o;
			}

			half4 frag( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				float4 break10 = _Color;
				float2 uv_MainTex9 = IN.texCoord0.xy;
				float temp_output_12_0 = ( break10.a * tex2D( _MainTex, uv_MainTex9 ).a );
				float4 appendResult11 = (float4(break10.r , break10.g , break10.b , temp_output_12_0));
				clip( temp_output_12_0 - 0.1);
				
				float4 Color = appendResult11;

				#if ETC1_EXTERNAL_ALPHA
					float4 alpha = SAMPLE_TEXTURE2D( _AlphaTex, sampler_AlphaTex, IN.texCoord0.xy );
					Color.a = lerp( Color.a, alpha.r, _EnableAlphaTexture );
				#endif

				#if defined(DEBUG_DISPLAY)
				SurfaceData2D surfaceData;
				InitializeSurfaceData(Color.rgb, Color.a, surfaceData);
				InputData2D inputData;
				InitializeInputData(IN.positionWS.xy, half2(IN.texCoord0.xy), inputData);
				half4 debugColor = 0;

				SETUP_DEBUG_DATA_2D(inputData, IN.positionWS);

				if (CanDebugOverrideOutputColor(surfaceData, inputData, debugColor))
				{
					return debugColor;
				}
				#endif

				Color *= IN.color * _RendererColor;
				return Color;
			}

			ENDHLSL
		}
		
		Pass
		{
			
			Name "Sprite Unlit Forward"
            Tags { "LightMode"="UniversalForward" }

			Blend DstColor Zero, One OneMinusSrcAlpha
			ZTest LEqual
			ZWrite Off
			Offset 0 , 0
			ColorMask RGBA
			Stencil
			{
				Ref [_StencilReference]
				Comp NotEqual
				Pass Replace
				Fail Keep
				ZFail Keep
			}

			HLSLPROGRAM

			#define ASE_SRP_VERSION 120108


			#pragma vertex vert
			#pragma fragment frag

			#define _SURFACE_TYPE_TRANSPARENT 1
			#define SHADERPASS SHADERPASS_SPRITEFORWARD

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			#include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/SurfaceData2D.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/Debugging2D.hlsl"

			

			sampler2D _MainTex;
			CBUFFER_START( UnityPerMaterial )
			float4 _Color;
			float _StencilReference;
			CBUFFER_END


			struct VertexInput
			{
				float4 positionOS : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 uv0 : TEXCOORD0;
				float4 color : COLOR;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				float4 texCoord0 : TEXCOORD0;
				float4 color : TEXCOORD1;
				float3 positionWS : TEXCOORD2;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			#if ETC1_EXTERNAL_ALPHA
				TEXTURE2D( _AlphaTex ); SAMPLER( sampler_AlphaTex );
				float _EnableAlphaTexture;
			#endif

			float4 _RendererColor;

			
			VertexOutput vert( VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.positionOS.xyz = vertexValue;
				#else
					v.positionOS.xyz += vertexValue;
				#endif
				v.normal = v.normal;
				v.tangent.xyz = v.tangent.xyz;

				VertexPositionInputs vertexInput = GetVertexPositionInputs( v.positionOS.xyz );

				o.texCoord0 = v.uv0;
				o.color = v.color;
				o.positionCS = vertexInput.positionCS;
				o.positionWS = vertexInput.positionWS;

				return o;
			}

			half4 frag( VertexOutput IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				float4 break10 = _Color;
				float2 uv_MainTex9 = IN.texCoord0.xy;
				float temp_output_12_0 = ( break10.a * tex2D( _MainTex, uv_MainTex9 ).a );
				float4 appendResult11 = (float4(break10.r , break10.g , break10.b , temp_output_12_0));
				clip( temp_output_12_0 - 0.1);
				
				float4 Color = appendResult11;

				#if ETC1_EXTERNAL_ALPHA
					float4 alpha = SAMPLE_TEXTURE2D( _AlphaTex, sampler_AlphaTex, IN.texCoord0.xy );
					Color.a = lerp( Color.a, alpha.r, _EnableAlphaTexture );
				#endif


				#if defined(DEBUG_DISPLAY)
				SurfaceData2D surfaceData;
				InitializeSurfaceData(Color.rgb, Color.a, surfaceData);
				InputData2D inputData;
				InitializeInputData(IN.positionWS.xy, half2(IN.texCoord0.xy), inputData);
				half4 debugColor = 0;

				SETUP_DEBUG_DATA_2D(inputData, IN.positionWS);

				if (CanDebugOverrideOutputColor(surfaceData, inputData, debugColor))
				{
					return debugColor;
				}
				#endif

				Color *= IN.color * _RendererColor;
				return Color;
			}

			ENDHLSL
		}
		
        Pass
        {
			
            Name "SceneSelectionPass"
            Tags { "LightMode"="SceneSelectionPass" }

            Cull Off

            HLSLPROGRAM

			#define ASE_SRP_VERSION 120108


			#pragma vertex vert
			#pragma fragment frag

            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define FEATURES_GRAPH_VERTEX
            #define SHADERPASS SHADERPASS_DEPTHONLY
			#define SCENESELECTIONPASS 1


            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

			

			sampler2D _MainTex;
			CBUFFER_START( UnityPerMaterial )
			float4 _Color;
			float _StencilReference;
			CBUFFER_END


            struct VertexInput
			{
				float3 positionOS : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};


            int _ObjectId;
            int _PassValue;

			
			VertexOutput vert(VertexInput v )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				o.ase_texcoord.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.positionOS.xyz = vertexValue;
				#else
					v.positionOS.xyz += vertexValue;
				#endif

				VertexPositionInputs vertexInput = GetVertexPositionInputs(v.positionOS.xyz);
				float3 positionWS = TransformObjectToWorld(v.positionOS);
				o.positionCS = TransformWorldToHClip(positionWS);

				return o;
			}

			half4 frag(VertexOutput IN ) : SV_TARGET
			{
				float4 break10 = _Color;
				float2 uv_MainTex9 = IN.ase_texcoord.xy;
				float temp_output_12_0 = ( break10.a * tex2D( _MainTex, uv_MainTex9 ).a );
				float4 appendResult11 = (float4(break10.r , break10.g , break10.b , temp_output_12_0));
				clip( temp_output_12_0 - 0.1);
				
				float4 Color = appendResult11;

				half4 outColor = half4(_ObjectId, _PassValue, 1.0, 1.0);
				return outColor;
			}

            ENDHLSL
        }

		
        Pass
        {
			
            Name "ScenePickingPass"
            Tags { "LightMode"="Picking" }

            Cull Back

            HLSLPROGRAM

			#define ASE_SRP_VERSION 120108


			#pragma vertex vert
			#pragma fragment frag

            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define FEATURES_GRAPH_VERTEX
            #define SHADERPASS SHADERPASS_DEPTHONLY
			#define SCENEPICKINGPASS 1


            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

        	

			sampler2D _MainTex;
			CBUFFER_START( UnityPerMaterial )
			float4 _Color;
			float _StencilReference;
			CBUFFER_END


            struct VertexInput
			{
				float3 positionOS : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

            float4 _SelectionID;

			
			VertexOutput vert(VertexInput v  )
			{
				VertexOutput o = (VertexOutput)0;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );

				o.ase_texcoord.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					float3 defaultVertexValue = v.positionOS.xyz;
				#else
					float3 defaultVertexValue = float3(0, 0, 0);
				#endif
				float3 vertexValue = defaultVertexValue;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
					v.positionOS.xyz = vertexValue;
				#else
					v.positionOS.xyz += vertexValue;
				#endif

				VertexPositionInputs vertexInput = GetVertexPositionInputs(v.positionOS.xyz);
				float3 positionWS = TransformObjectToWorld(v.positionOS);
				o.positionCS = TransformWorldToHClip(positionWS);

				return o;
			}

			half4 frag(VertexOutput IN ) : SV_TARGET
			{
				float4 break10 = _Color;
				float2 uv_MainTex9 = IN.ase_texcoord.xy;
				float temp_output_12_0 = ( break10.a * tex2D( _MainTex, uv_MainTex9 ).a );
				float4 appendResult11 = (float4(break10.r , break10.g , break10.b , temp_output_12_0));
				clip( temp_output_12_0 - 0.1);
				
				float4 Color = appendResult11;
				half4 outColor = _SelectionID;
				return outColor;
			}

            ENDHLSL
        }
		
	}
	CustomEditor "ASEMaterialInspector"
	Fallback "Hidden/InternalErrorShader"
	
}
/*ASEBEGIN
Version=19603
Node;AmplifyShaderEditor.ColorNode;5;-930.5287,-51.15557;Inherit;False;Property;_Color;Color;0;0;Create;True;0;0;0;False;0;False;0.3018868,0.1945689,0.1666074,0.5019608;0,0,0,0;True;True;0;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.TexturePropertyNode;7;-1161.236,260.4232;Inherit;True;Property;_MainTex;MainTex;1;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SamplerNode;9;-878.4715,256.0345;Inherit;True;Property;_TextureSample0;Texture Sample 0;2;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;6;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT3;5
Node;AmplifyShaderEditor.BreakToComponentsNode;10;-592.2957,-48.38689;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-471.6742,238.7345;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;11;-322.4385,-26.71824;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ClipNode;15;-129.2731,183.6091;Inherit;True;3;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;0.1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;14;-1295.85,-173.3452;Inherit;False;Property;_StencilReference;Stencil Reference;2;0;Create;True;0;0;0;True;0;False;33;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;22;188.0744,11.11747;Float;False;False;-1;2;ASEMaterialInspector;0;11;New Amplify Shader;cf964e524c8e69742b1d21fbe2ebcc4a;True;Sprite Unlit Forward;0;1;Sprite Unlit Forward;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;0;True;12;all;0;False;True;6;2;False;;0;False;;3;1;False;;10;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;True;255;True;_StencilReference;255;False;;255;False;;6;False;;3;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=UniversalForward;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;23;188.0744,11.11747;Float;False;False;-1;2;ASEMaterialInspector;0;11;New Amplify Shader;cf964e524c8e69742b1d21fbe2ebcc4a;True;SceneSelectionPass;0;2;SceneSelectionPass;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;0;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=SceneSelectionPass;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;24;188.0744,11.11747;Float;False;False;-1;2;ASEMaterialInspector;0;11;New Amplify Shader;cf964e524c8e69742b1d21fbe2ebcc4a;True;ScenePickingPass;0;3;ScenePickingPass;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;0;True;12;all;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Picking;False;False;0;Hidden/InternalErrorShader;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;21;188.0744,11.11747;Float;False;True;-1;2;ASEMaterialInspector;0;15;Cainos/Pixel Art Platformer - Dungeon/Sprite Shadow;cf964e524c8e69742b1d21fbe2ebcc4a;True;Sprite Unlit;0;0;Sprite Unlit;4;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=UniversalPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;0;True;12;all;0;True;True;6;2;False;;0;False;;3;1;False;;10;False;;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;True;True;True;255;True;_StencilReference;255;False;;255;False;;6;False;;3;False;;1;False;;1;False;;7;False;;1;False;;1;False;;1;False;;False;True;2;False;;True;3;False;;True;True;0;False;;0;False;;True;1;LightMode=Universal2D;False;False;0;Hidden/InternalErrorShader;0;0;Standard;3;Vertex Position;1;0;Debug Display;0;0;External Alpha;0;0;0;4;True;True;True;True;False;;False;0
WireConnection;9;0;7;0
WireConnection;10;0;5;0
WireConnection;12;0;10;3
WireConnection;12;1;9;4
WireConnection;11;0;10;0
WireConnection;11;1;10;1
WireConnection;11;2;10;2
WireConnection;11;3;12;0
WireConnection;15;0;11;0
WireConnection;15;1;12;0
WireConnection;21;1;15;0
ASEEND*/
//CHKSM=1C8AC1C92DE1D892435F8BE3472E6E48CD44F39B