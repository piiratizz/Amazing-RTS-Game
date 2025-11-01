using ComponentsActionTypes;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovementComponent : EntityComponent, IMoveable
{
    [SerializeField] private NavMeshAgent navMeshAgent;
    private UnitAnimationComponent _unitAnimation;

    private bool _initialized;
    
    public Vector3 Velocity => navMeshAgent.velocity;
    
    public override void Init(Entity entity)
    {
        _unitAnimation = entity.GetEntityComponent<UnitAnimationComponent>();
        _initialized = true;
    }

    public override void InitializeFields(EntityConfig config)
    {
        var unitConfig = config as UnitConfig;
        navMeshAgent.speed = unitConfig.Speed;
        
        navMeshAgent.avoidancePriority = Random.Range(0, 100);
    }

    public override void OnUpdate()
    {
        if(!_initialized) return;
        
        if(_unitAnimation == null) return;
        
        if(!navMeshAgent.enabled) return;
        
        if(navMeshAgent.isStopped) return;
        
        if (transform.position != navMeshAgent.destination)
        {
            _unitAnimation.SetMove(true);
        }

        if (transform.position == navMeshAgent.destination)
        {
            _unitAnimation.SetMove(false);
        }
    }

    public bool IsMoving()
    {
        if (!navMeshAgent.enabled) return false;
        return navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance;
    }

    public void MoveTo(Vector3 position)
    {
        if(!_initialized) return;
        
        if (navMeshAgent.enabled)
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.destination = position;
        }
    }

    public void StopMoving()
    {
        if(!_initialized) return;
        
        if (navMeshAgent.enabled)
        {
            navMeshAgent.isStopped = true;
            _unitAnimation.SetMove(false);
        }
    }

    public override void OnExit()
    {
        if(!_initialized) return;
        
        if (navMeshAgent.enabled)
        {
            navMeshAgent.destination = transform.position;
        }
    }

    public override void OnKillComponent()
    {
        if(!_initialized) return;
        
        navMeshAgent.enabled = false;
    }
}
