using Data;
using UnityEngine;
using UnityEngine.Assertions;

namespace Gameplay.Throw
{
    /// <summary>
    /// Represents a throw spot in the game, containing positional data and targets
    /// for perfect and backboard shots.
    /// </summary>
    public class ThrowSpot : MonoBehaviour
    {
        [SerializeField] private ThrowSpotData throwSpotData;
        [SerializeField] private Transform perfectTarget;
        [SerializeField] private Transform backboardTarget;

        private void Awake()
        {
            #if UNITY_EDITOR
            Assert.IsNotNull(throwSpotData, $"{nameof(ThrowSpotData)} reference is missing on '{gameObject.name}'");
            Assert.IsNotNull(perfectTarget, $"{nameof(Transform)} reference is missing on '{gameObject.name}'");
            Assert.IsNotNull(backboardTarget, $"{nameof(Transform)} reference is missing on '{gameObject.name}'");
            #endif
        }

        public ThrowSpotData ThrowSpotData => throwSpotData;
        public Vector3 StartPos => transform.position;
        public Vector3 PerfectTarget => perfectTarget.position;
        public Vector3 BackboardTarget => backboardTarget.position;
    }
}
