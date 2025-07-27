using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace SceneNavigation
{
    /// <summary>
    /// Manages the initial scene loading at app start.
    /// </summary>
    [RequireComponent(typeof(CustomSceneLoader))]
    public class BootLoader : MonoBehaviour
    {
        [SerializeField] private List<SceneField> scenesToUnload;
        [SerializeField] private List<SceneField> scenesToLoadSingle;
        [SerializeField] private List<SceneField> scenesToLoadAdditive;
        
        private CustomSceneLoader _customSceneLoader;

        private void Awake()
        {
            _customSceneLoader = GetComponent<CustomSceneLoader>();
            
            #if UNITY_EDITOR
            Assert.IsTrue(scenesToLoadAdditive.Count > 0, $"scenesToLoadAdditive reference is missing on '{gameObject.name}'");
            Assert.IsTrue(scenesToLoadSingle.Count > 0, $"scenesToLoadSingle reference is missing on '{gameObject.name}'");
            #endif
        }

        private void Start()
        {
            StartCoroutine(LoadMethod());
        }

        private IEnumerator LoadMethod()
        {
            //jobs simulation
            yield return new WaitForSeconds(2);
            
            _customSceneLoader.ChangeScene(scenesToUnload, scenesToLoadAdditive, scenesToLoadSingle);
        }
    }
}