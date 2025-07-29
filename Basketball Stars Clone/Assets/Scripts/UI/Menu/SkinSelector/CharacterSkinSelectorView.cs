using Data;
using Enums;
using Interfaces;

namespace UI.Menu.SkinSelector
{
    /// <summary>
    /// Specializes SkinSelectorView<CharacterSkinData/> to drive ball-skin selection
    /// Inherits from <see cref="SkinSelectorView{T}"/>.
    /// </summary>
    public class CharacterSkinSelectorView : SkinSelectorView<CharacterSkinData>
    {
        private IPlayerDataService _dataService;
        private ISoundService _soundService;
        
        private void Awake()
        {
            GetSpriteForItem = skinData => skinData.Icon;
        }
        
        protected override void Start()
        {
            _dataService = ServiceLocator.PlayerDataService;
            _soundService = ServiceLocator.SoundService;
            
            _dataService.GetBallSkinData(PlayerNumber.Player1);
            base.Start();
            OnSelectionChanged += UpdatePlayerCharacterSkin;
            OnSelectionChanged += UpdateEnemyCharacterSkin;
        }

        private void UpdateEnemyCharacterSkin(CharacterSkinData characterSkinData)
        {
            _dataService.SetCharacterSkin(PlayerNumber.Player1, characterSkinData);
            _soundService.PlaySound(SoundType.UI_SELECT);
        }

        private void UpdatePlayerCharacterSkin(CharacterSkinData characterSkinData)
        {
            int nextIndex = (Navigator.CurrentIndex + 1) % items.Count;
            var enemySkin = items[nextIndex];
            _dataService.SetCharacterSkin(PlayerNumber.Player2, enemySkin);
        }
        
        protected override int GetStartIndex()
        {
            var current = _dataService.GetCharacterSkinData(PlayerNumber.Player1);
            int idx = items.IndexOf(current);
            return idx >= 0 ? idx : 0;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            OnSelectionChanged -= UpdatePlayerCharacterSkin;
            OnSelectionChanged -= UpdateEnemyCharacterSkin;
        }
    }
}