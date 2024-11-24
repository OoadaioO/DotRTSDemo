using Unity.Burst;
using Unity.Entities;


partial struct ShootAttackSystem : ISystem {
    [BurstCompile]
    public void OnCreate(ref SystemState state) {
        state.RequireForUpdate<ShootAttack>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state) {

        foreach ((RefRW<ShootAttack> shootAttack, RefRO<Target> target) in SystemAPI.Query<RefRW<ShootAttack>, RefRO<Target>>()) {

            if (target.ValueRO.targetEntity == Entity.Null) {
                continue;
            }

            shootAttack.ValueRW.timer -= SystemAPI.Time.DeltaTime;
            if (shootAttack.ValueRO.timer > 0) {
                continue;
            }
            shootAttack.ValueRW.timer = shootAttack.ValueRO.timerMax;


            RefRW<Health> health = SystemAPI.GetComponentRW<Health>(target.ValueRO.targetEntity);

            int damageAmount = 1;
            health.ValueRW.healthAmount -= damageAmount;

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
