using System.Collections.Generic;
using Game.Scripts.GlobalSystems;
using Game.Scripts.GlobalSystems.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace MainMenu
{
    public class SettingsWindow : WindowBase
    {
        [SerializeField] private List<ButtonsToWindowsTypeBinding> buttonsToWindowsTypeBindings;
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private TMP_Dropdown graphicsDropdown;
        [SerializeField] private Toggle fpsToggle;
        
        [Inject] private SceneManager _sceneManager;
        [Inject] private SaveLoadSystem _saveLoadSystem;
        private WindowsManager _windowsManager;
        
        public override void Initialize(MainMenuHUD hud)
        {
            _windowsManager = hud.GetModule<WindowsManager>();
            
            foreach (var binding in buttonsToWindowsTypeBindings)
            {
                switch (binding.ButtonActionType)
                {
                    case ButtonActionType.WindowOpen:
                        binding.Button.onClick.AddListener(() =>
                        {
                            _windowsManager.OpenWindow(binding.WindowType);
                            _saveLoadSystem.Settings.ShowFps.Value = fpsToggle.isOn;
                            _saveLoadSystem.Settings.MasterVolume.Value = masterVolumeSlider.value;
                            _saveLoadSystem.Settings.MusicVolume.Value = musicVolumeSlider.value;
                            _saveLoadSystem.Settings.GraphicsQuality.Value = graphicsDropdown.value;
                            _saveLoadSystem.Save();
                        });
                        break;
                }
            }
        }

        public override void Open()
        {
            base.Open();
            masterVolumeSlider.value = _saveLoadSystem.Settings.MasterVolume.CurrentValue;
            musicVolumeSlider.value = _saveLoadSystem.Settings.MusicVolume.CurrentValue;
            graphicsDropdown.value = _saveLoadSystem.Settings.GraphicsQuality.CurrentValue;
            fpsToggle.isOn = _saveLoadSystem.Settings.ShowFps.CurrentValue;
        }
    }
}