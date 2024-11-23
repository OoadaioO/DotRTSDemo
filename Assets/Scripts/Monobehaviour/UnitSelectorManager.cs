using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class UnitSelectorManager : MonoBehaviour {

    public static UnitSelectorManager Instance { get; private set; }

    public event EventHandler OnSelectionAreaStart;
    public event EventHandler OnSelectionAreaEnd;

    [SerializeField] private LayerMask unitLayer;

    private Vector2 selectionStartMousePosition;



    private void Awake() {
        Instance = this;
    }

    // Update is called once per frame
    private void Update() {

        if (Input.GetMouseButtonDown(0)) {
            selectionStartMousePosition = Input.mousePosition;
            OnSelectionAreaStart?.Invoke(this, EventArgs.Empty);
        }
        if (Input.GetMouseButtonUp(0)) {
            Vector3 selectionEndMousePosition = Input.mousePosition;


            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;



            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().Build(entityManager);
            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
            NativeArray<Selected> selectedArray = entityQuery.ToComponentDataArray<Selected>(Allocator.Temp);

            for (int i = 0; i < entityArray.Length; i++) {
                entityManager.SetComponentEnabled<Selected>(entityArray[i], false);
                Selected selected = selectedArray[i];
                selected.onDeselected = true;
                entityManager.SetComponentData(entityArray[i], selected);
            }




            Rect selectionAreaRect = GetSelectionRectArea();

            float selectionAreaSize = selectionAreaRect.width + selectionAreaRect.height;
            float multiplySelectionSizeMin = 40;
            bool isMultiplySelectionArea = selectionAreaSize > multiplySelectionSizeMin;

            if (isMultiplySelectionArea) {
                entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform>().WithPresent<Selected>().Build(entityManager);

                entityArray = entityQuery.ToEntityArray(Allocator.Temp);
                NativeArray<LocalTransform> localTransformArray = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
                for (int i = 0; i < localTransformArray.Length; i++) {
                    LocalTransform unitLocalTransform = localTransformArray[i];
                    Vector2 unitScreenPosition = Camera.main.WorldToScreenPoint(unitLocalTransform.Position);
                    if (selectionAreaRect.Contains(unitScreenPosition)) {
                        entityManager.SetComponentEnabled<Selected>(entityArray[i], true);

                        Selected selected = entityManager.GetComponentData<Selected>(entityArray[i]);
                        selected.onSelected = true;
                        entityManager.SetComponentData(entityArray[i], selected);
                    }
                }


            } else {
                // single select
                entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
                PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();
                CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;

                UnityEngine.Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);


                RaycastInput raycastInput = new RaycastInput {
                    Start = cameraRay.GetPoint(0),
                    End = cameraRay.GetPoint(9999f),
                    Filter = new CollisionFilter {
                        BelongsTo = ~0u,
                        CollidesWith = (uint)unitLayer.value,
                        GroupIndex = 0,
                    }

                };
                if (collisionWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit raycastHit)) {
                    if (entityManager.HasComponent<Unit>(raycastHit.Entity)) {
                        // Hit a unit
                        entityManager.SetComponentEnabled<Selected>(raycastHit.Entity, true);
                        Selected selected = entityManager.GetComponentData<Selected>(raycastHit.Entity);
                        selected.onSelected = true;
                        entityManager.SetComponentData(raycastHit.Entity, selected);

                    }
                }

            }


            OnSelectionAreaEnd?.Invoke(this, EventArgs.Empty);
        }



        if (Input.GetMouseButtonDown(1)) {

            Vector3 mouseWorldPosition = MouseWorldPosition.Instance.GetPosition();

            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<UnitMover, Selected>().Build(entityManager);

            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
            NativeArray<UnitMover> unitMoverArray = entityQuery.ToComponentDataArray<UnitMover>(Allocator.Temp);
            NativeArray<float3> movePositionArray = GenerateMovePositionArray(mouseWorldPosition, entityArray.Length);


            for (int i = 0; i < unitMoverArray.Length; i++) {
                UnitMover unitMover = unitMoverArray[i];
                unitMover.targetPosition = movePositionArray[i];
                entityManager.SetComponentData(entityArray[i], unitMover);
            }


        }


    }


    public Rect GetSelectionRectArea() {

        Vector2 lowerLeft = new Vector2(
            Mathf.Min(selectionStartMousePosition.x, Input.mousePosition.x),
            Mathf.Min(selectionStartMousePosition.y, Input.mousePosition.y)
        );
        Vector2 upperRight = new Vector2(
            Mathf.Max(selectionStartMousePosition.x, Input.mousePosition.x),
            Mathf.Max(selectionStartMousePosition.y, Input.mousePosition.y)
        );


        return new Rect(
            lowerLeft.x,
            lowerLeft.y,
            upperRight.x - lowerLeft.x,
            upperRight.y - lowerLeft.y
        );
    }


    private NativeArray<float3> GenerateMovePositionArray(float3 targetPosition, int positionCount) {

        NativeArray<float3> positionArray = new NativeArray<float3>(positionCount, Allocator.Temp);
        if (positionCount == 0) {
            return positionArray;
        }
        positionArray[0] = targetPosition;
        if (positionCount == 1) {
            return positionArray;
        }

        float ringSize = 2.2f;
        int ring = 0;
        int positionIndex = 1;

        while (positionIndex < positionCount) {

            int ringPositionCount = 3 + ring * 2;

            for (int i = 0; i < ringPositionCount; i++) {

                float angle = i * math.PI2 / ringPositionCount;
                float3 ringVector = math.rotate(quaternion.RotateY(angle), new float3(ringSize * (ring + 1), 0, 0));
                float3 ringPosition = targetPosition + ringVector;

                positionArray[positionIndex] = ringPosition;
                positionIndex++;

                if (positionIndex >= positionCount) {
                    break;
                }
            }
            ring++;
        }
        return positionArray;

    }


}
