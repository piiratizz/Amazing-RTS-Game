    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UpgradeView : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private GameObject upgradeInfoPanelBackground;
        [SerializeField] private TextMeshProUGUI upgradeNameText;
        [SerializeField] private TextMeshProUGUI upgradeDescriptionText;
        [SerializeField] private PointerHandler pointerHandler;
        
        public void Initialize(EntityUpgrade upgrade)
        {
            image.sprite = upgrade.Icon;
            upgradeNameText.text = upgrade.DisplayName;
            upgradeDescriptionText.text = upgrade.Description;

            HideInfoPanel();
        }

        private void ShowInfoPanel()
        {
            upgradeInfoPanelBackground.SetActive(true);
        }

        private void HideInfoPanel()
        {
            upgradeInfoPanelBackground.SetActive(false);
        }
        
        private void OnEnable()
        {
            pointerHandler.PointerEnterEvent += ShowInfoPanel;
            pointerHandler.PointerExitEvent += HideInfoPanel;
        }

        private void OnDisable()
        {
            pointerHandler.PointerEnterEvent -= ShowInfoPanel;
            pointerHandler.PointerExitEvent -= HideInfoPanel;
        }
    }