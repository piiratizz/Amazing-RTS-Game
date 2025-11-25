using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.GlobalSystems;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Map
{
    public class PlayersBasesSpawner : MonoBehaviour
    {
        [SerializeField] private BuildingTypePrefabLink townhallPrefabLink;
        [SerializeField] private ConfigUnitPrefabLink workerPrefabLink;
        [SerializeField] private Map map;
        
        [Inject] private BuildingFactory _buildingFactory;
        [Inject] private UnitFactory _unitFactory;
        [Inject] private MatchResultController _matchResultsController;
        
        private int _playersCount;
        private List<int> _ownerIds;
        private List<Player> _players;
        
        public void Initialize(int playersCount, List<int> ownerIds, List<Player> players)
        {
            _playersCount = playersCount;
            _ownerIds = ownerIds;
            _players = players;

            foreach (var player in players)
            {
                player.ClampMovementInWorld(map.SizeX, map.SizeY);
            }
        }

        private void Start()
        {
            for (int i = 0; i < _playersCount; i++)
            {
                var townhallInstance = _buildingFactory.Create(
                    _ownerIds[i],
                    map.PlayersSpawnPositions.ElementAt(i).position,
                    townhallPrefabLink);
                
                _matchResultsController.RegisterTownhall(_ownerIds[i], townhallInstance);
            }

            for (int i = 0; i < _players.Count; i++)
            {
                _players[i].transform.position = map.PlayersSpawnPositions.ElementAt(i).position;
            }

            for(int i = 0; i < _ownerIds.Count; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var spawnPos= map.PlayersSpawnPositions.ElementAt(i).position;
                    spawnPos.z -= 10;
                    spawnPos.x -= j;
                    _unitFactory.Create(_ownerIds[i], workerPrefabLink, spawnPos);
                }
            }
            
        }
    }
}