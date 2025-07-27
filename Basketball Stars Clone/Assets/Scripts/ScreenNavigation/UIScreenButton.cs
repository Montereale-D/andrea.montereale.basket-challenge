using UnityEngine;
using UnityEngine.UI;

namespace ScreenNavigation
{
    /// <summary>
    /// Base class for UI buttons that trigger screen navigation or other screen-related actions.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public abstract class UIScreenButton : MonoBehaviour
    {
        private Button _button;

        private void Start()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);
        }

        protected abstract void OnClick();
    }
}