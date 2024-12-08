#include "Data.hlsl"


void GetUVFunction_float(out float4 Out) {
    #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
        Out = _uvBuffer[unity_InstanceID];
    #else
        Out = float4(0,0,0,0);
    #endif
    
}

void GetUVFunction_half (out half4 Out) {
    #if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
        Out = _uvBuffer[unity_InstanceID];
    #else
        Out = half4(0,0,0,0);
    #endif
    
}
