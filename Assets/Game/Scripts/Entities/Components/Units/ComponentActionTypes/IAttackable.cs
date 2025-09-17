namespace ComponentsActionTypes
{
    public interface IAttackable
    {
        void SetAutoAttack(bool status);
        void AttackEntity(Entity entity);
    }
}