using UnityEngine;

namespace Game.Scripts.AI
{
    public class AiEntitiesController : MonoBehaviour
    {
        [SerializeField] private LayerMask entitiesLayer;
        
        private AiContext _context;
        
        public void Initialize(AiContext ctx)
        {
            _context = ctx;
            var entities = Physics.OverlapSphere(ctx.AiBasePosition, 500, entitiesLayer);

            foreach (var entity in entities)
            {
                if (!entity.TryGetComponent(out Entity e)) continue;
                
                if (e.OwnerId == ctx.OwnerId || e.OwnerId == 0)
                {
                    RegisterMyEntity(e);
                    e.OnEntityDestroyed += DeleteMyEntity;
                }
                else
                {
                    RegisterEnemyEntity(e);
                    e.OnEntityDestroyed += DeleteEnemyEntity;
                }
            }
            
        }

        private void RegisterEnemyEntity(Entity entity)
        {
            switch (entity)
            {
                case BuildingEntity buildingEntity:
                    //_context.EnemyArmy.Add(buildingEntity);
                    break;
                case UnitEntity unitEntity:
                    _context.EnemyArmy.Add(unitEntity);
                    _context.EnemyArmyCost++;
                    break;
            }
        }

        private void DeleteEnemyEntity(Entity entity)
        {
            switch (entity)
            {
                case BuildingEntity buildingEntity:
                    //_context.EnemyArmy.Add(buildingEntity);
                    break;
                case UnitEntity unitEntity:
                    _context.EnemyArmy.Remove(unitEntity);
                    _context.EnemyArmyCost--;
                    break;
            }
        }
        
        private void DeleteMyEntity(Entity entity)
        {
            switch (entity)
            {
                case BuildingEntity buildingEntity:
                    _context.AiBuildings.Remove(buildingEntity);
                    break;
                case UnitEntity unitEntity:
                    _context.AiUnits.Remove(unitEntity);
                    _context.AiArmyCost--;
                    break;
                case ResourceEntity resourceEntity:
                    _context.ResourcesOnMap.Remove(resourceEntity);
                    break;
            }
        }
        
        public void RegisterMyEntity(Entity entity)
        {
            switch (entity)
            {
                case BuildingEntity buildingEntity:
                    _context.AiBuildings.Add(buildingEntity);
                    break;
                case UnitEntity unitEntity:
                    _context.AiUnits.Add(unitEntity);
                    _context.AiArmyCost++;
                    break;
                case ResourceEntity resourceEntity:
                    _context.ResourcesOnMap.Add(resourceEntity);
                    break;
            }
        }
    }
}