using System;
using System.Collections;
using Interfaces;
using UnityEngine;

namespace Gameplay.Basketball
{
    /// <summary>
    /// Detects collisions and triggers related to the basketball to determine scoring outcomes.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(BasketBall))]
    public class BallHitDetector : MonoBehaviour
    {
        private BasketBall _basketball;
        private bool _ballInside;

        public Action<bool> OnBallResult;

        private void Awake()
        {
            _basketball = GetComponent<BasketBall>();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.collider.CompareTag("Plane"))
            {
                PlaneEnterHandler();
            }
            else
            {
                StartCoroutine(DelayedPlaneEnterHandler());
            }

            if (other.collider.CompareTag("Backboard"))
            {
                ServiceLocator.SoundService.PlaySound(SoundType.SFX_SCORE_NOPERFECT);
            }
        }
        
        private IEnumerator DelayedPlaneEnterHandler()
        {
            yield return new WaitForSeconds(1.5f);
            PlaneEnterHandler();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Net")) return;

            NetEnterHandler();
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Net")) return;

            NetExitHandler();
        }

        private void PlaneEnterHandler()
        {
            StopAllCoroutines();
            OnBallResult?.Invoke(_ballInside);
            ResetState();
        }

        private void NetEnterHandler()
        {
            StopAllCoroutines();
            _ballInside = _basketball.IsGoingDown();
        }

        private void NetExitHandler()
        {
            StopAllCoroutines();
            OnBallResult?.Invoke(_ballInside);
            ResetState();
        }

        private void ResetState()
        {
            _ballInside = false;
        }
    }
}