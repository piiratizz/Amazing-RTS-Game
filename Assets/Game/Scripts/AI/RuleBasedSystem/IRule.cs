namespace Game.Scripts.AI
{
    public interface IRule
    {
        RuleCategory Category { get; }
        
        bool IsValid(AiContext ctx);
        float GetUtility(AiContext ctx);
        void Perform(AiContext ctx);
        
        float Cooldown { get; }
        float LastExecutionTime { get; set; }
    }

    public enum RuleCategory
    {
        Economy, 
        Production, 
        Military, 
        Building,
    }
}