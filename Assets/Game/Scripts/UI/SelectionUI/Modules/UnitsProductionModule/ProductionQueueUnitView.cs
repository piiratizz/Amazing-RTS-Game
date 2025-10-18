using UnityEngine;
using UnityEngine.UI;

public class ProductionQueueUnitView : MonoBehaviour
{
    [SerializeField] private Image previewImage;

    public void Initialize(Sprite icon)
    {
        previewImage.sprite = icon;
    }
}