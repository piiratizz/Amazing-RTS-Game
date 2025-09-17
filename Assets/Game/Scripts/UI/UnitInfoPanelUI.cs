using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfoPanelUI : MonoBehaviour
{
    [SerializeField] private Image iconRenderer;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private TextMeshProUGUI nameText;
    
    public void Initialize(int count, Sprite icon, string name)
    {
        countText.text = count.ToString();
        iconRenderer.sprite = icon;
        nameText.text = name;
    }

    public void UpdateCount(int count)
    {
        countText.text = count.ToString();
    }
}