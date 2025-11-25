using Game.Scripts.GlobalSystems.SceneManagement;
using Game.Scripts.Signals;
using TMPro;
using UnityEngine;
using Zenject;

namespace Game.Scripts.UI.Gameplay.MatchResultPopup
{
    public class MatchResultPopup : UIModule
    {
        [SerializeField] private TextMeshProUGUI winText;
        [SerializeField] private TextMeshProUGUI loseText;
        [SerializeField] private GameObject background;
        [SerializeField] private GameObject resultWindow;
        
        [Inject] private SignalBus _signalBus;
        [Inject] private SceneManager _sceneManager;
        
        private int _ownerId;
        
        public override void Initialize(int ownerId)
        {
            _ownerId = ownerId;
            _signalBus.Subscribe<PlayerWinSignal>(Show);
            
            winText.gameObject.SetActive(false);
            loseText.gameObject.SetActive(false);
            background.SetActive(false);
            resultWindow.SetActive(false);
        }

        public void Show(PlayerWinSignal signal)
        {
            if (signal.OwnerId == _ownerId)
            {
                winText.gameObject.SetActive(true);
            }
            else
            {
                loseText.gameObject.SetActive(true);
            }
            
            background.SetActive(true);
            resultWindow.SetActive(true);
        }

        public void GoToMainMenu()
        {
            _sceneManager.SwitchScene(Scenes.MainMenu).Forget();
        }
    }
}