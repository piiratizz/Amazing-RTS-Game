using System;
using Game.Scripts.GlobalSystems;
using R3;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI
{
    public class FpsCounter : MonoBehaviour
    {
        [SerializeField] private GameObject view;
        
        [Inject] private SaveLoadSystem _saveLoadSystem;

        private void Start()
        {
            _saveLoadSystem.Settings.ShowFps.Subscribe(ShowFps);
        }

        private void ShowFps(bool show)
        {
            view.SetActive(show);
        }
    }
}