using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UnitInfoPanelView : MonoBehaviour
{
    [SerializeField] private Image iconRenderer;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Button button;
    
    private string _entityName;
    
    public void Initialize(string name, int count, Sprite icon, UnityAction<string> onClick)
    {
        _entityName = name;
        countText.text = count.ToString();
        iconRenderer.sprite = icon;
        button.onClick.AddListener(() => onClick(_entityName));
    }

    public void UpdateCount(int count)
    {
        countText.text = count.ToString();
    }
}