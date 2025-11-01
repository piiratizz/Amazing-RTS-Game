namespace ComponentsActionTypes
{
    public interface IAttackable
    {
        bool IsCanAutoAttack { get; set; }
        bool IsAttacking { get; set; }
        void SetAutoAttack(bool status);
        void AttackEntity(Entity entity);
    }
}