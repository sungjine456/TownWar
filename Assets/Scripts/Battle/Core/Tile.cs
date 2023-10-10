public class Tile
{
    public Data.BuildingId _id;
    public BattleVector2Int _position;
    public int _index;

    public Tile(Data.BuildingId id, BattleVector2Int position, int index = -1)
    {
        _id = id;
        _position = position;
        _index = index;
    }
}
