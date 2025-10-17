using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProducedUnitView : MonoBehaviour, IPointerEnterHandler,  IPointerExitHandler
{
    [SerializeField] private Image unitPreviewImage;
    [SerializeField] private GameObject statsPanelBackground;
    [SerializeField] private TextMeshProUGUI unitNameText;
    [SerializeField] private TextMeshProUGUI unitDamageText;
    [SerializeField] private TextMeshProUGUI unitArmorText;
    [SerializeField] private TextMeshProUGUI unitSpeedText;
    [SerializeField] private TextMeshProUGUI unitRangeText;

    public void Initialize(Sprite preview, string unitName, int unitDamage, int unitArmor, float unitSpeed, float unitRange)
    {
        unitPreviewImage.sprite = preview;
        unitNameText.text = unitName;
        unitDamageText.text = unitDamage.ToString();
        unitArmorText.text = unitArmor.ToString();
        unitSpeedText.text = ((int)unitSpeed).ToString();
        unitRangeText.text = ((int)unitRange).ToString();

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
    }
}