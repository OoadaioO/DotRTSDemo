using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
partial struct ResetTargetSystem : ISystem {


    [BurstCompile]
    public void OnUpdate(ref SystemState state) {


        foreach (RefRW<Target> target in SystemAPI.Query<RefRW<Target>>()) {
            if (target.ValueRO.targetEntity != null) {
                // Target with Cleanup Component my be not destory on end of the frame
                // Double check the target entity with component removed
                if (!SystemAPI.Exists(target.ValueRO.targetEntity) || !SystemAPI.HasComponent<LocalTransform>(target.ValueRO.targetEntity)) {
                    target.ValueRW.targetEntity = Entity.Null;
                }
            }
        }

    }

}
