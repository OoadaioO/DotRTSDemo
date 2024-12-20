using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct ZombieSpawnerSystem : ISystem {

    public void OnCreate(ref SystemState state){
        state.RequireForUpdate<EntitiesReferences>();
    }


    [BurstCompile]
    public void OnUpdate(ref SystemState state) {

        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

        EntityCommandBuffer entityCommandBuffer =  SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);


        foreach ((
            RefRO<LocalTransform> localTransform,
            RefRW<ZombieSpawner> zombieSpawner)
            in SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRW<ZombieSpawner>>()) {
            zombieSpawner.ValueRW.timer -= SystemAPI.Time.DeltaTime;
            if (zombieSpawner.ValueRO.timer > 0f) {
                continue;
            }
            zombieSpawner.ValueRW.timer = zombieSpawner.ValueRO.timerMax;

            Entity zombieEntity = state.EntityManager.Instantiate(entitiesReferences.zombiePrefabEntity);
            SystemAPI.SetComponent<LocalTransform>(zombieEntity, LocalTransform.FromPosition(localTransform.ValueRO.Position));

            entityCommandBuffer.AddComponent(zombieEntity,new RandomWalking{
                originPosition = localTransform.ValueRO.Position,
                targetPositoin = localTransform.ValueRO.Position,
                distanceMin = zombieSpawner.ValueRO.randomWalkingDistanceMin,
                distanceMax = zombieSpawner.ValueRO.randomWalkingDistanceMax,
                random = new Unity.Mathematics.Random((uint)zombieEntity.Index)
            });
        }
    }


}
