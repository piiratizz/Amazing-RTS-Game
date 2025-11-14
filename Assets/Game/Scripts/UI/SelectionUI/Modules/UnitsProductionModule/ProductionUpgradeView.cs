using System.Collections.Generic;
using System.Linq;
using NTC.Pool;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProductionUpgradeView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private Transform resourceCostContainer;
    [SerializeField] private TextMeshProUGUI upgradeNameText;
    [SerializeField] private TextMeshProUGUI upgradeDescriptionText;
    [SerializeField] private ResourceCostView resourceCostViewPrefab;
    [SerializeField] private Button button;
    
    private EntityUpgrade _upgrade;

    private ResourceData[] _resourcesData;
    private List<ResourceCostView> _resourcesViewInstances = new List<ResourceCostView>();
    
    private void Start()
    {
        _resourcesData = Resources.LoadAll<ResourceData>("ResourcesData");
    }
    
    public void Initialize(EntityUpgrade upgrade, UnityAction<EntityUpgrade> callback)
    {
        Clear();
        infoPanel.SetActive(false);
        
        _upgrade = upgrade;
        icon.sprite = upgrade.Icon;
        upgradeNameText.text = upgrade.DisplayName;
        upgradeDescriptionText.text = upgrade.Description;
        
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => callback(_upgrade));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        infoPanel.SetActive(true);
        if (_resourcesViewInstances.Count != 0)
        {
            return;
        }

        foreach (var cost in _upgrade.ResourceCost)
        {
            var icon = _resourcesData.First(r => r.ResourceType == cost.Resource).UiDisplayIcon;
            var costInstance = NightPool.Spawn(resourceCostViewPrefab, resourceCostContainer);
            _resourcesViewInstances.Add(costInstance);
            costInstance.Initialize(icon, cost.Amount);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        infoPanel.SetActive(false);
    }

    private void Clear()
    {
        foreach (var instance in _resourcesViewInstances)
        {
            NightPool.Despawn(instance);
        }
        _resourcesViewInstances.Clear();
    }
}