using UI.Enums;
using UnityEngine;
using UnityEngine.Assertions;

namespace ScreenNavigation
{
    /// <summary>
    /// UI button that navigates to a specific screen when clicked.
    /// Inherits from <see cref="UIScreenButton"/> and uses <see cref="UIScreenManager"/> to perform the navigation.
    /// The target screen is specified via the <see cref="screenID"/> field.
    /// </summary>
    public class NavigationButton : UIScreenButton
    {
        [SerializeField] private UIScreenID screenID;

        protected override void OnClick()
        {
            #if UNITY_EDITOR
            Assert.IsNotNull(UIScreenManager.Instance, $"{nameof(UIScreenManager)} is missing");
            #endif
            
            UIScreenManager.Instance.NavigateToScreen(screenID);
        }
    }
}