using System;
using System.Collections.Generic;
using Game.Scripts.GlobalSystems;
using Game.Scripts.GlobalSystems.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace MainMenu
{
    public class MainMenuWindow : WindowBase
    {
        [SerializeField] private List<ButtonsToWindowsTypeBinding> buttonsToWindowsTypeBindings;

        [Inject] private SceneManager _sceneManager;
        private WindowsManager _windowsManager;
        
        public override void Initialize(MainMenuHUD hud)
        {
            _windowsManager = hud.GetModule<WindowsManager>();
            
            foreach (var binding in buttonsToWindowsTypeBindings)
            {
                switch (binding.ButtonActionType)
                {
                    case ButtonActionType.WindowOpen:
                        binding.Button.onClick.AddListener(() => _windowsManager.OpenWindow(binding.WindowType));
                        break;
                    case ButtonActionType.CloseApplication:
                        binding.Button.onClick.AddListener(() => Application.Quit());
                        break;
                    case ButtonActionType.SwitchScene:
                        binding.Button.onClick.AddListener(() => _sceneManager.SwitchScene(Scenes.Gameplay).Forget());
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