using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace SceneNavigation
{
    /// <summary>
    /// UI component that triggers a scene transition when the attached button is clicked.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class SceneLoadRequestButton : MonoBehaviour
    {
        [SerializeField] private List<SceneField> scenesToUnload;
        [SerializeField] private List<SceneField> scenesToLoadSingle;
        [SerializeField] private List<SceneField> scenesToLoadAdditive;
        [SerializeField] private CustomSceneLoader sceneLoader;
        
        private Button _button;

        private void Start()
        {
            #if UNITY_EDITOR
            Assert.IsNotNull(sceneLoader, $"{nameof(CustomSceneLoader)} reference is missing on '{gameObject.name}'");
            #endif
            
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            sceneLoader.ChangeScene(scenesToUnload, scenesToLoadAdditive, scenesToLoadSingle);
        }
    }
}