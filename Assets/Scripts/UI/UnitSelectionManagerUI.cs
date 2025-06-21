using Assets.Scripts.Monobehaviour.UI;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.ECS.UI
{
    public class UnitSelectionManagerUI : MonoBehaviour {
    
        [SerializeField] private RectTransform selectionAreaRectTransform;
        [SerializeField] private Canvas canvas;

        public bool isECSScene = true;

        private void Start() {
            if (isECSScene)
            {
                UnitSelectionManager.Instance.OnSelectionAreaStart += UnitSelectionManager_OnSelectionAreaStart;
                UnitSelectionManager.Instance.OnSelectionAreaEnd += UnitSelectionManager_OnSelectionAreaEnd;
            }
            else
            {
                UnitSelectionManagerMono.Instance.OnSelectionAreaStart += UnitSelectionManager_OnSelectionAreaStart;
                UnitSelectionManagerMono.Instance.OnSelectionAreaEnd += UnitSelectionManager_OnSelectionAreaEnd;
            }

            selectionAreaRectTransform.gameObject.SetActive(false);
        }

        private void Update() {
            if (selectionAreaRectTransform.gameObject.activeSelf) {
                UpdateVisual();
            }
        }

        private void UnitSelectionManager_OnSelectionAreaStart(object sender, System.EventArgs e) {
            selectionAreaRectTransform.gameObject.SetActive(true);

            UpdateVisual();
        }

        private void UnitSelectionManager_OnSelectionAreaEnd(object sender, System.EventArgs e) {
            selectionAreaRectTransform.gameObject.SetActive(false);
        }

        private void UpdateVisual()
        {
            Rect selectionAreaRect;

            if (isECSScene)
                selectionAreaRect = UnitSelectionManager.Instance.GetSelectionAreaRect();
            else
                selectionAreaRect = UnitSelectionManagerMono.Instance.GetSelectionAreaRect();

            float canvasScale = canvas.transform.localScale.x;
            selectionAreaRectTransform.anchoredPosition = new Vector2(selectionAreaRect.x, selectionAreaRect.y) / canvasScale;
            selectionAreaRectTransform.sizeDelta = new Vector2(selectionAreaRect.width, selectionAreaRect.height) / canvasScale;
        }

    }
}