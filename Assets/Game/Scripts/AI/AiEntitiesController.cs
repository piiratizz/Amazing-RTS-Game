using Game.Scripts.GlobalSystems;
using UnityEngine;
using Zenject;
using System.Linq;

namespace Game.Scripts.AI
{
    public class AiEntitiesController : MonoBehaviour
    {
        [SerializeField] private float enemyNearBaseRadiusCheck;
        
        [Inject] private WorldEntitiesRegistry _entitiesRegistry;
        [Inject] private GlobalGrid _globalGrid; 
        
        
        private AiContext _context;
        private float _timer;

        [SerializeField] private float updateInterval = 0.2f; 
        
        private bool _initialized;

        private Entity _townhall;
        
        public void Initialize(AiContext ctx)
        {
            _context = ctx;
            _initialized = true;
        }

        private void Update()
        {
            if (!_initialized)
                return;

            _timer += Time.deltaTime;

            if (_timer >= updateInterval)
            {
                _timer = 0;
                RefreshContext();
            }
        }
        
        private void RefreshContext()
        {
            ClearOld();
            
            var myEntities = _entitiesRegistry.GetEntities(_context.OwnerId);

            foreach (var e in myEntities)
                RegisterMyEntity(e);

            if (_townhall == null)
            {
                _townhall = myEntities.First(e =>
                {
                    if (e is BuildingEntity b)
                    {
                        return b.BuildingType == BuildingType.Townhall;
                    }

                    return false;
                });
                
                _context.AiBasePosition = _townhall.transform.position;
            }
            
            int enemyId = 1;

            var enemyEntities = _entitiesRegistry.GetEntities(enemyId);

            foreach (var e in enemyEntities)
                RegisterEnemyEntity(e);
            
            foreach (var r in _entitiesRegistry.GetEntities(0))
                RegisterResource(r);

            _context.EnemyNearBase = IsEnemyNearBase();
            
        }
        
        private void ClearOld()
        {
            _context.AiUnits.Clear();
            _context.AiBuildings.Clear();
            _context.ResourcesOnMap.Clear();

            _context.EnemyArmy.Clear();
            _context.EnemyArmyCost = 0;

            _context.EnemyNearBase = false;
            
            _context.AiArmyCost = 0;
        }
        
        private void RegisterMyEntity(Entity e)
        {
            switch (e)
            {
                case UnitEntity unit:
                    _context.AiUnits.Add(unit);
                    if (unit.UnitType != UnitType.Worker)
                        _context.AiArmyCost += 1;
                    break;

                case BuildingEntity building:
                    _context.AiBuildings.Add(building);
                    break;

                case ResourceEntity res:
                    _context.ResourcesOnMap.Add(res);
                    break;
            }
        }
        
        private void RegisterEnemyEntity(Entity e)
        {
            switch (e)
            {
                case UnitEntity unit:
                    _context.EnemyArmy.Add(unit);
                    if (unit.UnitType != UnitType.Worker)
                        _context.EnemyArmyCost += 1;
                    break;

                case BuildingEntity building:
                    _context.EnemyBaseKnown = true;
                    _context.EnemyBasePosition = building.transform.position;
                    break;
            }
        }

        private void RegisterResource(Entity e)
        {
            if (e is ResourceEntity r)
                _context.ResourcesOnMap.Add(r);
        }
        
        private bool IsEnemyNearBase()
        {
            var heat = _globalGrid.HeatMap;
            
            heat.GetCell(_context.AiBasePosition, out int baseX, out int baseY);

            int cellsRadius = Mathf.CeilToInt(enemyNearBaseRadiusCheck / heat.CellSize);
            
            for (int x = baseX - cellsRadius; x <= baseX + cellsRadius; x++)
            {
                for (int y = baseY - cellsRadius; y <= baseY + cellsRadius; y++)
                {
                    if (!heat.IsInside(x, y))
                        continue;

                    int enemyHeat = heat.GetEnemyHeat(_context.OwnerId, x, y);

                    if (enemyHeat > 0)
                        return true;
                }
            }

            return false;
        }

    }
}
