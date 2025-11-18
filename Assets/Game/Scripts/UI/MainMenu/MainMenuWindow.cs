using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MainMenu
{
    public class MainMenuWindow : WindowBase
    {
        [SerializeField] private List<ButtonsToWindowsTypeBinding> buttonsToWindowsTypeBindings;

        private WindowsManager _manager;
        
        public override void Initialize(MainMenuHUD hud)
        {
            _manager = hud.GetModule<WindowsManager>();
            
            foreach (var binding in buttonsToWindowsTypeBindings)
            {
                switch (binding.ButtonActionType)
                {
                    case ButtonActionType.WindowOpen:
                        binding.Button.onClick.AddListener(() => _manager.OpenWindow(binding.WindowType));
                        break;
                    case ButtonActionType.CloseApplication:
                        Application.Quit();
                        break;
                }
            }
        }
    }

    [Serializable]
    public class ButtonsToWindowsTypeBinding
    {
        public Button Button;
        public WindowType WindowType;
        public ButtonActionType ButtonActionType;
    }
}