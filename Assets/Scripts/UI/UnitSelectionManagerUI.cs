using System;
using UnityEngine;

public class UnitSelectionManagerUI : MonoBehaviour {

    [SerializeField] private RectTransform selectionAreaRectTransform;

    private Canvas canvas;

    private void Awake() {
        canvas = GetComponentInParent<Canvas>();
    }

    private void Start() {
        selectionAreaRectTransform.gameObject.SetActive(false);

        UnitSelectorManager.Instance.OnSelectionAreaStart += UnitSelectorManager_OnSelectionAreaStart;
        UnitSelectorManager.Instance.OnSelectionAreaEnd += UnitSelectorManager_OnSelectionAreaEnd;
    }


    private void OnDestroy() {

        UnitSelectorManager.Instance.OnSelectionAreaStart -= UnitSelectorManager_OnSelectionAreaStart;
        UnitSelectorManager.Instance.OnSelectionAreaEnd -= UnitSelectorManager_OnSelectionAreaEnd;
    }


    private void UnitSelectorManager_OnSelectionAreaStart(object sender, EventArgs e) {
        selectionAreaRectTransform.gameObject.SetActive(true);
        UpdateVisual();
    }

    private void UnitSelectorManager_OnSelectionAreaEnd(object sender, EventArgs e) {
        selectionAreaRectTransform.gameObject.SetActive(false);
    }




    // Update is called once per frame
    private void Update() {

        if (selectionAreaRectTransform.gameObject.activeSelf) {
            UpdateVisual();
        }

    }

    private void UpdateVisual() {

        Rect selectionRectArea = UnitSelectorManager.Instance.GetSelectionRectArea();

        Vector2 lowerLeftCornerUI = selectionRectArea.min / canvas.scaleFactor;
        Vector2 upperRightUI = selectionRectArea.max / canvas.scaleFactor;

        selectionAreaRectTransform.anchoredPosition = lowerLeftCornerUI;
        selectionAreaRectTransform.sizeDelta = upperRightUI - lowerLeftCornerUI;
    }

}
