using Game.Scripts.GlobalSystems;
using Zenject;

public class UnitWorkerRegistrar : EntityComponent
{
    [Inject] private GlobalWorkersObserver _workersObserver;
    
    private UnitEntity _entity;
    
    public override void Init(Entity entity)
    {
        if(entity is not UnitEntity unitEntity)
            return;
        
        _entity = unitEntity;
        _workersObserver.RegisterWorker(_entity);
    }

    public override void OnKillComponent()
    {
        _workersObserver.UnregisterWorker(_entity);
    }
}