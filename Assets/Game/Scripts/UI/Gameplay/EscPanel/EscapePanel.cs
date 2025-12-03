using System;
using Game.Scripts.GlobalSystems.SceneManagement;
using MainMenu;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.UI.Gameplay.EscPanel
{
    public class EscapePanel : UIModule
    {
        [SerializeField] private Button continueBtn;
        [SerializeField] private Button settingsBtn;
        [SerializeField] private Button quitBtn;
        [SerializeField] private InputActionAsset inputActionAsset;
        [SerializeField] private GameObject view;
        [SerializeField] private SettingsWindow settingsWindow;
        
        [Inject] private SceneManager _sceneManager;
        
        private void OnEnable()
        {
            inputActionAsset.FindAction("Escape").performed += ToggleView;
        }

        public override void Initialize(int ownerId)
        {
            view.SetActive(false);
            settingsWindow.Close();
            settingsWindow.Initialize(null);
            
            continueBtn.onClick.AddListener(() =>
            {
                ToggleView(default);
            });
            settingsBtn.onClick.AddListener(() =>
            {
                settingsWindow.Open();
            });
            quitBtn.onClick.AddListener(() => _sceneManager.SwitchScene(Scenes.MainMenu).Forget());
        }

        private void ToggleView(InputAction.CallbackContext obj)
        {
            settingsWindow.Close();
            view.SetActive(!view.activeSelf);
        }
        
        private void OnDisable()
        {
            inputActionAsset.FindAction("Escape").performed -= ToggleView;
        }
    }
}