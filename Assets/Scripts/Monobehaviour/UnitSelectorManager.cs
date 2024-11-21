using System;
using Unity.Collections;
using Unity.Entities;
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
            OnSelectionAreaEnd?.Invoke(this, EventArgs.Empty);
        }






        if (Input.GetMouseButtonDown(1)) {

            Vector3 mouseWorldPosition = MouseWorldPosition.Instance.GetPosition();

            EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<UnitMover, Selected>().Build(entityManager);

            NativeArray<UnitMover> unitMoverArray = entityQuery.ToComponentDataArray<UnitMover>(Allocator.Temp);


            for (int i = 0; i < unitMoverArray.Length; i++) {
                UnitMover unitMover = unitMoverArray[i];
                unitMover.targetPosition = mouseWorldPosition;
                unitMoverArray[i] = unitMover;
            }

            entityQuery.CopyFromComponentDataArray(unitMoverArray);

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
