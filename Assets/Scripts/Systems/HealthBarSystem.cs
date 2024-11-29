using System.Numerics;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

// partial struct HealthBarSystem : ISystem
// {


//     [BurstCompile]
//     public void OnUpdate(ref SystemState state)
//     {

//         foreach(RefRO<HealthBar> healthBar in SystemAPI.Query<RefRO<HealthBar>>()){

//             Health health = SystemAPI.GetComponent<Health>(healthBar.ValueRO.healthEntity); 
//             float healthNormalized = (float)health.healthAmount / health.healthMax;


//             RefRW<PostTransformMatrix> barVisualPostTransformMatrix = SystemAPI.GetComponentRW<PostTransformMatrix>(healthBar.ValueRO.barVisualEntity);
//             barVisualPostTransformMatrix.ValueRW.Value = float4x4.Scale(healthNormalized, 1, 1);

//             //RefRW<LocalTransform> barVisualLocalTransform = SystemAPI.GetComponentRW<LocalTransform>(healthBar.ValueRO.barVisualEntity);
//             //barVisualLocalTransform.ValueRW.Scale = healthNormalized;


//         }

//     }


// }

partial class HealthBarSystem : SystemBase {

    protected override void OnUpdate() {

        float3 cameraForward = float3.zero;
        if (Camera.main != null) {
            cameraForward = Camera.main.transform.forward;
        }

        foreach ((RefRO<HealthBar> healthBar, RefRW<LocalTransform> localTransform) in SystemAPI.Query<RefRO<HealthBar>, RefRW<LocalTransform>>()) {

            LocalTransform healthTransform = SystemAPI.GetComponent<LocalTransform>(healthBar.ValueRO.healthEntity);

            if (localTransform.ValueRO.Scale == 1) {
                // health bar is visible
                localTransform.ValueRW.Rotation = healthTransform.InverseTransformRotation(quaternion.LookRotation(cameraForward, math.up()));
            }



            Health health = SystemAPI.GetComponent<Health>(healthBar.ValueRO.healthEntity);

            if (!health.onHealthChanged) {
                continue;
            }
            
            
            float healthNormalized = (float)health.healthAmount / health.healthMax;

            if (healthNormalized >= 1 || healthNormalized < 0) {
                localTransform.ValueRW.Scale = 0f;
            } else {
                localTransform.ValueRW.Scale = 1f;
            }

            RefRW<PostTransformMatrix> barVisualPostTransformMatrix = SystemAPI.GetComponentRW<PostTransformMatrix>(healthBar.ValueRO.barVisualEntity);
            barVisualPostTransformMatrix.ValueRW.Value = float4x4.Scale(healthNormalized, 1, 1);


        }
    }
}
