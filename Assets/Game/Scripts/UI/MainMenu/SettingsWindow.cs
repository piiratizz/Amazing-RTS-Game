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
            if (hud != null)
            {
                _windowsManager = hud.GetModule<WindowsManager>();
            }

            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            graphicsDropdown.onValueChanged.AddListener(OnGraphicsChanged);
            fpsToggle.onValueChanged.AddListener(OnFpsToggled);
            

            foreach (var binding in buttonsToWindowsTypeBindings)
            {
                switch (binding.ButtonActionType)
                {
                    case ButtonActionType.WindowOpen:
                        binding.Button.onClick.AddListener(() =>
                        {
                            if (_windowsManager != null)
                            {
                                _windowsManager.OpenWindow(binding.WindowType);
                            }
                            SaveData();
                        });
                        break;
                    case ButtonActionType.CloseWindow:
                        binding.Button.onClick.AddListener(Close);
                        SaveData();
                        break;
                }
            }
        }

        public void OnMasterVolumeChanged(float value)
        {
            _saveLoadSystem.Settings.MasterVolume.Value = masterVolumeSlider.value;
        }
        
        public void OnMusicVolumeChanged(float value)
        {
            _saveLoadSystem.Settings.MusicVolume.Value = musicVolumeSlider.value;
        }
        
        public void OnGraphicsChanged(int value)
        {
            _saveLoadSystem.Settings.GraphicsQuality.Value = graphicsDropdown.value;
        }
        
        public void OnFpsToggled(bool value)
        {
            _saveLoadSystem.Settings.ShowFps.Value = fpsToggle.isOn;
        }
        
        private void SaveData()
        {
            _saveLoadSystem.Save();
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