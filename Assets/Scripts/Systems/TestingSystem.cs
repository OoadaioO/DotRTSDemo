using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial class TestingSystem : SystemBase {

  

    protected override void OnUpdate() {

       //TestSelectedTag();
    }

    private void TestSelectedTag() {
        int unitCount = 0;

        foreach ((
                    RefRW<LocalTransform> localTransform,
                    RefRO<UnitMover> unitMover,
                    RefRW<PhysicsVelocity> physicsVelocity
                ) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<UnitMover>, RefRW<PhysicsVelocity>>().WithDisabled<Selected>()) {

            unitCount++;

        }

        Debug.Log("unit Count:" + unitCount);
    }



    private void TestSpawnEntities() {
        //*/ test spawn entities
        Entity spawnerEntity = SystemAPI.GetSingletonEntity<UnitSpawner>();
        RefRO<UnitSpawner> unitSpawner = SystemAPI.GetComponentRO<UnitSpawner>(spawnerEntity);
        Unity.Mathematics.Random random = new Unity.Mathematics.Random(123);

        for (int i = 0; i < unitSpawner.ValueRO.count; i++) {
            Entity newEntity = this.EntityManager.Instantiate(unitSpawner.ValueRO.prefab);
            RefRW<LocalTransform> localTransform = SystemAPI.GetComponentRW<LocalTransform>(newEntity);
            localTransform.ValueRW.Position = random.NextFloat3(new float3(-5, 0, -5), new float3(-5, 0, -5));
        }


        this.Enabled = false;
        //*/
    }
}
