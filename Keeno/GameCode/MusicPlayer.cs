using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Keeno.GameCode
{

    class MusicPlayer
    {
        private SoundEffectInstance _dayForest, _nightForest;
        private Song _mainTheme, _firstRain;
        public MusicPlayer() 
        {
            _dayForest = Assets.ForestDay.CreateInstance();
            _dayForest.IsLooped = true;
            _nightForest = Assets.ForestNight.CreateInstance();
            _nightForest.IsLooped = true;

            _mainTheme = Assets.MainThemeSFX;
            _firstRain = Assets.FirstRainSFX;
            MediaPlayer.IsRepeating = true;
        }

        public void PlayMainTheme()
        {
            if (MediaPlayer.State != MediaState.Playing)
                MediaPlayer.Play(_mainTheme);
        }
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
