using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.UI;

[UpdateAfter(typeof(SpriteSheetAnimationSystem))]
partial class SpriteSheetRenderSystem : SystemBase {



    protected override void OnCreate() {
        base.OnCreate();

    }

    protected override void OnUpdate() {
        Mesh mesh = GameHandler.Instance.Mesh;
        Material material = GameHandler.Instance.Material;
        if (mesh == null || material == null) {
            return;
        }

        int entityCount = GameHandler.Instance.EntityCount;

        NativeArray<float4x4> matrixArray = new NativeArray<float4x4>(entityCount, Allocator.TempJob);
        NativeArray<float4> uvArray = new NativeArray<float4>(entityCount, Allocator.TempJob);


        Entities.ForEach((int entityInQueryIndex, ref LocalTransform localTransform, ref SpriteSheetAnimation_Data spriteSheetAnimationData) => {
            matrixArray[entityInQueryIndex] = spriteSheetAnimationData.matrix;
            uvArray[entityInQueryIndex] = spriteSheetAnimationData.uv;

        })
        .WithAll<TestTag>()
        .WithBurst()
        .ScheduleParallel();

        CompleteDependency();


        ComputeBuffer uvBuffer = GameHandler.Instance.uvBuffer;
        ComputeBuffer matrixBuffer = GameHandler.Instance.matrixBuffer;


        if (uvBuffer == null || matrixBuffer == null) {
            matrixArray.Dispose();
            uvArray.Dispose();
            return;
        }


        uvBuffer.SetData(uvArray);
        matrixBuffer.SetData(matrixArray);


        material.SetBuffer("_uvBuffer", uvBuffer);
        material.SetBuffer("_matrixBuffer", matrixBuffer);

        Bounds bounds = new Bounds(float3.zero, new float3(float.MaxValue, float.MaxValue, float.MaxValue));
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, entityCount);

        matrixArray.Dispose();
        uvArray.Dispose();
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        GameHandler.Instance.Dispose();
    }
}



public struct TestTag : IComponentData {

}