using System;
using Data;
using Enums;
using UnityEngine;

namespace Interfaces
{
    /// <summary>
    /// Interface for accessing and managing player's data.
    /// </summary>
    public interface IPlayerDataService
    {
        Sprite CurrentCharacterIcon(PlayerNumber player);
        event Action<PlayerNumber, CharacterSkinData> OnCharacterSkinChanged;
        event Action<PlayerNumber, BallSkinData> OnBallSkinChanged;
        
        Material GetBallMaterial(PlayerNumber player);
        GameObject GetCharacterPrefab(PlayerNumber player);
        CharacterSkinData GetCharacterSkinData(PlayerNumber player);
        BallSkinData GetBallSkinData(PlayerNumber player);
        void SetBallSkin(PlayerNumber player, BallSkinData ballSkin);
        void SetCharacterSkin(PlayerNumber player, CharacterSkinData ballSkin);
    }
}