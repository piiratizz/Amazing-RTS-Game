using UnityEngine;

namespace Game.Scripts.GlobalSystems
{
    public class SaveLoadSystem
    {
        public GameSettingsProxy Settings;

        private const string SettingsKey = "GameSettings";

        public SaveLoadSystem()
        {
            Load();
        }

        public void Save()
        {
            var json = JsonUtility.ToJson(Settings.Data, true);
            PlayerPrefs.SetString(SettingsKey, json);
        }

        public void Load()
        {
            if (PlayerPrefs.HasKey(SettingsKey))
            {
                var settings = JsonUtility.FromJson<GameSettings>(PlayerPrefs.GetString(SettingsKey));
                Settings = new GameSettingsProxy(settings);
                return;
            }

            Settings = new GameSettingsProxy(new GameSettings()
            {
                MasterVolume = 0.4f,
                MusicVolume = 0.4f,
                GraphicsQuality = 2,
                ShowFps = false
            });
        }
    }
}