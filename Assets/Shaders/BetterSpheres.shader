Shader "Shaders/BetterSpheres"
{
    Properties
    {
        [HDR] _Color ("Tint", Color) = (0, 0, 0, 1)
    }
    
    SubShader
    {
        Tags{ "RenderType"="Opaque" "Queue"="Geometry" }
        
        Pass
        {
            CGPROGRAM
            
            #include "UnityCG.cginc"
        
            #pragma vertex vert
            #pragma fragment frag
        
            fixed4 _Color;
            
            StructuredBuffer<float3> SphereLocations;
            StructuredBuffer<int> Triangles;
            StructuredBuffer<float3> Positions;
        
            float4 vert(uint vertex_id : SV_VertexID, uint instance_id : SV_InstanceID) : SV_POSITION
            {
                int positionIndex = Triangles[vertex_id];
                float3 position = Positions[positionIndex];
                
                position += SphereLocations[instance_id];
                
                return mul(UNITY_MATRIX_VP, float4(position, 1));
            }

            fixed4 frag() : SV_TARGET
            {
                return _Color;
            }
        
            ENDCG
        }
    }
    Fallback "VertexLit"
}