using System.Collections.Generic;

public static class Data
{
    public const int GRID_SIZE = 45;
    public const float CELL_SIZE = 1f;

    [System.Serializable]
    public class Player
    {
        public int gold;
        public List<Building> buildings = new();
    }

    public enum BuildingId
    {
        GoldMine
    }

    [System.Serializable]
    public class Building
    {
        public int idx;
        public BuildingId buildingId;
        public int x;
        public int y;
        public int columns;
        public int rows;

        public Building(int idx, BuildingId buildingId, int x, int y, int columns, int rows)
        {
            this.idx = idx;
            this.buildingId = buildingId;
            this.x = x;
            this.y = y;
            this.columns = columns;
            this.rows = rows;
        }
    }
}
