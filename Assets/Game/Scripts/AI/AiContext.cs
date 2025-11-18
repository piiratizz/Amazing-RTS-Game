using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.AI
{
    public class AiContext
    {
        public AiPlayer AiPlayer;
        public int OwnerId;
        
        public int Food;
        public int Wood;
        public int Gold;

        public int Workers;
        public int MaxWorkers;
        public int FoodWorkers;
        public int WoodWorkers;
        public int GoldWorkers;
        public int LazyWorkers;

        public int AiArmyCost;
        public int EnemyArmyCost;
        public List<UnitEntity> EnemyArmy = new List<UnitEntity>();

        public bool EnemyNearBase;

        public Vector3 EnemyBasePosition;
        public bool EnemyBaseKnown;
        
        public Vector3 AiBasePosition;
        
        public List<BuildingEntity> AiBuildings = new List<BuildingEntity>();
        public List<UnitEntity> AiUnits = new List<UnitEntity>();
        public List<ResourceEntity> ResourcesOnMap = new List<ResourceEntity>();
        
        //public IAIPlayerController Player;
        //public SquadManager SquadManager;
    }
}