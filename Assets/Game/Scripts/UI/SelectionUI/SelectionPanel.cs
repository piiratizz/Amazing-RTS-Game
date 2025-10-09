using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI
{
    public class SelectionPanel : MonoBehaviour
    {
        [SerializeField] private List<SelectionPanelModule> modules;
        [Inject] private Player _player;
        
        private void Start()
        {
            Hide();
        }

        private void OnEnable()
        {
            _player.PlayerSelectionManager.OnSelectionChanged += OnSelectionChanged;
        }
        
        private void OnDisable()
        {
            _player.PlayerSelectionManager.OnSelectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged(List<Entity> targets)
        {
            Hide();
            
            if(targets.Count == 0) return;
            
            Show(targets);
        }
        
        public void Show(List<Entity> targets)
        {
            foreach (var module in modules)
                module.Show(targets);
        }

        public void Hide()
        {
            foreach (var module in modules)
                module.Hide();
        }
    }
}