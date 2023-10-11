using System;
using System.Collections.Generic;
using System.Text;

public class Data
{
    public const int GRID_SIZE = 45;
    public const float CELL_SIZE = 1f;
    public const int battleGridOffset = 1;
    public const int battleTilesWorthOfOneWall = 15;

    public const int minGoldCollect = 10;
    public const int minElixirCollect = 10;

    public enum BuildingId
    {
        townHall, goldMine, goldStorage, elixirMine, elixirStorage, buildersHut, armyCamp, barracks, wall, cannon
    }

    public enum UnitId
    {
        warrior, archer
    }

    public enum UnitStatus
    {
        army, trained, training
    }

    public enum ResourceType
    {
        gold, elixir
    }

    [Serializable]
    public class Player
    {
        public int gold;
        public int elixir;
        public int gems;
        public List<PlayerBuilding> buildings = new();
        public PlayerUnit[] armyUnits;
        public PlayerUnit[] trainedUnits;
        public PlayerUnit[] trainingUnits;
        public DateTime lastPlayTime;
    }

    [Serializable]
    public struct PlayerUnit
    {
        public UnitId id;
        public int level;
        public float trainedTime;

        public PlayerUnit(UnitId id, int level, float trainedTime)
        {
            this.id = id;
            this.level = level;
            this.trainedTime = trainedTime;
        }
    }

    [Serializable]
    public class PlayerBuilding
    {
        public int id;
        public BuildingId buildingId;
        public int level;
        public int x;
        public int y;
        public int capacity;
        public float storage;
        public float constructedTime;

        public PlayerBuilding(int id, BuildingId buildingId, int level, int x, int y, int capacity, float storage, float constructedTime)
        {
            this.id = id;
            this.buildingId = buildingId;
            this.level = level;
            this.x = x;
            this.y = y;
            this.capacity = capacity;
            this.storage = storage;
            this.constructedTime = constructedTime;
        }

        public void SetData(BuildingToBuild data)
        {
            level = data.level;
            buildingId = data.buildingId;
            storage = data.storage;
            capacity = data.capacity;
        }
    }

    [Serializable]
    public class Unit
    {
        public UnitId id;
        public int level;
        public int numberOfPopulation;
        public int trainTime;
        public float trainedTime;
        public int damage;
        public float moveSpeed;
        public int health;
        public int attackRange;
        public float attackSpeed;
        public float rangedSpeed;

        public Unit(UnitToTrain unit)
        {
            id = unit.id;
            level = unit.level;
            numberOfPopulation = unit.numberOfPopulation;
            trainTime = unit.trainTime;
            damage = unit.damage;
            moveSpeed = unit.moveSpeed;
            health = unit.health;
            attackRange = unit.attackRange;
            attackSpeed = unit.attackSpeed;
            rangedSpeed = unit.rangedSpeed;
        }

        public PlayerUnit GetPlayerUnit()
        {
            return new(id, level, trainedTime);
        }
    }

    [Serializable]
    public class UnitToTrain
    {
        public UnitId id;
        public int level;
        public int numberOfPopulation;
        public int trainTime;
        public int damage;
        public float moveSpeed;
        public int health;
        public int attackRange;
        public float attackSpeed;
        public float rangedSpeed;
    }

    /*
     * 건물 종류 별 capacity 사용 용도
     *  - goldMine, elixirMine : 자원 임시 저장 용량
     *  - goldStorage, elixirStorage : 자원 저장 용량
     *  - armyCamp : 유닛 저장 크기
     */
    [Serializable]
    public class Building
    {
        public int id;
        public BuildingId buildingId;
        public int level;
        public int x;
        public int y;
        public int columns;
        public int rows;
        public float storage;
        public DateTime boost;
        public int health;
        public float damage;
        public int capacity;
        public int speed;
        public float radius;
        public int blindRange;
        public int rangedSpeed;
        DateTime constructTime;
        int buildTime;

        public DateTime ConstructTime { get { return constructTime; } }
        public int BuildTime { get { return buildTime; } }

        public Building(BuildingToBuild data) 
        {
            SetData(data);
        }

        public Building(int id, BuildingId buildingId, int level, int x, int y, int columns, int rows)
        {
            this.id = id;
            this.buildingId = buildingId;
            this.level = level;
            this.x = x;
            this.y = y;
            this.columns = columns;
            this.rows = rows;
        }

        void SetBuildTime(int buildTime)
        {
            this.buildTime = buildTime;
            constructTime = DateTime.Now.AddSeconds(buildTime);
        }

        public void SetData(BuildingToBuild data)
        {
            level = data.level;
            buildingId = data.buildingId;
            columns = data.columns;
            rows = data.rows;
            storage = data.storage;
            boost = data.boost;
            health = data.health;
            damage = data.damage;
            capacity = data.capacity;
            speed = data.speed;
            radius = data.radius;
            blindRange = data.blindRange;
            rangedSpeed = data.rangedSpeed;

            SetBuildTime(data.buildTime);
        }

        public PlayerBuilding GetPlayerBuilding()
        {
            var span = constructTime - DateTime.Now;

            return new(id, buildingId, level, x, y, capacity, storage, (float)span.TotalSeconds);
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.Append("level : ");
            sb.Append(level);
            sb.Append(", storage : ");
            sb.Append(storage);
            sb.Append(", health : ");
            sb.Append(health);
            sb.Append(", capacity : ");
            sb.Append(capacity);
            sb.Append(", speed : ");
            sb.Append(speed);
            return sb.ToString();
        }
    }

    /*
     * 건물 종류 별 capacity 사용 용도
     *  - goldMine, elixirMine : 자원 임시 저장 용량
     *  - goldStorage, elixirStorage : 자원 저장 용량
     *  - armyCamp : 유닛 저장 크기
     */
    [Serializable]
    public class BuildingToBuild
    {
        public BuildingId buildingId;
        public int level;
        public int requiredGold;
        public int requiredElixir;
        public int requiredGems;
        public int columns;
        public int rows;
        public float storage;
        public DateTime boost;
        public int health;
        public float damage;
        public int capacity;
        public int speed;
        public float radius;
        public int buildTime;
        public int blindRange;
        public int rangedSpeed;

        public override string ToString()
        {
            StringBuilder sb = new();
            sb.Append("level : ");
            sb.Append(level);
            sb.Append(", storage : ");
            sb.Append(storage);
            sb.Append(", health : ");
            sb.Append(health);
            sb.Append(", capacity : ");
            sb.Append(capacity);
            sb.Append(", speed : ");
            sb.Append(speed);
            return sb.ToString();
        }
    }

    [Serializable]
    public class BuildingAvailability
    {
        public int hallLevel;
        public BuildingLimit[] limits;
    }

    [Serializable]
    public class BuildingLimit
    {
        public BuildingId id;
        public int count;
        public int maxLevel;
    }
}
