using UnityEngine;

namespace Gameplay.Basketball
{
    /// <summary>
    /// Controls the basketball's physics, appearance, and behavior during gameplay.
    /// </summary>
    [RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(MeshRenderer)), RequireComponent(typeof(TrailRenderer))]
    public class BasketBall : MonoBehaviour
    {
        private Rigidbody _ballRigidbody;
        private MeshRenderer _meshRenderer;
        private TrailRenderer _trailRenderer;

        private void Awake()
        {
            _ballRigidbody = GetComponent<Rigidbody>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _trailRenderer = GetComponent<TrailRenderer>();
            
            _ballRigidbody.isKinematic = true;
        }

        public void SetMeshMaterial(Material material)
        {
            _meshRenderer.material = material;
        }

        private void UnlockBall()
        {
            _ballRigidbody.isKinematic = false;
            _trailRenderer.enabled = true;
        }

        public void ThrowBall(Vector3 velocity)
        {
            UnlockBall();
            _ballRigidbody.velocity = velocity;
        }
        
        public void ResetBall(Vector3 position)
        {
            _ballRigidbody.isKinematic = true;
            
            _ballRigidbody.velocity = Vector3.zero;
            _ballRigidbody.angularVelocity = Vector3.zero;
            _ballRigidbody.rotation = Quaternion.identity;
            
            _ballRigidbody.transform.position = position;
            
            _trailRenderer.enabled = false;
        }

        public bool IsGoingDown() => _ballRigidbody.velocity.y < 0;
    }
}