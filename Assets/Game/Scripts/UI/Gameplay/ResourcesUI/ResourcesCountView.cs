using System;
using System.Runtime.InteropServices;
using GlobalResourceStorageSystem;
using TMPro;
using UnityEngine;
using Zenject;

public class ResourcesCountView : UIModule
{
    [SerializeField] private TextMeshProUGUI foodText;
    [SerializeField] private TextMeshProUGUI woodText;
    [SerializeField] private TextMeshProUGUI goldText;

    [Inject] private ResourcesStoragesManager _resourceStorages;
    private GlobalResourceStorage _globalResourceStorage;
    
    private bool _initialized;
    
    public override void Initialize(int ownerId)
    {
        _globalResourceStorage = _resourceStorages.Get(ownerId);
        
        foodText.text = _globalResourceStorage.GetResource(ResourceType.Food).ToString();
        woodText.text = _globalResourceStorage.GetResource(ResourceType.Wood).ToString();
        goldText.text = _globalResourceStorage.GetResource(ResourceType.Gold).ToString();
        
        _globalResourceStorage.OnResourceChanged += OnResourceChanged;
        _initialized = true;
    }

    private void OnResourceChanged(ResourceType resourceType, int currentAmount)
    {
        UpdateResourceCount(resourceType, currentAmount);
    }

    private void UpdateResourceCount(ResourceType resourceType, int currentAmount)
    {
        switch (resourceType)
        {
            case ResourceType.Food:
                foodText.text = currentAmount.ToString();
                break;
            case ResourceType.Wood:
                woodText.text = currentAmount.ToString();
                break;
            case ResourceType.Gold:
                goldText.text = currentAmount.ToString();
                break;
        }
    }
    
    private void OnEnable()
    {
        if(!_initialized) return;
        _globalResourceStorage.OnResourceChanged += OnResourceChanged;
    }

    private void OnDisable()
    {
        _globalResourceStorage.OnResourceChanged -= OnResourceChanged;
    }
}