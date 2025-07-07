Shader "Unlit/VolumeShader"
{
    Properties
    {
        _VolumeTexture("3D Texture", 3D) = "white" {}
        _VolumeTextureSize("Texture Size", Vector) = (1,1,1)
        _Alpha("Alpha", Float) = 0.02
        _StepSize("Step Size", Float) = 0.01
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Blend One OneMinusSrcAlpha
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
            float _Alpha;
            float _StepSize;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 objectVertex : TEXCOORD0;
                float3 rayDirObjectSpace : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 objCamPos = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1.0)).xyz;

                o.objectVertex = v.vertex.xyz;
                o.rayDirObjectSpace = normalize(o.objectVertex - objCamPos);
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 BlendUnder(float4 color, float4 newColor)
            {
                color.rgb += (1.0 - color.a) * newColor.a * newColor.rgb;
                color.a += (1.0 - color.a) * newColor.a;
                return color;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 rayOrigin = i.objectVertex;
                float3 rayDir = i.rayDirObjectSpace;
                float3 pos = rayOrigin;

                float4 accumulatedColor = float4(0, 0, 0, 0);

                for (int step = 0; step < MAX_STEP_COUNT; step++)
                {
                    if (max(abs(pos.x), max(abs(pos.y), abs(pos.z))) < 0.5f + EPSILON)
                    {
                        float3 texCoord = pos + float3(0.5, 0.5, 0.5); // Convert [-0.5, 0.5] to [0,1]
                        float4 sampleCol = tex3D(_VolumeTexture, texCoord);
                        sampleCol.a *= _Alpha;
                        accumulatedColor = BlendUnder(accumulatedColor, sampleCol);

                        // Optional early termination
                        if (accumulatedColor.a >= 0.95f)
                            break;

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
}
