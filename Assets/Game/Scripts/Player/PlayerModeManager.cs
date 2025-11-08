using System;
using R3;
using UnityEngine;
using Zenject;

public class PlayerModeManager : MonoBehaviour
{
    [Inject] private SignalBus _signalBus;
    
    private readonly ReactiveProperty<PlayerModes> _mode = new ReactiveProperty<PlayerModes>();
    
    public ReadOnlyReactiveProperty<PlayerModes> Mode => _mode;
    
    private void Start()
    {
        _signalBus.Subscribe<ChanglePlayerModeSignal>(register =>
        {
            SetMode(register.Mode);
        });
    }

    public void SetMode(PlayerModes mode)
    {
        _mode.Value = mode;
    }
}