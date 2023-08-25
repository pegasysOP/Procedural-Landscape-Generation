Shader "Shaders/BasicNoiseShader"
{
    Properties
    {
        
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
    
        CGPROGRAM
        
        #pragma surface surf Standard fullforwardshadows
        
        #include "WhiteNoise.cginc"
        
        struct Input
        {
            float3 worldPos;
        };
        
        void surf(Input i, inout SurfaceOutputStandard o)
        {
            o.Albedo = rand3dTo3d(i.worldPos);
        }
        
        ENDCG
    }
    Fallback "Standard"
}