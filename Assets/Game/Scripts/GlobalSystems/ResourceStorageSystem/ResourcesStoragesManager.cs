using System.Collections.Generic;
using UnityEngine;

namespace GlobalResourceStorageSystem
{
    public class ResourcesStoragesManager
    {
        private Dictionary<int, GlobalResourceStorage> _storages = new Dictionary<int, GlobalResourceStorage>();
    
        private StorageRegistration _storageRegistration;

        public ResourcesStoragesManager()
        {
            _storageRegistration = new StorageRegistration(_storages);
        }
        
        public StorageRegistration Register()
        {
            return _storageRegistration;
        }

        public GlobalResourceStorage Get(int playerId)
        {
            return _storages[playerId];
        }
    }

    public class StorageRegistration
    {
        private Dictionary<int, GlobalResourceStorage> _storages;
        
        public StorageRegistration(Dictionary<int, GlobalResourceStorage> storages)
        {
            _storages = storages;
        }
        
        public GlobalResourceStorage FromNew(int playerId)
        {
            var instance = new GlobalResourceStorage();
            _storages.Add(playerId, instance);

            return instance;
        }

        public void FromInstance(int playerId, GlobalResourceStorage storage)
        {
            _storages.Add(playerId, storage);
        }
    }
}
