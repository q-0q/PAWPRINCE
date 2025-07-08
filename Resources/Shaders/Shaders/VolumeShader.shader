Shader "Unlit/VolumeRaymarchWithDepth"
{
    Properties
    {
        _VolumeTexture("3D Texture", 3D) = "white" {}
        _VolumeTextureSize("Texture Size", Vector) = (1,1,1)
        _Color("Color", Color) = (1,1,1,1)
        _Alpha("Alpha", Float) = 0.02
        _StepSize("Step Size", Float) = 0.01
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Blend One OneMinusSrcAlpha
        ZWrite Off
        ZTest LEqual
        Cull Back
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            #define MAX_STEP_COUNT 128
            #define EPSILON 0.00001f

            sampler3D _VolumeTexture;
            float3 _VolumeTextureSize;
            float4 _Color;
            float _Alpha;
            float _StepSize;

            UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 objectPos : TEXCOORD0;
                float3 rayDirObject : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
            };

            v2f vert(appdata v)
            {
                v2f o;
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 camObj = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1.0)).xyz;

                o.objectPos = v.vertex.xyz;
                o.rayDirObject = normalize(o.objectPos - camObj);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = o.vertex;

                return o;
            }

            float4 BlendUnder(float4 baseColor, float4 newColor)
            {
                baseColor.rgb += (1.0 - baseColor.a) * newColor.a * newColor.rgb;
                baseColor.a += (1.0 - baseColor.a) * newColor.a;
                return baseColor;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 pos = i.objectPos;
                float3 rayDir = normalize(i.rayDirObject);
                float4 accumulatedColor = float4(0, 0, 0, 0);

                for (int step = 0; step < MAX_STEP_COUNT; step++)
                {
                    if (max(abs(pos.x), max(abs(pos.y), abs(pos.z))) < 0.5f + EPSILON)
                    {
                        float3 worldPos = mul(unity_ObjectToWorld, float4(pos, 1.0)).xyz;
                        float4 clipPos = UnityWorldToClipPos(worldPos);
                        float2 screenUV = (clipPos.xy / clipPos.w) * 0.5 + 0.5;

                        #if UNITY_UV_STARTS_AT_TOP
                        screenUV.y = 1.0 - screenUV.y;
                        #endif

                        // Depth test
                        float sceneDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, UNITY_PROJ_COORD(screenUV));
                        float linearSceneDepth = LinearEyeDepth(sceneDepth);
                        float linearRayDepth = LinearEyeDepth(clipPos.z / clipPos.w);

                        if (linearRayDepth < linearSceneDepth - 0.001)
                        {
                            float3 texCoord = pos + float3(0.5, 0.5, 0.5); // [-0.5, 0.5] -> [0,1]
                            float4 sampleCol = tex3D(_VolumeTexture, texCoord) * _Color;
                            sampleCol.a *= _Alpha;
                            accumulatedColor = BlendUnder(accumulatedColor, sampleCol);

                            if (accumulatedColor.a >= 0.95)
                                break;
                        }

                        pos += rayDir * _StepSize;
                    }
                    else
                    {
                        break;
                    }
                }

                return accumulatedColor;
            }
            ENDCG
        }
    }

    FallBack Off
}
