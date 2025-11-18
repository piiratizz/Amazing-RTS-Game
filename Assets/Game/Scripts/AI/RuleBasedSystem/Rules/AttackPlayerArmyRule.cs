namespace Game.Scripts.AI.RuleBasedSystem.Rules
{
    public class AttackPlayerArmyRule : IRule
    {
        public RuleCategory Category => RuleCategory.Military;

        public bool IsValid(AiContext ctx)
        {
            if (ctx.EnemyArmyCost <= 0)
            {
                return false;
            }
            
            return ctx.EnemyArmyCost < ctx.AiArmyCost;
        }

        public float GetUtility(AiContext ctx)
        {
            return (float)ctx.EnemyArmyCost / ctx.AiArmyCost;
        }

        public void Perform(AiContext ctx)
        {
            foreach (var aiUnit in ctx.AiUnits)
            {
                if(aiUnit.UnitType == UnitType.Worker)
                    continue;
                
                aiUnit.GetEntityComponent<UnitCommandDispatcher>().ExecuteCommand(UnitCommandsType.Move, new MoveArgs()
                {
                    AllowAutoAttack = true,
                    Position = ctx.EnemyArmy[0].transform.position
                });
            }
        }

        public float Cooldown => 4;
        public float LastExecutionTime { get; set; }
    }
}