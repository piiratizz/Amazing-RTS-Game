using System.Collections.Generic;

public class PlayerRegistry
{
    private Dictionary<int, Player> _players = new Dictionary<int, Player>();

    public void Register(int id, Player player)
    {
        _players.Add(id, player);
    }

    public void Unregister(int id)
    {
        _players.Remove(id);
    }

    public Player GetPlayer(int id)
    {
        return _players[id];
    }
}