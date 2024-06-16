using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ED.Additional.Components.UI
{
    public class MultiClickButton : Button
    {
        private float _lastTap;
        private int _tapCount;
        public int RequiredTapCount = 3;
        public float ResetTime = 0.5f;

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (UnityEngine.Time.unscaledTime - _lastTap > ResetTime)
            {
                _tapCount = 0;
            }

            _lastTap = UnityEngine.Time.unscaledTime;
            _tapCount++;

            if (_tapCount == RequiredTapCount)
            {
                base.OnPointerClick(eventData);
                _tapCount = 0;
            }
        }
    }
}