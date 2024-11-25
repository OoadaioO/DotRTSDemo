using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;


partial struct ShootAttackSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<ShootAttack>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {

        EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

        foreach ((RefRO<LocalTransform> localTransform, RefRW<ShootAttack> shootAttack, RefRO<Target> target) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<ShootAttack>, RefRO<Target>>()) {

            if (target.ValueRO.targetEntity == Entity.Null) {
                continue;
            }

            shootAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;
            if (shootAttack.ValueRO.timer > 0) {
                continue;
            }
            shootAttack.ValueRW.timer = shootAttack.ValueRO.timerMax;

            Entity bulletEntity = state.EntityManager.Instantiate(entitiesReferences.bulletPrefabEntity);
            SystemAPI.SetComponent(bulletEntity, LocalTransform.FromPosition(localTransform.ValueRO.Position));

            RefRW<Bullet> bulletBullet = SystemAPI.GetComponentRW<Bullet>(bulletEntity);
            bulletBullet.ValueRW.damageAmount = shootAttack.ValueRO.damageAmount;

            RefRW<Target> bulletTarget =  SystemAPI.GetComponentRW<Target>(bulletEntity);
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
