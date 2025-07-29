using Enums;

namespace Interfaces
{
    /// <summary>
    /// Interface for accessing and managing player scoring logic.
    /// </summary>
    public interface IScoreDataService
    {
        int GetPlayerScore(PlayerNumber player);
        public int ActiveBonus { get; }
        void ThrowResult(PlayerNumber playerNumber, TargetType lastTargetType, bool result);
    }
}