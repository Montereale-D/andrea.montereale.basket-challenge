using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SceneNavigation
{
    /// <summary>
    /// Handles fade-in and fade-out transitions using a UI Image component.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class FadeInOut : MonoBehaviour
    {
        private Image _fadeImage;
    
        private void Awake()
        {
            _fadeImage = GetComponent<Image>();
        }
    
        public IEnumerator FadeInCoroutine(float duration)
        {
            Color startColor = new Color(_fadeImage.color.r, _fadeImage.color.g, _fadeImage.color.b, 1f);
            Color targetColor = new Color(_fadeImage.color.r, _fadeImage.color.g, _fadeImage.color.b, 0f);
            _fadeImage.raycastTarget = true;
        
            yield return FadeCoroutine(startColor, targetColor, duration);
            gameObject.SetActive(false);
            _fadeImage.raycastTarget = false;
        }
    
        public IEnumerator FadeOutCoroutine(float duration)
        {
            Color startColor = new Color(_fadeImage.color.r, _fadeImage.color.g, _fadeImage.color.b, 0f);
            Color targetColor = new Color(_fadeImage.color.r, _fadeImage.color.g, _fadeImage.color.b, 1f);
            _fadeImage.raycastTarget = true;
        
            gameObject.SetActive(true);
            yield return FadeCoroutine(startColor, targetColor, duration);
        }

        private IEnumerator FadeCoroutine(Color startColor, Color endColor, float duration)
        {
            float elapsedTime = 0f;
            float elapsedPercentage = 0f;

            while (elapsedPercentage < 1)
            {
                elapsedPercentage = elapsedTime / duration;
            
                _fadeImage.color = Color.Lerp(startColor, endColor, elapsedPercentage);
            
                yield return null;
            
                elapsedTime += Time.deltaTime;
            }
        
            _fadeImage.color = endColor;
        }
    }
}