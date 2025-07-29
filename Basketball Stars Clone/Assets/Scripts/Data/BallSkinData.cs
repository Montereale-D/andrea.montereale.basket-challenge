using Enums;
using UnityEngine;

namespace Data
{
    /// <summary>
    /// ScriptableObject that store all data related to a ball appearance.
    /// Used in the customization system to represent the selected ball appearance.
    /// </summary>
    [CreateAssetMenu(fileName = "BallSkin", menuName = "Skins/Ball Skin")]
    public class BallSkinData : SkinData
    {
        [SerializeField] private  BallSkinType skinType;
        
        public BallSkinType SkinType => skinType;
    }
}