using System.Collections.Generic;
using Unity.Burst;
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


        List<float4x4> matrixList = new List<float4x4>();
        List<float4> uvList = new List<float4>();

        Entities.ForEach((ref LocalTransform localTransform, ref SpriteSheetAnimation_Data spriteSheetAnimationData) => {

            matrixList.Add(spriteSheetAnimationData.matrix);
            uvList.Add(spriteSheetAnimationData.uv);
        })
        .WithAll<TestTag>()
        .WithoutBurst()
        .Run();

        ComputeBuffer uvBuffer = GameHandler.Instance.uvBuffer;
        ComputeBuffer matrixBuffer = GameHandler.Instance.matrixBuffer;
        int count = GameHandler.Instance.EntityCount;

        if (uvBuffer == null || matrixBuffer == null) {
            return;
        }


        uvBuffer.SetData(uvList);
        matrixBuffer.SetData(matrixList);


        material.SetBuffer("_uvBuffer",uvBuffer);
        material.SetBuffer("_matrixBuffer",matrixBuffer);

        Bounds bounds = new Bounds(float3.zero, new float3(float.MaxValue, float.MaxValue, float.MaxValue));
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, count);
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        GameHandler.Instance.Dispose();
    }
}



public struct TestTag : IComponentData {

}