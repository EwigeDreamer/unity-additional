using UnityEngine;

namespace EwigeDreamer.Additional.Components.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class FitInParent : MonoBehaviour
    {
        private void Start() => Fit();
        private void OnRectTransformDimensionsChange() => Fit();
		
        private void Fit()
        {
            var tr = transform as RectTransform;
            var pr = tr!.parent as RectTransform;
			
            var parentSize = pr!.rect.size;
            var childSize = tr.rect.size;
            var ratios = childSize / parentSize;
            var maxRatio = Mathf.Max(ratios.x, ratios.y);
            tr.localScale = Vector3.one / maxRatio;
        }
    }
}