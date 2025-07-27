using UnityEngine;

namespace Data
{
    /// <summary>
    /// Abstract base class for all skin data types.
    /// Holds common fields like icon, model prefab and display name.
    /// </summary>
    public abstract class SkinData : ScriptableObject
    {
        public Sprite icon;
        public GameObject modelPrefab;
        public string displayName;
    }
}