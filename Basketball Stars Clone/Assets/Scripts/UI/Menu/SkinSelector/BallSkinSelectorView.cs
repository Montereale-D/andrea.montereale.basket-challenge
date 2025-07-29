using Data;
using Enums;
using Interfaces;

namespace UI.Menu.SkinSelector
{
    /// <summary>
    /// Specializes SkinSelectorView<BallSkinData/> to drive ball-skin selection
    /// Inherits from <see cref="SkinSelectorView{T}"/>.
    /// </summary>
    public class BallSkinSelectorView : SkinSelectorView<BallSkinData>
    {
        private IPlayerDataService _dataService;
        private ISoundService _soundService;
        
        private void Awake()
        {
            GetSpriteForItem = skinData => skinData.Icon;
            OnSelectionChanged += UpdatePlayerBallSkin;
            OnSelectionChanged += UpdateEnemyBallSkin;
        }

        protected override void Start()
        {
            _dataService = ServiceLocator.PlayerDataService;
            _soundService = ServiceLocator.SoundService;
            base.Start();
        }

        private void UpdatePlayerBallSkin(BallSkinData ballSkinData)
        {
            _dataService.SetBallSkin(PlayerNumber.Player1, ballSkinData);
            _soundService.PlaySound(SoundType.UI_SELECT);
        }

        private void UpdateEnemyBallSkin(BallSkinData ballSkinData)
        {
            int nextIndex = (Navigator.CurrentIndex + 1) % items.Count;
            var enemySkin = items[nextIndex];
            _dataService.SetBallSkin(PlayerNumber.Player2, enemySkin);
        }
        
        protected override int GetStartIndex()
        {
            var current = _dataService.GetBallSkinData(PlayerNumber.Player1);
            int idx = items.IndexOf(current);
            return idx >= 0 ? idx : 0;
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            OnSelectionChanged -= UpdatePlayerBallSkin;
            OnSelectionChanged -= UpdateEnemyBallSkin;
        }
    }
}