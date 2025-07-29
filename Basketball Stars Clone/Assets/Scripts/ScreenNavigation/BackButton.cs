using Interfaces;
using UnityEngine.Assertions;

namespace ScreenNavigation
{
    /// <summary>
    /// UI button that navigates back to the previous screen when clicked.
    /// Inherits from <see cref="UIScreenButton"/> and uses <see cref="UIScreenManager"/> to perform the navigation.
    /// </summary>
    public class BackButton : UIScreenButton
    {
        protected override void OnClick()
        {
            
            #if UNITY_EDITOR
            Assert.IsNotNull(UIScreenManager.Instance, $"{nameof(UIScreenManager)} is missing");
            #endif
            
            UIScreenManager.Instance.NavigateBack();
            ServiceLocator.SoundService.PlaySound(SoundType.UI_BACK);
        }
    }
}