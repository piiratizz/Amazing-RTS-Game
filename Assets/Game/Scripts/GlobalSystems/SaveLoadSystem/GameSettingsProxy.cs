using R3;

namespace Game.Scripts.GlobalSystems
{
    public class GameSettingsProxy
    {
        public readonly GameSettings Data;
        
        public ReactiveProperty<float> MasterVolume;
        public ReactiveProperty<float> MusicVolume;
        public ReactiveProperty<int> GraphicsQuality;
        public ReactiveProperty<bool> ShowFps;

        public GameSettingsProxy(GameSettings data)
        {
            Data = data;

            MasterVolume     = new ReactiveProperty<float>(data.MasterVolume);
            MusicVolume      = new ReactiveProperty<float>(data.MusicVolume);
            GraphicsQuality  = new ReactiveProperty<int>(data.GraphicsQuality);
            ShowFps          = new ReactiveProperty<bool>(data.ShowFps);

            MasterVolume.Subscribe(v => Data.MasterVolume = v);
            MusicVolume.Subscribe(v => Data.MusicVolume = v);
            GraphicsQuality.Subscribe(v => Data.GraphicsQuality = v);
            ShowFps.Subscribe(v => Data.ShowFps = v);
        }
    }

}