using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

class UnitMoverAuthoring : MonoBehaviour
{
	public float moveSpeed;
	public float rotationSpeed;

	class MoveSpeedAuthoringBaker : Baker<UnitMoverAuthoring>
	{
		public override void Bake(UnitMoverAuthoring authoring)
		{
			var entity = GetEntity(TransformUsageFlags.Dynamic);

			AddComponent(entity, new UnitMover
			{
				moveSpeed = authoring.moveSpeed,
				rotationSpeed = authoring.rotationSpeed,
			});


		}
	}

}

public struct UnitMover : IComponentData
{
	public float moveSpeed;
	public float rotationSpeed;
	public float3 targetPosition;
}
