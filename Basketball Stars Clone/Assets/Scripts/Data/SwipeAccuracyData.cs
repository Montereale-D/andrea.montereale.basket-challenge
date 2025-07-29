using System;
using UnityEngine;

namespace Data
{
    /// <summary>
    /// Represent the aiming difficulty for a target.
    /// </summary>
    [Serializable]
    public struct SwipeAccuracyData
    {
        [Range(0,1)] public float verticalPosition;
        [Range(10,100)] public float areaWidth;
    }
}