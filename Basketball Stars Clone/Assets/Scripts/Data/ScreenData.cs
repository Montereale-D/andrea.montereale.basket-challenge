using System;
using Enums;
using UnityEngine;

namespace Data
{
    /// <summary>
    /// Represents a UI screen.
    /// </summary>
    [Serializable]
    public struct ScreenData
    {
        public UIScreenID ScreenID;
        public GameObject Screen;
    }
}