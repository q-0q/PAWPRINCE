Shader "Unlit/FogShader"
{
    Properties
    {
        _VolumeTexture ("Texture", 3D) = "white" {}
        _VolumeTextureSize ("Texture Size", Vector) = (0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : POSITION;
                float4 localSpacePos : TEXCOORD1;
            };

            sampler3D _VolumeTexture;
            float4 _VolumeTexture_ST;
            float3 _VolumeTextureSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _VolumeTexture);
                
                // o.localSpacePos = mul(unity_ObjectToWorld, v.vertex);
                o.localSpacePos = v.vertex;


                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture

                float normCoordX = (i.localSpacePos.x + 0.5);
                float normCoordY = (i.localSpacePos.y + 0.5);
                float normCoordZ = (i.localSpacePos.z + 0.5);

                float3 normCoord = float3(normCoordX, normCoordY, normCoordZ);
                float4 col = tex3D(_VolumeTexture, normCoord);
                return col;
            }
            ENDCG
        }
    }
}
