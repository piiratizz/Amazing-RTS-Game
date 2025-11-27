using System.Collections.Generic;
using Game.Scripts.Settings;
using UnityEngine;

namespace Game.Scripts.GlobalSystems
{
    public class MatchSettingsManager : MonoBehaviour
    {
        private MatchSettings _settings;

        public MatchSettings Settings => _settings;
        
        public void Initialize()
        {
            _settings = new MatchSettings()
            {
                MapName = "Small Forest",
                MatchPlayers = new List<MatchPlayerData>()
                {
                    new MatchPlayerData()
                    {
                        OwnerId = 1,
                        Color = PlayerColor.Yellow,
                        IsAI = false
                    },
                    new MatchPlayerData()
                    {
                        OwnerId = 2,
                        Color = PlayerColor.Green,
                        IsAI = true
                    },
                }
            };
        }
        
        public void Initialize(MatchSettings settings)
        {
            _settings = settings;
        }
    }
}