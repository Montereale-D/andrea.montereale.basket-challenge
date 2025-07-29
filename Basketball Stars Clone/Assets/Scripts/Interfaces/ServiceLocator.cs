namespace Interfaces
{
    /// <summary>
    /// Static service locator used to access core game services from anywhere in the project.
    /// Provides centralized access to services.
    /// </summary>
    public static class ServiceLocator
    {
        public static IPlayerDataService PlayerDataService { get; set; }
        public static IGameDataService GameDataService { get; set; }
        public static IScoreDataService ScoreDataService { get; set; }
        public static ISoundService SoundService { get; set; }
    }
}