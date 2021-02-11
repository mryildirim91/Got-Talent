Shader "Hidden/FlatKit/OutlineFilter"
{
    Properties
    {
        [HideInInspector]_MainTex ("Base (RGB)", 2D) = "white" {}

        _EdgeColor ("Outline Color", Color) = (1, 1, 1, 1)
        _Thickness ("Thickness", Range(0, 5)) = 1

        [Space(15)]
        [Toggle(OUTLINE_USE_DEPTH)]_UseDepth ("Use Depth", Float) = 1
        _DepthThresholdMin ("Min Depth Threshold", Range(0, 1)) = 0
        _DepthThresholdMax ("Max Depth Threshold", Range(0, 1)) = 0.25

        [Space(15)]
        [Toggle(OUTLINE_USE_NORMALS)]_UseNormals ("Use Normals", Float) = 0
        _NormalThresholdMin ("Min Normal Threshold", Range(0, 1)) = 0.5
        _NormalThresholdMax ("Max Normal Threshold", Range(0, 1)) = 1.0

        [Space(15)]
        [Toggle(OUTLINE_USE_COLOR)]_UseColor ("Use Color", Float) = 0
        _ColorThresholdMin ("Min Color Threshold", Range(0, 1)) = 0
        _ColorThresholdMax ("Max Color Threshold", Range(0, 1)) = 0.25

        [Space(15)]
        [Toggle(OUTLINE_ONLY)]_OutlineOnly ("Outline Only", Float) = 0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 200

        Pass
        {
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

            #pragma shader_feature OUTLINE_USE_DEPTH
            #pragma shader_feature OUTLINE_USE_NORMALS
            #pragma shader_feature OUTLINE_USE_COLOR
            #pragma shader_feature OUTLINE_ONLY

            uniform half _Thickness;
            uniform half4 _EdgeColor;
            uniform half _DepthThresholdMin, _DepthThresholdMax;
            uniform half _NormalThresholdMin, _NormalThresholdMax;
            uniform half _ColorThresholdMin, _ColorThresholdMax;

            TEXTURE2D(_CameraColorTexture);
            SAMPLER(sampler_CameraColorTexture);
            float4 _CameraColorTexture_TexelSize;

            TEXTURE2D(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);

            TEXTURE2D(_CameraDepthNormalsTexture);
            SAMPLER(sampler_CameraDepthNormalsTexture);

            float3 DecodeViewNormalStereo(float4 enc4)
            {
                float kScale = 1.7777;
                float3 nn = enc4.xyz * float3(2.0 * kScale, 2.0 * kScale, 0) + float3(-kScale, -kScale, 1);
                float g = 2.0 / dot(nn.xyz, nn.xyz);
                float3 n;
                n.xy = g * nn.xy;
                n.z = g - 1.0;
                return n;
            }

            // Z buffer depth to linear 0-1 depth
            // Handles orthographic projection correctly
            float Linear01Depth(float z)
            {
                float isOrtho = unity_OrthoParams.w;
                float isPers = 1.0 - unity_OrthoParams.w;
                z *= _ZBufferParams.x;
                return (1.0 - isOrtho * z) / (isPers * z + _ZBufferParams.y);
            }

            // Decode normals stored in _CameraDepthNormalsTexture
            float3 SampleNormal(float2 uv)
            {
                float4 raw = SAMPLE_TEXTURE2D(_CameraDepthNormalsTexture, sampler_CameraDepthNormalsTexture, uv);
                return DecodeViewNormalStereo(raw);
            }

            float SampleDepth(float2 uv)
            {
                float d;
                #if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
                d = SAMPLE_TEXTURE2D_ARRAY(_CameraDepthTexture, sampler_CameraDepthTexture, uv, unity_StereoEyeIndex).r;
                #else
                d = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, uv);
                #endif
                return Linear01Depth(d);
            }

            float4 Outline(float2 uv)
            {
                float4 original = SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, uv);

                float offset_positive = + ceil(_Thickness * 0.5);
                float offset_negative = - floor(_Thickness * 0.5);
                float2 texel_size = 1.0 / float2(_CameraColorTexture_TexelSize.z, _CameraColorTexture_TexelSize.w);

                float left = texel_size.x * offset_negative;
                float right = texel_size.x * offset_positive;
                float top = texel_size.y * offset_negative;
                float bottom = texel_size.y * offset_positive;

                float2 uv0 = uv + float2(left, top);
                float2 uv1 = uv + float2(right, bottom);
                float2 uv2 = uv + float2(right, top);
                float2 uv3 = uv + float2(left, bottom);

                #ifdef OUTLINE_USE_DEPTH
                float d0 = SampleDepth(uv0);
                float d1 = SampleDepth(uv1);
                float d2 = SampleDepth(uv2);
                float d3 = SampleDepth(uv3);
                
                float depthThresholdScale = 300.0;
                float d = length(float2(d1 - d0, d3 - d2)) * depthThresholdScale;
                d = smoothstep(_DepthThresholdMin, _DepthThresholdMax, d);
                #else
                float d = 0;
                #endif  // OUTLINE_USE_DEPTH

                #ifdef OUTLINE_USE_NORMALS
                float3 n0 = SampleNormal(uv0);
                float3 n1 = SampleNormal(uv1);
                float3 n2 = SampleNormal(uv2);
                float3 n3 = SampleNormal(uv3);
                
                float3 nd1 = n1 - n0;
                float3 nd2 = n3 - n2;
                float n = sqrt(dot(nd1, nd1) + dot(nd2, nd2));
                n = smoothstep(_NormalThresholdMin, _NormalThresholdMax, n);
                #else
                float n = 0;
                #endif  // OUTLINE_USE_NORMALS

                #ifdef OUTLINE_USE_COLOR
                float3 c0 = SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, uv0);
                float3 c1 = SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, uv1);
                float3 c2 = SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, uv2);
                float3 c3 = SAMPLE_TEXTURE2D(_CameraColorTexture, sampler_CameraColorTexture, uv3);

                float3 cd1 = c1 - c0;
                float3 cd2 = c3 - c2;
                float c = sqrt(dot(cd1, cd1) + dot(cd2, cd2));
                c = smoothstep(_ColorThresholdMin, _ColorThresholdMax, c);
                #else
                float c = 0;
                #endif  // OUTLINE_USE_COLOR

                float g = max(d, max(n, c));

                #ifdef OUTLINE_ONLY
                original.rgb = lerp(1.0 - _EdgeColor.rgb, _EdgeColor.rgb, g * _EdgeColor.a);
                #endif  // OUTLINE_ONLY

                float4 output;
                output.rgb = lerp(original.rgb, _EdgeColor.rgb, g * _EdgeColor.a);
                output.a = original.a;
                return output;
            }

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.vertex = vertexInput.positionCS;
                output.uv = input.uv;

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                float4 c = Outline(input.uv);
                return c;
            }

            #pragma vertex vert
            #pragma fragment frag
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}