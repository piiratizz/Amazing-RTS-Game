using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Scripts.AI
{
    public class RulesPerformer : MonoBehaviour
    {
        [System.Serializable]
        public class RuleCategoryConfig
        {
            public RuleCategory category;
            public float tickRate = 1f;

            [HideInInspector] public float timer;
        }

        [SerializeField] private List<RuleCategoryConfig> categoriesConfig;

        private AiContext _ctx;
        private List<IRule> _rules = new List<IRule>();

        public void Initialize(AiContext ctx, IEnumerable<IRule> allRules)
        {
            _ctx = ctx;
            _rules = allRules.ToList();
        }

        private void Update()
        {
            if (_ctx == null)
                return;

            foreach (var cat in categoriesConfig)
            {
                cat.timer += Time.deltaTime;

                if (cat.timer >= cat.tickRate)
                {
                    cat.timer = 0f;
                    EvaluateCategory(cat.category);
                }
            }
        }

        private void EvaluateCategory(RuleCategory category)
        {
            float bestUtility = float.MinValue;
            IRule bestRule = null;
            float currentTime = Time.time;

            foreach (var rule in _rules)
            {
                if (rule.Category != category)
                    continue;
                
                if (!rule.IsValid(_ctx))
                    continue;

                if (currentTime < rule.LastExecutionTime + rule.Cooldown)
                    continue;

                float util = rule.GetUtility(_ctx);

                
                
                if (util > bestUtility)
                {
                    bestUtility = util;
                    bestRule = rule;
                }
            }
            
            
            if (bestRule != null)
            {
                Debug.Log($"PERFORMING RULE {bestRule} : {bestUtility}");
                bestRule.Perform(_ctx);
                bestRule.LastExecutionTime = Time.time;
            }
        }
    }
}