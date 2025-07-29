namespace Interfaces
{
    /// <summary>
    /// Interface for managing sound-related functionalities, such as volume control and playback.
    /// </summary>
    public interface ISoundService
    {
        float GetVolume(AudioType type);
        void SetVolume(AudioType type, float volume);
        void PlaySound(SoundType type);
        void PlayRandomSound(SoundType type);
    }
    
    public enum AudioType
    {
        Music,
        Source
    }
    
    public enum SoundType
    {
        UI_CONFIRM,
        UI_BACK,
        UI_SELECT,
        UI_RECAP,
        SFX_SCORE_PERFECT,
        SFX_SCORE_NOPERFECT,
        SFX_FIREBALL,
        SFX_THROWBALL,
        SFX_BONUS
    }

    public enum MusicType
    {
        MENU,
        GAME
    }
}