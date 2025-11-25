using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Map
{
    public class Map : MonoBehaviour
    {
        [SerializeField] private string mapName;
        [SerializeField] private List<Transform> playersSpawnPositions;
        [SerializeField] private PlayersBasesSpawner playersBasesSpawner;
        [SerializeField] private int sizeX;
        [SerializeField] private int sizeY;
        
        public int SizeX => sizeX;
        public int SizeY => sizeY;
        
        public PlayersBasesSpawner PlayersBasesSpawner => playersBasesSpawner;
        
        public string Name => mapName;
        public IReadOnlyCollection<Transform> PlayersSpawnPositions => playersSpawnPositions;
    }
}