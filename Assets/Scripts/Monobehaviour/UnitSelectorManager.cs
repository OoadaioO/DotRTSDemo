using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class UnitSelectorManager : MonoBehaviour {

    public static UnitSelectorManager Instance { get; private set; }

    public event EventHandler OnSelectionAreaStart;
    public event EventHandler OnSelectionAreaEnd;


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

            Rect selectionAreaRect = GetSelectionRectArea();

            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().Build(entityManager);
            NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);

            for (int i = 0; i < entityArray.Length; i++) {
                entityManager.SetComponentEnabled<Selected>(entityArray[i], false);
            }


            entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<LocalTransform>().WithPresent<Selected>().Build(entityManager);
            entityArray = entityQuery.ToEntityArray(Allocator.Temp);
            NativeArray<LocalTransform> localTransformArray = entityQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);

            for (int i = 0; i < localTransformArray.Length; i++) {
                LocalTransform unitLocalTransform = localTransformArray[i];

                Vector2 unitScreenPosition = Camera.main.WorldToScreenPoint(unitLocalTransform.Position);
                if (selectionAreaRect.Contains(unitScreenPosition)) {
                    entityManager.SetComponentEnabled<Selected>(entityArray[i], true);
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


            for (int i = 0; i < unitMoverArray.Length; i++) {
                UnitMover unitMover = unitMoverArray[i];
                unitMover.targetPosition = mouseWorldPosition;
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


}
