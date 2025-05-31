using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keeno.GameCode
{
    class GameManager
    {
        private Map _map;
        private TimeManager _timeManager;
        private List<Keeno> _keenosInGame;
        private Player _player;
        private TextManager _textManager;
        public event Action TenKeenoMilestone, TwentyFiveKeenoMilestone, OneHundredKeenoMilestone, OneHundredKeenoMilestoneReset;
        private bool _tenKeenoMilestone, _twentyFiveKeenoMilestone, _oneHundredKeenoMilestone, _oneHundredKeenoMilestoneReset;
        public GameManager(Map map, TimeManager timeManager, List<Keeno> keenosInGame, Player player, TextManager textManager )
        {
            _map = map;
            _timeManager = timeManager;
            _keenosInGame = keenosInGame;
            _player = player;
            _textManager = textManager;

            _tenKeenoMilestone = true;
            _twentyFiveKeenoMilestone = true;
            _oneHundredKeenoMilestone = true;
        }
        public void TrackMilestones()
        {
            if(_keenosInGame.Count == 10 && _tenKeenoMilestone)
            {
                _tenKeenoMilestone = false;
                TenKeenoMilestone?.Invoke();
            }
            if (_keenosInGame.Count == 25 && _twentyFiveKeenoMilestone)
            {
                _twentyFiveKeenoMilestone = false;
                TwentyFiveKeenoMilestone?.Invoke();
            }
            if (_keenosInGame.Count == 100 && _oneHundredKeenoMilestone)
            {
                _oneHundredKeenoMilestone = false;
                OneHundredKeenoMilestone?.Invoke();
            }
            if (_keenosInGame.Count < 100 && !_oneHundredKeenoMilestone)
            {
                _oneHundredKeenoMilestone = true;
                OneHundredKeenoMilestoneReset?.Invoke();
            }
        }
        public void ResetAll()
        {
            _map.Reset();
            _timeManager.RestartDay();
            _player.Reset();
            _textManager.CompleteReset();

            for (int i = 0; i < _keenosInGame.Count; i++)
            {
                _keenosInGame[i].Die();
            }
            ResourceTracker.Reset();

            // Reset Milestones
            _tenKeenoMilestone = true;
            _twentyFiveKeenoMilestone = true;
            _oneHundredKeenoMilestone = true;
        }
        public void NextDay()
        {
            _map.ClearAllWorkers();
            _player.DayReset();
            _timeManager.RestartDay();
            for (int i = 0; i < _keenosInGame.Count; i++)
            {
                _keenosInGame[i].NewDay();
            }
        }
    }

}
