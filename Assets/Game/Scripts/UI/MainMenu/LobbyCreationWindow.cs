using System.Collections.Generic;
using System.Linq;
using Game.Scripts.GlobalSystems;
using Game.Scripts.GlobalSystems.SceneManagement;
using Game.Scripts.Settings;
using TMPro;
using UnityEngine;
using Zenject;

namespace MainMenu
{
    public class LobbyCreationWindow : WindowBase
    {
        [SerializeField] private List<ButtonsToWindowsTypeBinding> buttonsToWindowsTypeBindings;
        [SerializeField] private List<LobbyPlayerView> lobbyPlayers;
        
        [Inject] private MatchSettingsManager _matchSettingsManager;
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
                        binding.Button.onClick.AddListener(() =>
                        {
                            _matchSettingsManager.Settings.MatchPlayers[0].Color = (PlayerColor)lobbyPlayers[0].DropdownValue;
                            _matchSettingsManager.Settings.MatchPlayers[1].Color = (PlayerColor)lobbyPlayers[1].DropdownValue;
                            _sceneManager.SwitchScene(Scenes.Gameplay).Forget();
                        });
                        break;
                }
            }
        }
    }
}