using System.Collections.Generic;

namespace Game.Scripts.GlobalSystems
{
    public class WorldEntitiesRegistry
    {
        private Dictionary<int, List<Entity>> _entities = new Dictionary<int, List<Entity>>();
        
        public void Register(Entity entity)
        {
            if (!_entities.ContainsKey(entity.OwnerId))
            {
                _entities.Add(entity.OwnerId, new List<Entity>());
            }
            
            _entities[entity.OwnerId].Add(entity); 
        }
        
        public void UnRegister(Entity entity)
        {
            _entities[entity.OwnerId].Remove(entity);
        }

        public IReadOnlyCollection<Entity> GetEntities(int ownerId)
        {
            return _entities[ownerId].AsReadOnly();
        }
    }
}