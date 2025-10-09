using ComponentsActionTypes;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovementComponent : EntityComponent, IMoveable
{
    [SerializeField] private NavMeshAgent navMeshAgent;
    private UnitAnimationComponent _unitAnimation;

    public override void Init(Entity entity)
    {
        _unitAnimation = entity.GetEntityComponent<UnitAnimationComponent>();
    }

    public override void InitializeFields(EntityConfig config)
    {
        var unitConfig = config as UnitConfig;
        navMeshAgent.speed = unitConfig.Speed;
        
        navMeshAgent.avoidancePriority = Random.Range(0, 100);
    }

    public override void OnUpdate()
    {
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
        if (navMeshAgent.enabled)
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.destination = position;
        }
    }

    public void StopMoving()
    {
        if (navMeshAgent.enabled)
        {
            navMeshAgent.isStopped = true;
            _unitAnimation.SetMove(false);
        }
    }

    public override void OnExit()
    {
        if (navMeshAgent.enabled)
        {
            navMeshAgent.destination = transform.position;
        }
    }

    public override void OnKillComponent()
    {
        navMeshAgent.enabled = false;
    }
}
