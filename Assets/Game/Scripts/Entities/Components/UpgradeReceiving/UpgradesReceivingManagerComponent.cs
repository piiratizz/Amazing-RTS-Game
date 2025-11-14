using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class UpgradesReceivingManagerComponent : EntityComponent
{
    private int _ownerId;
    private EntityType _entityType;
    private EntityConfig _config;

    private readonly Dictionary<Type, List<object>> _receiversByUpgradeType = new();
    
    [Inject] private GlobalUpgradesManager _globalUpgradesManager;
    [Inject] private SignalBus _signalBus;
    
    public override void LateInit(Entity entity)
    {
        _entityType = entity.EntityType;
        _ownerId = entity.OwnerId;
        _signalBus.Subscribe<UpgradeAddedSignal>(OnUpgradeAdded);

        foreach (var component in
                 entity.GetAllComponentsByInterface<IUpgradeReceiver<UnitStatsModifierUpgrade>>())
        {
            RegisterReceiver(component);
        }

        var upgrades = _globalUpgradesManager.GetByEntityType(_ownerId, _entityType);
        
        
        foreach (var upgrade in upgrades)
        {
            ApplyUpgrade(upgrade);
        }
    }

    public override void InitializeFields(EntityConfig config)
    {
        _config = config;
    }

    private void RegisterReceiver<T>(IUpgradeReceiver<T> receiver) where T : EntityUpgrade
    {
        var type = typeof(T);
        if (!_receiversByUpgradeType.TryGetValue(type, out var list))
            _receiversByUpgradeType[type] = list = new List<object>();

        list.Add(receiver);
    }
    
    private void OnUpgradeAdded(UpgradeAddedSignal signal)
    {
        if(signal.PlayerId != _ownerId) return;

        if(signal.Upgrade.EntityType != _entityType) return;
        
        ApplyUpgrade(signal.Upgrade);
    }

    private void ApplyUpgrade(EntityUpgrade upgrade)
    {
        Debug.Log("Applying stats modifier");
        
        if (upgrade is UnitStatsModifierUpgrade statsModifierUpgrade)
        {
            if (_config is not UnitConfig unitConfig) return;
            
            foreach (var unit in statsModifierUpgrade.UnitsWillUpgrade)
            {
                if (unit != unitConfig.UnitType) continue;
                foreach (IUpgradeReceiver<UnitStatsModifierUpgrade> receiver in _receiversByUpgradeType[typeof(UnitStatsModifierUpgrade)])
                {
                    receiver.ReceiveUpgrade(statsModifierUpgrade);
                }
            }
        }
    }
}