using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ProductionUpgradeView : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Button button;
    
    private EntityUpgrade _upgrade;

    public void Initialize(EntityUpgrade upgrade, UnityAction<EntityUpgrade> callback)
    {
        _upgrade = upgrade;
        icon.sprite = upgrade.Icon;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => callback(_upgrade));
    }
}