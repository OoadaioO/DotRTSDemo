using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

partial struct RandomWalkingSystem : ISystem {

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {

        foreach ((RefRO<LocalTransform> localTransform, RefRW<UnitMover> unitMover, RefRW<RandomWalking> randomWalking) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<UnitMover>, RefRW<RandomWalking>>()) {



            if (math.lengthsq(randomWalking.ValueRO.targetPositoin - localTransform.ValueRO.Position) <= UnitMoverSystem.REACH_TARGET_POSITION_DISTANCE) {
                // reach target position
                Unity.Mathematics.Random random = randomWalking.ValueRO.random;
                float3 moveDirection = new float3(random.NextFloat(-1, 1), 0, random.NextFloat(-1, 1));
                moveDirection = math.normalizesafe(moveDirection);

                randomWalking.ValueRW.targetPositoin = randomWalking.ValueRO.originPosition +
                    moveDirection * random.NextFloat(randomWalking.ValueRO.distanceMin, randomWalking.ValueRO.distanceMax);

                randomWalking.ValueRW.random = random;
            } else {
                unitMover.ValueRW.targetPosition = randomWalking.ValueRO.targetPositoin;
            }



        }
    }

}