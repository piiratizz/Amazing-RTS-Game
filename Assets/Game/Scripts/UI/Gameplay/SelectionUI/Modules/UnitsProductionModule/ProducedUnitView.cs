using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NTC.Pool;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProducedUnitView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private ResourceCostView resourceCostPrefab;
    [SerializeField] private Image unitPreviewImage;
    [SerializeField] private GameObject statsPanelBackground;
    [SerializeField] private Transform resourceCostContainer;
    [SerializeField] private TextMeshProUGUI unitNameText;
    [SerializeField] private TextMeshProUGUI unitDamageText;
    [SerializeField] private TextMeshProUGUI unitArmorText;
    [SerializeField] private TextMeshProUGUI unitSpeedText;
    [SerializeField] private TextMeshProUGUI unitRangeText;
    [SerializeField] private Button produceButton;
    
    private readonly List<ResourceCostView> _resourceCostsInstances = new List<ResourceCostView>();
    private IReadOnlyCollection<ResourceCost> _resourceCosts;

    private ResourceData[] _resourcesData;
    
    private UnitConfig _unitConfig;
    private Action<UnitConfig> _onProduceButtonClickCallback;
    
    private void Start()
    {
        _resourcesData = Resources.LoadAll<ResourceData>("ResourcesData");
        produceButton.onClick = new Button.ButtonClickedEvent();
        produceButton.onClick.AddListener(OnProduceButtonClick);
    }

    public void Initialize(
        UnitConfig unitConfig,
        IReadOnlyCollection<ResourceCost> resourceCosts,
        Action<UnitConfig> onProduceButtonClickCallback)
    {
        _unitConfig = unitConfig;
        unitPreviewImage.sprite = unitConfig.Icon;
        unitNameText.text = unitConfig.DisplayName;
        unitDamageText.text = unitConfig.Damage.ToString();
        unitArmorText.text = unitConfig.Armor.ToString();
        unitSpeedText.text = ((int)unitConfig.Speed).ToString();
        unitRangeText.text = ((int)unitConfig.AttackRange).ToString();
        _resourceCosts = resourceCosts;
        _onProduceButtonClickCallback = onProduceButtonClickCallback;
        
        SetInfoVisibility(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetInfoVisibility(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetInfoVisibility(false);
    }

    private void SetInfoVisibility(bool visible)
    {
        statsPanelBackground.SetActive(visible);
        unitNameText.gameObject.SetActive(visible);
        unitDamageText.gameObject.SetActive(visible);
        unitArmorText.gameObject.SetActive(visible);
        unitSpeedText.gameObject.SetActive(visible);
        unitRangeText.gameObject.SetActive(visible);
        resourceCostContainer.gameObject.SetActive(visible);

        if (visible)
        {
            _resourceCostsInstances.Clear();
            foreach (var resourceCost in _resourceCosts)
            {
                var resourceData = _resourcesData.First(r => r.ResourceType == resourceCost.Resource);

                var instance = NightPool.Spawn(resourceCostPrefab, resourceCostContainer);
                instance.Initialize(resourceData.UiDisplayIcon, resourceCost.Amount);
                _resourceCostsInstances.Add(instance);
            }
        }
        else
        {
            foreach (var instance in _resourceCostsInstances)
            {
                NightPool.Despawn(instance);
            }
            _resourceCostsInstances.Clear();
        }
    }

    private void OnProduceButtonClick()
    {
        _onProduceButtonClickCallback?.Invoke(_unitConfig);
    }
}