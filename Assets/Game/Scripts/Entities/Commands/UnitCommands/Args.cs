using UnityEngine;

public struct MoveArgs
{
    public Vector3 Position;
    public bool AllowAutoAttack;
}

public struct AttackArgs
{
    public Entity Entity;
    public int UnitOffsetIndex;
    public int TotalUnits;
}

public struct ResourceGatherArgs
{
    public Entity Resource;
}

public struct BuildArgs
{
    public Entity Building;
}