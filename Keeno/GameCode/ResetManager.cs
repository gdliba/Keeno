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
        public ResetManager(Map map, TimeManager timeManager, List<Keeno> keenosInGame)
        {
            _map = map;
            _timeManager = timeManager;
            _keenosInGame = keenosInGame;
        }

        public void ResetAll()
        {
            _map.Reset();
            _timeManager.RestartDay();

            for (int i = 0; i < _keenosInGame.Count; i++)
            {
                _keenosInGame[i].Die();
            }
            ResourceTracker.Reset();

        }
    }

}
