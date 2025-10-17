using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceCostView : MonoBehaviour
{
    [SerializeField] private Image resourceImage;
    [SerializeField] private TextMeshProUGUI costText;

    public void Initialize(Sprite resourcePreview, int cost)
    {
        resourceImage.sprite = resourcePreview;
        costText.text = cost.ToString();
    }
}