using System;
using System.Collections.Generic;
using Game.Scripts.UI;
using UnityEngine;
using Zenject;

public class GameplayHUD : MonoBehaviour
{
    [SerializeField] private List<UIModule> uiModules;
    [Inject] private Player _player;
    
    public TModule GetModule<TModule>() where TModule : UIModule
    {
        foreach (var module in uiModules)
        {
            if(module is TModule uiModule)
                return uiModule;
        }
        return null;
    }
    
    public void Start()
    {
        int id = _player.OwnerId;
        
        foreach (var uiModule in uiModules)
        {
            uiModule.Initialize(id);
        }
    }
}