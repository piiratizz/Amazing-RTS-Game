using System;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.Events;

public class ResourceSourceComponent : EntityComponent
{
    [SerializeField] private UnityEvent<AnimationHitArgs> onResourceHitted = new UnityEvent<AnimationHitArgs>();
    [SerializeField] private UnityEvent onResourceEnded = new UnityEvent();
    [SerializeField] private UnityEvent onResourceSelectedToGather = new UnityEvent();
    [SerializeField] private float timeToDestroy = 5;
    
    private ResourceType _resourceType;
    private int _baseAmount;
    private ReactiveProperty<int> _currentAmount;
    private ReactiveProperty<bool> _isEmpty;
    
    public ResourceType ResourceType => _resourceType;
    public int BaseAmount => _baseAmount;
    public ReadOnlyReactiveProperty<int> Amount => _currentAmount;
    public ReadOnlyReactiveProperty<bool> IsEmpty => _isEmpty.ToReadOnlyReactiveProperty();
    
    public override void InitializeFields(EntityConfig config)
    {
        var resourcesConfig = config as ResourcesConfig;

        if (resourcesConfig == null)
        {
            throw new NullReferenceException("ResourcesConfig must be of type ResourcesConfig");
        }
        
        _resourceType = resourcesConfig.Resource.ResourceType;
        _baseAmount = resourcesConfig.Amount;
        _currentAmount = new ReactiveProperty<int>(resourcesConfig.Amount);
        _isEmpty = new ReactiveProperty<bool>(false);
    }

    public void TakeHit(AnimationHitArgs args)
    {
        onResourceHitted.Invoke(args);
    }
    
    /// <summary>
    /// Returns amount that was collected, 0 = storage is empty
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    public int TryExtractResource(int amount)
    {
        if (amount >= _currentAmount.Value)
        {
            var current = _currentAmount.Value;
            _currentAmount.Value = 0;
            _isEmpty.Value = true;
            OnSourceEmpty().Forget(); 
            return current;
        }
        
        _currentAmount.Value -= amount;
        return amount;
    }

    public void SelectResourceToGather()
    {
        onResourceSelectedToGather.Invoke();
    }

    private async UniTask OnSourceEmpty()
    {
        onResourceEnded.Invoke();
        
        await UniTask.WaitForSeconds(timeToDestroy);
        
        Destroy(gameObject);
    }
}