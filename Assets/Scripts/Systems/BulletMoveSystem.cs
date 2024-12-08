using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[RequireMatchingQueriesForUpdate]
partial struct BulletMoveSystem : ISystem {

    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<Bullet>();
    }


    [BurstCompile]
    public void OnUpdate(ref SystemState state) {


        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach ((
            RefRW<LocalTransform> localTransform,
            RefRO<Bullet> bullet,
            RefRW<Target> target,
            Entity entity)
            in SystemAPI.Query<
                RefRW<LocalTransform>,
                RefRO<Bullet>,
                RefRW<Target>>().WithEntityAccess()) {


            if (target.ValueRO.targetEntity == Entity.Null) {
                entityCommandBuffer.DestroyEntity(entity);
                continue;
            }

            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
            ShootVictim shootVictim = SystemAPI.GetComponent<ShootVictim>(target.ValueRO.targetEntity);

            float3 targetPosition = targetLocalTransform.TransformPoint(shootVictim.hitLocalPosition); 

            float3 moveDirection = targetPosition - localTransform.ValueRO.Position;
            moveDirection = math.normalizesafe(moveDirection);

            float distanceBeforeSq = math.distancesq(localTransform.ValueRO.Position, targetPosition);

            localTransform.ValueRW.Position += bullet.ValueRO.speed * SystemAPI.Time.DeltaTime * moveDirection;

            float distanceAfterSq = math.distancesq(localTransform.ValueRO.Position, targetPosition);

            if (distanceAfterSq > distanceBeforeSq) {
                // overshot
                localTransform.ValueRW.Position = targetPosition;
            }

            float destoryDistanceSq = 0.0000001f;
            if ((float)math.distancesq(localTransform.ValueRO.Position, targetPosition) < destoryDistanceSq) {
                // Close enough to damage target
                RefRW<Health> targetHealth = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);
                targetHealth.ValueRW.healthAmount -= bullet.ValueRO.damageAmount;
                targetHealth.ValueRW.onHealthChanged = true;

                entityCommandBuffer.DestroyEntity(entity);
            }
        }

    }


}
