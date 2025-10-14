using System;
using System.Collections.Generic;
using System.Globalization;
using Game.Scripts.UI.Modules.Presenters;
using R3;
using Zenject;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Modules
{
    public class ShowInfoModule : SelectionPanelModule
    {
        [SerializeField] private UnitInfoPanelView unitInfoPanelViewPrefab;
        [SerializeField] private Transform unitInfoPanelUIContainer;
        [SerializeField] private GameObject background;
        
        [SerializeField] private Slider hpSlider;
        [SerializeField] private TextMeshProUGUI hpText;
        
        [SerializeField] private TextMeshProUGUI unitNameText;
        [SerializeField] private Image unitIconImage;

        [SerializeField] private GameObject statsContainer;
        [SerializeField] private TextMeshProUGUI attackStatsText;
        [SerializeField] private TextMeshProUGUI armorStatsText;
        [SerializeField] private TextMeshProUGUI speedStatsText;
        [SerializeField] private TextMeshProUGUI rangeStatsText;
        
        [Inject] private Player _player;
        
        private Dictionary<Type, IEntityInfoPresenter> _presenters;
        private IEntityInfoPresenter _currentPresenter;

        public override void Initialize()
        {
            _presenters = new Dictionary<Type, IEntityInfoPresenter>()
            {
                {
                    typeof(BuildingEntity), new BuildingsInfoPresenter(
                        hpSlider,
                        hpText,
                        unitNameText,
                        unitIconImage)
                },
                {
                    typeof(ResourceEntity), new ResourceInfoPresenter(
                        hpSlider,
                        hpText,
                        unitNameText,
                        unitIconImage)
                },
                {
                    typeof(UnitEntity), new UnitsInfoPresenter(
                        unitInfoPanelViewPrefab,
                        unitInfoPanelUIContainer,
                        hpSlider,
                        hpText,
                        unitNameText,
                        unitIconImage,
                        statsContainer,
                        attackStatsText,
                        armorStatsText,
                        speedStatsText,
                        rangeStatsText)
                }
            };

            foreach (var presenter in _presenters.Values)
            {
                presenter.Hide();
            }
        }

        public override void Show(List<Entity> targets)
        {
            _player.PlayerSelectionManager.OnSelectionChanged += UpdateView;
            UpdateView(targets);
            _currentPresenter.Show(targets);
        }
        
        private void UpdateView(List<Entity> targets)
        {
            if(targets.Count == 0) return;
            
            background.SetActive(true);

            bool isUnit = false;
            
            foreach (var target in targets)
            {
                if (target.EntityType == EntityType.Unit)
                {
                    isUnit = true;
                }
            }

            if (isUnit)
            {
                _currentPresenter = _presenters[typeof(UnitEntity)];
                return;
            }
            
            if(targets.Count == 1)
            {
                if (targets[0] is ResourceEntity resourceEntity)
                {
                    _currentPresenter = _presenters[typeof(ResourceEntity)];
                }
                if (targets[0] is BuildingEntity buildingEntity)
                {
                    _currentPresenter = _presenters[typeof(BuildingEntity)];
                }
            }
        }
        
        public override void Hide()
        {
            background.SetActive(false);
            _currentPresenter?.Hide();
        }
    }
}