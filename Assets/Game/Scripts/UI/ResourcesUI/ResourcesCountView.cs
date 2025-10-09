using System;
using TMPro;
using UnityEngine;
using Zenject;

public class ResourcesCountView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI foodText;
    [SerializeField] private TextMeshProUGUI woodText;
    [SerializeField] private TextMeshProUGUI goldText;

    [Inject] private GlobalResourceStorage _globalResourceStorage;
    
    private void Start()
    {
        foodText.text = _globalResourceStorage.GetResource(ResourceType.Food).ToString();
        woodText.text = _globalResourceStorage.GetResource(ResourceType.Wood).ToString();
        goldText.text = _globalResourceStorage.GetResource(ResourceType.Gold).ToString();
    }

    private void OnResourceChanged(ResourceType resourceType, int currentAmount)
    {
        switch (resourceType)
        {
            case ResourceType.Food:
                foodText.text = _globalResourceStorage.GetResource(ResourceType.Food).ToString();
                break;
            case ResourceType.Wood:
                woodText.text = _globalResourceStorage.GetResource(ResourceType.Wood).ToString();
                break;
            case ResourceType.Gold:
                goldText.text = _globalResourceStorage.GetResource(ResourceType.Gold).ToString();
                break;
        }
    }
    
    private void OnEnable()
    {
        _globalResourceStorage.OnResourceChanged += OnResourceChanged;
    }

    private void OnDisable()
    {
        _globalResourceStorage.OnResourceChanged -= OnResourceChanged;
    }
}