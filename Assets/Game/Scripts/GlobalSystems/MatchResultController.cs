using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Signals;
using Zenject;

namespace Game.Scripts.GlobalSystems
{
    public class MatchResultController
    {
        private Dictionary<int, BuildingEntity> _townhalls = new Dictionary<int, BuildingEntity>();

        private SignalBus _signalBus;
        
        public MatchResultController(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }
        
        public void RegisterTownhall(int ownerId, BuildingEntity townhall)
        {

            townhall.OnEntityDestroyed += OnTownHallDestroyed;
            _townhalls.Add(ownerId, townhall);
        }

        private void OnTownHallDestroyed(Entity townhall)
        {
            _townhalls.Remove(townhall.OwnerId);

            if (_townhalls.Count == 1)
            {
                _signalBus.Fire(new PlayerWinSignal()
                {
                    OwnerId = _townhalls.Keys.First(id => id != townhall.OwnerId),
                });
            }
        }
    }
}