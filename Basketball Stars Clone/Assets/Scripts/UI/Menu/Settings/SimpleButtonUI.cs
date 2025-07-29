using Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu.Settings
{
    /// <summary>
    /// Class for simple actions request on a button click.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class SimpleButtonUI : MonoBehaviour
    {
        [SerializeField] private SoundType soundType;
        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(() => ServiceLocator.SoundService.PlaySound(soundType));
        }
    }
}