using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDetectionComponent : EntityComponent
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private bool isLongRange;
    [SerializeField] private float delayBetweenIterations = 0.3f;
    
    public Entity ClosestEnemy { get; private set; }
    
    private Entity _entity;
    private float _detectionRadius;

    private Stack<Entity> _potentialTargets;
    private Collider[] _cachedHitsColliders;
    
    private Coroutine _getClosestUnitCoroutine;
    
    public override void Init(Entity entity)
    {
        _entity = entity;
        _cachedHitsColliders = new Collider[100];
        _potentialTargets = new Stack<Entity>(20);
        
        _getClosestUnitCoroutine = StartCoroutine(GetClosestUnitCoroutine());
    }

    public override void InitializeFields(EntityConfig config)
    {
        _detectionRadius = config.DetectionRadius;
    }


    private IEnumerator GetClosestUnitCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(delayBetweenIterations);
            
            int spottedCount = Physics.OverlapSphereNonAlloc(transform.position, _detectionRadius, _cachedHitsColliders, layerMask);
            Entity closest = null;
            float minDist = float.MaxValue;

            for (var i = 0; i < spottedCount; i++)
            {
                var entity = _cachedHitsColliders[i].GetComponent<Entity>();

                if (entity == null)
                {
                    continue;
                }
                
                if (entity.GetEntityComponent<HealthComponent>().IsDead)
                {
                    continue;
                }
                
                if (entity.OwnerId == _entity.OwnerId)
                {
                    continue;
                }
                
                float dist = (entity.transform.position - transform.position).sqrMagnitude;
            
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = entity;
                    _potentialTargets.Push(closest);
                }
            }

            if (isLongRange && _potentialTargets.Count > 1)
            {
                ClosestEnemy = _potentialTargets.Pop();;
                continue;
            }
            else if(isLongRange && _potentialTargets.Count <= 1)
            {
                ClosestEnemy = closest;
            }

            if (ClosestEnemy is UnitEntity unitEntity)
            {
                if (unitEntity.AttackersCount > 3 && _potentialTargets.Count > 1)
                {
                    _potentialTargets.Pop();
                    ClosestEnemy = _potentialTargets.Pop();
                }
                else
                {
                    ClosestEnemy = closest;
                }
            }
           
        }
    }
    
    public override void OnKillComponent()
    {
        if (_getClosestUnitCoroutine != null)
        {
            StopCoroutine(_getClosestUnitCoroutine);
        }
    }
}