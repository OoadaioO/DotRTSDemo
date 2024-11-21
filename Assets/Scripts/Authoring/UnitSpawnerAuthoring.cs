using Unity.Entities;
using UnityEngine;

public class UnitSpawnerAuthoring : MonoBehaviour {

    public GameObject prefab;
    public int count;

    public class Baker : Baker<UnitSpawnerAuthoring> {
        public override void Bake(UnitSpawnerAuthoring authoring) {

            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            Entity prefabEntity = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic);

            AddComponent(entity, new UnitSpawner {
                prefab = prefabEntity,
                count = authoring.count
            });


        }
    }
}
public struct UnitSpawner : IComponentData {
    public Entity prefab;
    public int count;
}
