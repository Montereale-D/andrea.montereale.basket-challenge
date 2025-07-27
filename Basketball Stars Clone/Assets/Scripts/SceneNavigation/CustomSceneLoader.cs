using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SceneNavigation
{
    /// <summary>
    /// Handles scene transitions (single and additive) with support for loading and unloading,
    /// displaying a loading bar and fade in/out effect.
    /// </summary>
    public class CustomSceneLoader : MonoBehaviour
    {
        [SerializeField, Tooltip("Optional loading bar")] private Slider loadingBar;
        [SerializeField] private FadeInOut fadeInOut;
    
        [SerializeField] private float fadeInDuration = 2f;
        [SerializeField] private float fadeOutDuration = 2f;
    
        private readonly List<AsyncOperation> _loadOperations = new();
        private void Start()
        {
            #if UNITY_EDITOR
            Assert.IsNotNull(fadeInOut, $"{nameof(FadeInOut)} reference is missing on '{gameObject.name}'");
            #endif
            
            StartCoroutine(fadeInOut.FadeInCoroutine(fadeInDuration));
        }
    
        public void ChangeScene(List<SceneField> scenesToUnload, List<SceneField> scenesToLoadAdditive, List<SceneField> scenesToLoadSingle = null)
        {
            StartCoroutine(OnSceneOut(scenesToUnload, scenesToLoadAdditive,scenesToLoadSingle));
        }
    
        private IEnumerator OnSceneOut(List<SceneField> scenesToUnload, List<SceneField> scenesToLoadAdditive, List<SceneField> scenesToLoadSingle)
        {
            yield return null;
        
            LoadScenes(scenesToLoadSingle, LoadSceneMode.Single);
            LoadScenes(scenesToLoadAdditive, LoadSceneMode.Additive);

            if (loadingBar)
            {
                yield return StartCoroutine(ProgressLoading());
            }
        
            yield return fadeInOut.FadeOutCoroutine(fadeOutDuration);
        
            foreach (var operation in _loadOperations)
            {
                operation.allowSceneActivation = true;
            }
        
            yield return new WaitUntil(() => _loadOperations.TrueForAll(op => op.isDone));

            foreach (var scene in scenesToUnload)
            {
                SceneManager.UnloadSceneAsync(scene);
            }
        }
        
        private void LoadScenes(List<SceneField> scenes, LoadSceneMode mode)
        {
            foreach (var scene in scenes)
            {
                AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scene, mode);
                if (asyncOperation != null)
                {
                    asyncOperation.allowSceneActivation = false;
                    _loadOperations.Add(asyncOperation);
                }
            }
        }
        
        private IEnumerator ProgressLoading()
        {
            float progress = 0f;
        
            while (progress < 0.9f)
            {
                progress = 0;
                foreach (var scene in _loadOperations)
                {
                    progress += Mathf.Clamp01(scene.progress / 0.9f);
                }
            
                loadingBar.value = progress;
            
                yield return null;
            }
        }
    }
}