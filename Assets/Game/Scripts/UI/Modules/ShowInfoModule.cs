using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Modules
{
    public class ShowInfoModule : SelectionPanelModule
    {
        [SerializeField] private UnitInfoPanelUI unitInfoPanelUIPrefab;
        [SerializeField] private Transform unitInfoPanelUIContainer;
        [SerializeField] private GameObject background;

        private readonly Dictionary<string, UnitInfoPanelUI> _instances = new Dictionary<string, UnitInfoPanelUI>();
        
        private readonly Dictionary<string, ISelectable> _groupedEntities = new Dictionary<string, ISelectable>(10);
        private readonly Dictionary<string, int> _entitiesCount = new Dictionary<string, int>(10);
        
        public override void Show(List<ISelectable> targets)
        {
            background.SetActive(true);

            _groupedEntities.Clear();
            _entitiesCount.Clear();
            
            foreach (var target in targets)
            {
                if (!_groupedEntities.ContainsKey(target.DisplayName))
                {
                    _entitiesCount.Add(target.DisplayName, 1);
                    _groupedEntities.Add(target.DisplayName, target);
                }
                else
                {
                    _entitiesCount[target.DisplayName]++;
                }
            }
            
            foreach (var entity in _groupedEntities)
            {
                if (!_instances.ContainsKey(entity.Value.DisplayName))
                {
                    var instance = Instantiate(unitInfoPanelUIPrefab, unitInfoPanelUIContainer);
                    instance.Initialize(_entitiesCount[entity.Value.DisplayName], entity.Value.Icon, entity.Value.DisplayName);
                    _instances.Add(entity.Value.DisplayName, instance);
                }
                else
                {
                    var instance = _instances[entity.Value.DisplayName];
                    instance.gameObject.SetActive(true);
                    instance.UpdateCount(_entitiesCount[entity.Value.DisplayName]);
                }
            }
        }

        public override void Hide()
        {
            foreach (var instance in _instances.Values)
            {
                instance.gameObject.SetActive(false);
            }
            
            background.SetActive(false);
        }
    }
}