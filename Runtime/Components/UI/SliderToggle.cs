using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EwigeDreamer.Additional.Components.UI
{
    public class SliderToggle : Scrollbar, IPointerClickHandler, IEndDragHandler
    {
        private UnityEvent<bool> onSwitch = new();
        public UnityEvent<bool> OnSwitch => onSwitch;

        private bool isTween;
        private bool isOn;

        public bool IsOn
        {
            get => isOn;
            set
            {
                if (value == isOn) return;
                isOn = value;
                isTween = true;
                onSwitch.Invoke(value);
            }
        }

        public void SetIsOn(bool value, bool forced)
        {
            if (value == isOn) return;
            if (forced)
            {
                isOn = value;
                isTween = false;
                base.value = value ? 1f : 0f;
                onSwitch.Invoke(value);
            }
            else
            {
                IsOn = value;
            }
        }

        protected override void Update()
        {
            base.Update();

            if (isTween)
            {
                var targetValue = isOn ? 1f : 0f;
                base.value = Mathf.Lerp(base.value, targetValue, Time.unscaledDeltaTime * 10f);
                if (Mathf.Approximately(base.value, targetValue))
                {
                    base.value = targetValue;
                    isTween = false;
                }
            }
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            IsOn = !IsOn;
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            IsOn = base.value > 0.5f;
        }

        public override void OnPointerDown(PointerEventData eventData) { }

        public override void OnInitializePotentialDrag(PointerEventData eventData) { }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            isTween = false;
            base.OnBeginDrag(eventData);
        }
    }
}