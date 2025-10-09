using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitInfoPanelView : MonoBehaviour
{
    [SerializeField] private Image iconRenderer;
    [SerializeField] private TextMeshProUGUI countText;
    
    public void Initialize(int count, Sprite icon)
    {
        countText.text = count.ToString();
        iconRenderer.sprite = icon;
    }

    public void UpdateCount(int count)
    {
        countText.text = count.ToString();
    }
}