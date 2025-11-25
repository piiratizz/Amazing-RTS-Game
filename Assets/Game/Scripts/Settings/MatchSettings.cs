using System.Collections.Generic;

namespace Game.Scripts.Settings
{
    public class MatchSettings
    {
        public List<MatchPlayerData> MatchPlayers;
        public string MapName;
    }

    public class MatchPlayerData
    {
        public int OwnerId;
        public bool IsAI;
        public PlayerColor Color;
    }
    
}