using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;

public class GlobalUpgradesManager
{
    private Dictionary<int, PlayerUpgrades> _playerUpgrades = new();

    private SignalBus _signalBus;
    
    [Inject]
    public GlobalUpgradesManager(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }
    
    public void RegisterPlayer(int playerId)
    {
        _playerUpgrades.Add(playerId, new PlayerUpgrades());
    }

    public void AddUpgrade(int playerId, EntityUpgrade upgrade)
    {
        _playerUpgrades[playerId].Stats.Add(upgrade);
        
        _signalBus.Fire(new UpgradeAddedSignal()
        {
            PlayerId = playerId,
            Upgrade = upgrade
        });
    }
    
    public List<EntityUpgrade> Get(int playerId) => _playerUpgrades[playerId].Stats;

    public List<EntityUpgrade> GetByEntityType(int playerId, EntityType entityType)
    {
        return _playerUpgrades[playerId].Stats.Where(s => s.EntityType == entityType).ToList();
    }

}

class PlayerUpgrades
{
    public readonly List<EntityUpgrade> Stats = new List<EntityUpgrade>();
}