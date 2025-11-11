using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;

public class BuildingBuildComponent : EntityComponent
{
    [SerializeField] private List<GameObject> buildingStagesObjects = new List<GameObject>();
    
    private HealthComponent _healthComponent;
    private int _currentBuildingIndex;
    
    private ReactiveProperty<bool> _isBuilded = new ReactiveProperty<bool>();
    
    public ReadOnlyReactiveProperty<bool> IsBuilded => _isBuilded;
    public bool IsFullHp => _healthComponent.CurrentHealth == _healthComponent.MaxHealth;
    
    public override void Init(Entity entity)
    {
        _healthComponent = entity.GetEntityComponent<HealthComponent>();
        HideAll();
        UpdateStageView();
    }

    public override void InitializeFields(EntityConfig config)
    {
        var buildingConfig = config as  BuildingConfig;

        if (buildingConfig == null)
        {
            throw new System.Exception("BuildingConfig is null");
        }

        if (buildingConfig.SpawnHealth == buildingConfig.MaxHealth)
        {
            _isBuilded.Value = true;
        }
    }

    public void AddBuildProgress(int progress)
    {
        _healthComponent.ApplyHealing(progress);

        if (IsFullHp && _isBuilded.CurrentValue == false)
        {
            _isBuilded.Value = true;
        }
        
        UpdateStageView();
    }
    
    private void UpdateStageView()
    {
        float normalizedProgress = (float)_healthComponent.CurrentHealth /  _healthComponent.MaxHealth;
        int prefabIndex = (int)((buildingStagesObjects.Count-1) * normalizedProgress);
        
        buildingStagesObjects[_currentBuildingIndex].SetActive(false);
        _currentBuildingIndex = prefabIndex;
        
        buildingStagesObjects[prefabIndex].SetActive(true);
    }

    private void HideAll()
    {
        foreach (GameObject obj in buildingStagesObjects)
        {
            obj.SetActive(false);
        }
    }
}