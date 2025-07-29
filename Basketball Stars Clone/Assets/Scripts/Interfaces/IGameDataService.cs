using Enums;

namespace Interfaces
{
    /// <summary>
    /// Interface for accessing and managing game settings.
    /// </summary>
    public interface IGameDataService
    {
        float GameTimerDuration { get; }
        GameMode CurrentGameMode { get; }
        AiDifficulty AIDifficulty { get; }
        void SetTimerDuration(int modelCurrent);
        void SetGameMode(GameMode mode);
        void SetAIDifficulty(AiDifficulty difficulty);
    }
}