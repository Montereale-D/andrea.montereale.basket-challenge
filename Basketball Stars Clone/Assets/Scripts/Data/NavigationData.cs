using UI.Enums;

namespace Data
{
    /// <summary>
    /// Represents data passed when navigating to a UI screen.
    /// </summary>
    public struct NavigationData
    {
        public UIScreenID ScreenID;
        public object Data;
    }
}