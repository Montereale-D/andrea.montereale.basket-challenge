using System.Collections;
using Enums;
using Events;
using TMPro;
using UnityEngine;

namespace UI.Gameplay
{
    /// <summary>
    /// Controller class that display an animated message on the player screen
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI), typeof(CanvasGroup), typeof(RectTransform))]
    public class ScoreNotificationController : MonoBehaviour
    {
        [SerializeField, Tooltip("Duration of the floating message in seconds")] private float duration = 1f;
        [SerializeField, Tooltip("Vertical movement offset over duration")] private float verticalMovement = 50f;
        
        private TextMeshProUGUI _textGUI;
        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        
        private Vector2 _initialPosition;

        private void Awake()
        {
            _textGUI = GetComponent<TextMeshProUGUI>();
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();

            _initialPosition = _rectTransform.anchoredPosition;
        }
        
        private void OnEnable()
        {
            EventBus.Subscribe<ScoreChangedEvent>(OnScoreChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<ScoreChangedEvent>(OnScoreChanged);
        }
        
        private void OnScoreChanged(ScoreChangedEvent args)
        {
            string message = args.Target switch
            {
                TargetType.Perfect => "Perfect ",
                TargetType.Backboard => "Backboard ",
                _ => ""
            };
            
            ShowMessage(message + "+"+ args.Score);
        }

        private void ShowMessage(string message)
        {
            StopAllCoroutines();
            _textGUI.text = message;
            _rectTransform.anchoredPosition = _initialPosition;
            _canvasGroup.alpha = 1f;
            StartCoroutine(AnimateMessage());
        }

        private IEnumerator AnimateMessage()
        {
            float elapsed = 0f;
            Vector2 start = _initialPosition;
            Vector2 end = start + Vector2.up * verticalMovement;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                _rectTransform.anchoredPosition = Vector2.Lerp(start, end, t);
                _canvasGroup.alpha = 1f - t;
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            _rectTransform.anchoredPosition = end;
            _canvasGroup.alpha = 0f;
        }
    }
}