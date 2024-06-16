using UnityEngine;

namespace ED.Additional.Components.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class FitInSafeArea : MonoBehaviour
    {
        private void Start() => Fit();
        private void OnRectTransformDimensionsChange() => Fit();

        private void Fit() {
            var rtr = GetComponent<RectTransform>();
            var safeArea = Screen.safeArea;
            var anchorMin = safeArea.position;
            var anchorMax = safeArea.position + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            rtr.anchorMin = anchorMin;
            rtr.anchorMax = anchorMax;
        }
    }
}