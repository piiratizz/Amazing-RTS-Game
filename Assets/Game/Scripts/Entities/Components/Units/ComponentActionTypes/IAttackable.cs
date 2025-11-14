namespace ComponentsActionTypes
{
    public interface IAttackable
    {
        bool IsCanAutoAttack { get; set; }
        bool IsAttacking { get; set; }
        int BaseDamage { get; }
        int BonusDamage { get; }
        void SetAutoAttack(bool status);
        void AttackEntity(Entity entity);
    }
}