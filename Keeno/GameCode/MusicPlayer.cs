using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Keeno.GameCode
{
    /// <summary>
    /// Class in charge of Playing, Pausing, Resuming Music.
    /// Mostly used to keep Game1 cleaner.
    /// </summary>
    class MusicPlayer
    {
        private SoundEffectInstance _dayForest, _nightForest;
        private Song _mainTheme, _firstRain;
        public MusicPlayer() 
        {
                // Didn't end up using the "ambient sounds".\\

            //_dayForest = Assets.ForestDay.CreateInstance();
            //_dayForest.IsLooped = true;
            //_nightForest = Assets.ForestNight.CreateInstance();
            //_nightForest.IsLooped = true;

            _mainTheme = Assets.MainThemeSFX;
            _firstRain = Assets.FirstRainSFX;
            MediaPlayer.IsRepeating = true;
        }
        /// <summary>
        /// Main Theme is the song on the Start screen.
        /// </summary>
        public void PlayMainTheme()
        {
            if (MediaPlayer.State != MediaState.Playing)
                MediaPlayer.Play(_mainTheme);
        }
        /// <summary>
        /// First Rain is the name of the song In Game.
        /// </summary>
        public void PlayFirstRain()
        {
            if (MediaPlayer.State != MediaState.Playing)
                MediaPlayer.Play(_firstRain);
        }
        public void ResumeMusic()
        {
            if (MediaPlayer.State == MediaState.Paused)
                MediaPlayer.Resume();
        }
        public void PauseMusic()
        {
            if (MediaPlayer.State == MediaState.Playing)
                MediaPlayer.Pause();
        }
    }
}
