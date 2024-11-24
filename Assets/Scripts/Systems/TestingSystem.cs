using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
partial class TestingSystem : SystemBase {



    protected override void OnUpdate() {

        //TestCount();

        if (Input.GetKeyDown(KeyCode.T)) {
            foreach ((RefRW<Health> health, Entity entity) in SystemAPI.Query<RefRW<Health>>().WithEntityAccess()) {
                health.ValueRW.healthAmount = -1;
                Debug.Log("set target health 0:" + entity);
            }
        }
    }

    private void TestCount() {
        int unitCount = 0;

        foreach (RefRO<LocalTransform> localTransform in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<Friendly>()) {
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
