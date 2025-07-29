using UnityEngine;

namespace Data
{
    /// <summary>
    /// Abstract base class for all skin data types.
    /// Holds common fields like icon, model prefab and display name.
    /// </summary>
    public abstract class SkinData : ScriptableObject
    {
        [SerializeField] private Sprite icon;
        [SerializeField] private GameObject modelPrefab;
        [SerializeField] private string displayName;
        
        public Sprite Icon => icon;
        public GameObject ModelPrefab => modelPrefab;
        public string DisplayName => displayName;
    }
}