#ifndef STRUCTUREDBUFFER_DATA_INCLUDED
#define STRUCTUREDBUFFER_DATA_INCLUDED

#if defined(UNITY_PROCEDURAL_INSTANCING_ENABLED)
StructuredBuffer<float4x4> _matrixBuffer;
StructuredBuffer<float4> _uvBuffer;
#endif

#endif