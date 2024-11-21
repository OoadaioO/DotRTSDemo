using System;
using UnityEngine;

public class UnitSelectionManagerUI : MonoBehaviour {

    [SerializeField] private RectTransform selectionAreaRectTransform;

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

        selectionAreaRectTransform.anchoredPosition = new Vector2(
            selectionRectArea.x,
            selectionRectArea.y
        );
        selectionAreaRectTransform.sizeDelta = new Vector2(selectionRectArea.width, selectionRectArea.height);
    }
}
