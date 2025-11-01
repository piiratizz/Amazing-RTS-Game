using Zenject;

public class PlayerFactory : PlaceholderFactory<int, PlayerModes, Player>
{
    private readonly PlayerRegistry _playerRegistry;

    public PlayerFactory(PlayerRegistry playerRegistry)
    {
        _playerRegistry = playerRegistry;
    }
    
    public override Player Create(int ownerId, PlayerModes mode)
    {
        var player = base.Create(ownerId, mode);
        _playerRegistry.Register(ownerId, player);

        return player;
    }
}