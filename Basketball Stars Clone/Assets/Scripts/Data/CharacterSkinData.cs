using Enums;
using UnityEngine;

namespace Data
{
    /// <summary>
    /// ScriptableObject that store all data related to a character appearance.
    /// Used in the customization system to represent the selected character appearance.
    /// </summary>
    [CreateAssetMenu(fileName = "CharacterSkin", menuName = "Skins/Character Skin")]
    public class CharacterSkinData : SkinData
    {
        public CharacterSkinType skinType;
    }
}