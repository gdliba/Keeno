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
        List<Keeno> _keenosInGame;
        private Player _player;
        public ResetManager(Map map, TimeManager timeManager, List<Keeno> keenosInGame, Player player)
        {
            _map = map;
            _timeManager = timeManager;
            _keenosInGame = keenosInGame;
            _player = player;
        }

        public void ResetAll()
        {
            _map.Reset();
            _timeManager.RestartDay();
            _player.Reset();


            for (int i = 0; i < _keenosInGame.Count; i++)
            {
                _keenosInGame[i].Die();
            }
            ResourceTracker.Reset();

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
