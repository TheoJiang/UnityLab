Shader "Unlit/VoxelLightingColor"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Geometry"
        }
                LOD 100

        Pass
        {
                        // Render State
            Cull Back
        Blend One Zero
        ZTest LEqual
        ZWrite On
            HLSLPROGRAM
// Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
// #pragma exclude_renderers d3d11 gles

            // #include "UnityCG.cginc"
                        // Pragmas
            #pragma target 4.5
            #pragma exclude_renderers gles gles3 glcore
            #pragma multi_compile_instancing
            #pragma multi_compile_fog
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma vertex vert
            #pragma fragment frag
            #pragma enable_d3d11_debug_symbols

                        // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                // UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float3 worldPos : TEXCOORD1;
                float4 index : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 lights[10*10*10];
            float4 Pos[10*10*10];
            
            v2f vert (appdata v)
            {
                v2f o;
                float3 positionWS = TransformObjectToWorld(v.vertex);
                o.vertex = TransformWorldToHClip(positionWS);
                o.worldPos = positionWS;
                // int x = abs(o.worldPos.x)*2 + 10;
                // int y = abs(o.worldPos.y)*2 + 10;
                // int z = abs(o.worldPos.z)*2 + 10;
                // int x = trunc(o.worldPos.x) + 10;
                // int y = trunc(o.worldPos.y) + 10;
                // int z = trunc(o.worldPos.z) + 10;
                // float index = x * (y - 1) * (z-1) + y*z;
                // o.index = float4(x,y,z,index);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = TransformObjectToWorld(v.normal);

                // o.normal = v.normal;
                // UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // sample the texture
                int x = trunc(i.worldPos.x) + 5;
                int y = trunc(i.worldPos.y) + 5;
                int z = trunc(i.worldPos.z) + 5;
                int index = x*y*z + (x-1)*10*10;
                int index0 = x*y*z + (x-1)*10*10;
                // float index = x * (y - 1) * (z-1) + y*z;
                // o.index = float4(x,y,z,index);

                int idx = index;
                float4 col = tex2D(_MainTex, i.uv);
                // int idx = i.index.w;
                // col += dot(normalize((Pos[idx] - (i.worldPos))), normalize(i.normal)) * lights[idx];
                col *=  lights[idx];
                col.a = 1;
                // apply fog
                // UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDHLSL
        }
    }
}
