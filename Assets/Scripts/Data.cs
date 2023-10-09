using System.Collections.Generic;

public static class Data
{
    public const int GRID_SIZE = 45;
    public const float CELL_SIZE = 1f;

    public const int MIN_COLLECT_RESOUCES = 10;

    [System.Serializable]
    public class Player
    {
        public List<BuildingToSave> buildings = new();
    }

    public enum BuildingId
    {
        TownHall,
        GoldMine,
        GoldStorage
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
        public int requiredGold;
        public int capacity;
        public float speed;
        public float storage;

        public Building(int idx, BuildingId buildingId, int x, int y, int columns, int rows, int requiredGold, int capacity, float speed, float storage)
        {
            this.idx = idx;
            this.buildingId = buildingId;
            this.x = x;
            this.y = y;
            this.columns = columns;
            this.rows = rows;
            this.requiredGold = requiredGold;
            this.capacity = capacity;
            this.speed = speed;
            this.storage = storage;
        }
    }

    [System.Serializable]
    public class BuildingToBuild
    {
        public BuildingId buildingId;
        public int requiredGold;
        public int columns;
        public int rows;
        public int capacity;
        public float speed;

        public override string ToString()
        {
            return "buildingId : " + buildingId.ToString() + ", requiredGold : " + requiredGold + ", columns : " + columns + ", rows : " + rows
                 + ", capacity : " + capacity + ", speed : " + speed;
        }
    }

    [System.Serializable]
    public class BuildingToSave
    {
        public int idx;
        public BuildingId buildingId;
        public int x;
        public int y;
        public float storage;

        public BuildingToSave(int idx, BuildingId buildingId, int x, int y, float storage)
        {
            this.idx = idx;
            this.buildingId = buildingId;
            this.x = x;
            this.y = y;
            this.storage = storage;
        }

        public override string ToString()
        {
            return "Idx : " + idx + ", buildingId : " + buildingId.ToString() + ", x : " + x + ", y : " + y + ", storage : " + storage;
        }
    }
}
