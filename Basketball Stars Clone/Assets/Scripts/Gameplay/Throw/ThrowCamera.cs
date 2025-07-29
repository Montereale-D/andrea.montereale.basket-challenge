using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace Gameplay.Throw
{
    /// <summary>
    /// Controls the camera movement and orientation for the throw.
    /// </summary>
    public class ThrowCamera : MonoBehaviour
    {
        [SerializeField] private Transform lookTarget;
        [SerializeField] private Transform endPositionTarget;
        [SerializeField] private float movementDuration = 1.5f;
        [SerializeField] private AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private float verticalStrength = 1f;
        [SerializeField] private float horizontalStrength = 1f;
        [SerializeField] private float offsetFromBall;

        private void Awake()
        {
            #if UNITY_EDITOR
            Assert.IsNotNull(lookTarget, $"{nameof(Transform)} reference is missing on '{gameObject.name}'");
            Assert.IsNotNull(endPositionTarget, $"{nameof(Transform)} reference is missing on '{gameObject.name}'");
            Assert.IsNotNull(easeCurve, $"{nameof(AnimationCurve)} reference is missing on '{gameObject.name}'");
            #endif
        }

        public void ResetCameraTransform(Vector3 ballPosition)
        {
            StopAllCoroutines();
        
            Vector3 targetAdj = new Vector3(lookTarget.position.x, ballPosition.y, lookTarget.position.z);
            Vector3 directionToTarget = (targetAdj - ballPosition).normalized;
            transform.position = ballPosition + offsetFromBall * directionToTarget + (Vector3.up * 0.2f);
            transform.LookAt(lookTarget.position);
        }

        public void ApplyMovement()
        {
            StopAllCoroutines();
            StartCoroutine(ParabolicMovement(transform.position, endPositionTarget.position, movementDuration));
        }

        private IEnumerator ParabolicMovement(Vector3 start, Vector3 end, float duration)
        {
            float elapsed = 0f;

            Vector3 direction = (end - start).normalized;
            float totalDistance = Vector3.Distance(start, end);
            Vector3 horizontalDir = new Vector3(direction.x, 0f, direction.z).normalized;
        
            Quaternion startRotation = transform.rotation;
            Quaternion endRotation = endPositionTarget.rotation;

            while (elapsed < duration)
            {
                float t = elapsed / duration;
                float easedT = easeCurve.Evaluate(t);

                float horizontalOffset = easedT * totalDistance * horizontalStrength;
                float height = Mathf.Sin(easedT * Mathf.PI) * verticalStrength;

                Vector3 horizontalMove = start + horizontalDir * horizontalOffset;
                transform.position = new Vector3(horizontalMove.x, start.y + height, horizontalMove.z);
                transform.rotation = Quaternion.Slerp(startRotation, endRotation, easedT);

                elapsed += Time.deltaTime;
                yield return null;
            }
        }
    }
}