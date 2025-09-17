using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.UI
{
    public class SelectionPanel : MonoBehaviour
    {
        [SerializeField] private List<SelectionPanelModule> modules;

        private void Start()
        {
            Hide();
        }

        public void Show(List<ISelectable> targets)
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