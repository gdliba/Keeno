using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keeno.GameCode
{
    class ResetManager
    {
        private Map _map;
        private TimeManager _timeManager;
        private List<Keeno> _keenosInGame;
        private Player _player;
        private TextManager _textManager;
        public event Action TenKeenoMilestone, TwentyFiveKeenoMilestone;
        private bool _tenKeenoMilestone, _twentyFiveKeenoMilestone;
        public ResetManager(Map map, TimeManager timeManager, List<Keeno> keenosInGame, Player player, TextManager textManager )
        {
            _map = map;
            _timeManager = timeManager;
            _keenosInGame = keenosInGame;
            _player = player;
            _textManager = textManager;

            _tenKeenoMilestone = true;
            _twentyFiveKeenoMilestone= true;
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
        }
        public void NextDay()
        {
            _map.ClearAllWorkers();
            _player.Reset();
            _timeManager.RestartDay();
            for (int i = 0; i < _keenosInGame.Count; i++)
            {
                _keenosInGame[i].NewDay();
            }
        }
    }

}
