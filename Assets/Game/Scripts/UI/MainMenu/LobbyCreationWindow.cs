using System.Collections.Generic;
using Game.Scripts.GlobalSystems.SceneManagement;
using UnityEngine;
using Zenject;

namespace MainMenu
{
    public class LobbyCreationWindow : WindowBase
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
                    case ButtonActionType.SwitchScene:
                        binding.Button.onClick.AddListener(() => _sceneManager.SwitchScene(Scenes.Gameplay).Forget());
                        break;
                }
            }
        }
    }
}