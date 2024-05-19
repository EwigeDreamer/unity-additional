using UnityEngine;
using UnityEngine.Events;

namespace EwigeDreamer.Additional.Components.UI
{
    [RequireComponent(typeof(SliderToggle))]
    public class SliderToggleEvents : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent onToggleOn = new();
        [SerializeField]
        private UnityEvent onToggleOff = new();

        public UnityEvent OnToggleOn => onToggleOn;
        public UnityEvent OnToggleOff => onToggleOff;

        private SliderToggle toggle;
		
        private void Awake()
        {
            toggle = GetComponent<SliderToggle>();
            toggle.OnSwitch.AddListener(OnToggle);
        }

        private void OnToggle(bool isOn)
        {
            if (isOn) onToggleOn.Invoke();
            else onToggleOff.Invoke();
        }

        private void OnDestroy()
        {
            toggle.OnSwitch.RemoveListener(OnToggle);
            toggle = null;
        }
    }
}