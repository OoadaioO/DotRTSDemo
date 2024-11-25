using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;


partial struct ShootAttackSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<ShootAttack>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {

        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

        foreach ((
            RefRW<LocalTransform> localTransform,
            RefRW<ShootAttack> shootAttack,
            RefRO<Target> target,
            RefRW<UnitMover> unitMover)
            in SystemAPI.Query<
                RefRW<LocalTransform>,
                RefRW<ShootAttack>,
                RefRO<Target>,
                RefRW<UnitMover>>()) {

            if (target.ValueRO.targetEntity == Entity.Null) {
                continue;
            }


            LocalTransform targetLocalTransform = SystemAPI.GetComponent<LocalTransform>(target.ValueRO.targetEntity);
            if (math.distance(targetLocalTransform.Position, localTransform.ValueRO.Position) > shootAttack.ValueRO.attackDistance) {
                // Too far , move closer
                unitMover.ValueRW.targetPosition = targetLocalTransform.Position;
                continue;
            } else {
                // Close enough, stop moving and attack
                unitMover.ValueRW.targetPosition = localTransform.ValueRO.Position;
            }

            float3 aimDirection = targetLocalTransform.Position - localTransform.ValueRO.Position;
            aimDirection = math.normalizesafe(aimDirection);

            quaternion targetRotation = quaternion.LookRotationSafe(aimDirection, math.up());
            localTransform.ValueRW.Rotation = math.slerp(localTransform.ValueRO.Rotation,targetRotation,unitMover.ValueRO.rotationSpeed * SystemAPI.Time.DeltaTime);


            shootAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;
            if (shootAttack.ValueRO.timer > 0) {
                continue;
            }
            shootAttack.ValueRW.timer = shootAttack.ValueRO.timerMax;



            Entity bulletEntity = state.EntityManager.Instantiate(entitiesReferences.bulletPrefabEntity);
            float3 bulletSpawnWorldPosition = localTransform.ValueRO.TransformPoint(shootAttack.ValueRO.bulletSpawnLocalPosition);
            SystemAPI.SetComponent(bulletEntity, LocalTransform.FromPosition(bulletSpawnWorldPosition));

            RefRW<Bullet> bulletBullet = SystemAPI.GetComponentRW<Bullet>(bulletEntity);
            bulletBullet.ValueRW.damageAmount = shootAttack.ValueRO.damageAmount;

            RefRW<Target> bulletTarget = SystemAPI.GetComponentRW<Target>(bulletEntity);
            bulletTarget.ValueRW.targetEntity = target.ValueRO.targetEntity;
        }
    }


}



// partial class ShootAttackSystem : SystemBase {
//     protected override void OnCreate() {
//         base.OnCreate();
//         this.RequireForUpdate<ShootAttack>();
//     }

//     protected override void OnUpdate() {
//         foreach ((RefRW<ShootAttack> shootAttack, RefRO<Target> target) in SystemAPI.Query<RefRW<ShootAttack>, RefRO<Target>>()) {
//             shootAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;
//             if (shootAttack.ValueRO.timer > 0) {
//                 continue;
//             }
//             shootAttack.ValueRW.timer = shootAttack.ValueRO.timerMax;
//             if (target.ValueRO.targetEntity == null) {
//                 continue;
//             }


//         }
//     }


// }
