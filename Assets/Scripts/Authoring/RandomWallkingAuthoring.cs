using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class RandomWalkingAuthoring : MonoBehaviour {

 public float3 targetPositoin;
    public float3 originPosition;
    public float distanceMin;
    public float distanceMax;
    class Baker : Baker<RandomWalkingAuthoring> {
        public override void Bake(RandomWalkingAuthoring authoring) {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new RandomWalking {
                targetPositoin = authoring.targetPositoin,
                originPosition = authoring.originPosition,
                distanceMin = authoring.distanceMin,
                distanceMax = authoring.distanceMax,
            });
        }
    }
}

public struct RandomWalking : IComponentData {

    public float3 targetPositoin;
    public float3 originPosition;
    public float distanceMin;
    public float distanceMax;
}