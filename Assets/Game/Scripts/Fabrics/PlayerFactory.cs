using Zenject;

public class PlayerFactory : PlaceholderFactory<int, PlayerModes, Player>
{
    public override Player Create(int ownerId, PlayerModes mode)
    {
        var player = base.Create(ownerId, mode);
        return player;
    }
}