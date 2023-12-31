using System.Collections.Generic;

using UnityEngine;

public class GameBuildGrid : BuildGrid
{
    public List<GameBuilding> _buildings;

    GameBuilding InstantiateBuilding(Data.Building data)
    {
        var prefab = GameManager.Instance.GetBuildingPrefab(data.buildingId);
        GameBuilding b = null;

        if (prefab)
        {
            b = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            b.Initialize(data);
            b.Id = data.id;
        }

        return b;
    }

    public GameBuilding GetBuilding(Data.BuildingId id)
    {
        for (int i = 0; i < _buildings.Count; i++)
        {
            if (_buildings[i].BuildingId == id)
                return _buildings[i];
        }

        return null;
    }

    public void BuildBuilding(Data.Building data, bool isBuilding = true)
    {
        GameBuilding b = InstantiateBuilding(data);

        if (b)
        {
            b.IsConstructing = isBuilding;
            b.IsBuilding = isBuilding;

            _buildings?.Add(b);
        }
    }

    public void AddBuilding(Data.Building data)
    {
        GameBuilding b = InstantiateBuilding(data);
        
        if (b)
        {
            _buildings?.Add(b);

            switch (b.BuildingId)
            {
                case Data.BuildingId.townHall:
                    GameManager.Instance.AddMaxGold(b.Capacity);
                    GameManager.Instance.AddMaxElixir(b.Capacity);
                    break;
                case Data.BuildingId.goldStorage:
                    GameManager.Instance.AddMaxGold(b.Capacity);
                    break;
                case Data.BuildingId.elixirStorage:
                    GameManager.Instance.AddMaxElixir(b.Capacity);
                    break;
            }
        }
    }

    public void AddBuilding(Data.Building data, float constructedTime)
    {
        GameBuilding b = InstantiateBuilding(data);

        if (b)
        {
            b.Upgrade(BuildingController.Instance.GetNextLevelBuildingInfo(b), constructedTime);

            _buildings?.Add(b);

            switch (b.BuildingId)
            {
                case Data.BuildingId.townHall:
                    GameManager.Instance.AddMaxGold(b.Capacity);
                    GameManager.Instance.AddMaxElixir(b.Capacity);
                    break;
                case Data.BuildingId.goldStorage:
                    GameManager.Instance.AddMaxGold(b.Capacity);
                    break;
                case Data.BuildingId.elixirStorage:
                    GameManager.Instance.AddMaxElixir(b.Capacity);
                    break;
            }
        }
    }

    public bool CanPlaceBuilding(Building b)
    {
        if (b.CurrentX < 0 || b.CurrentY < 0 || b.CurrentX + b.Columns > Data.GRID_SIZE || b.CurrentY + b.Rows > Data.GRID_SIZE)
            return false;

        for (int i = 0; i < _buildings.Count; i++)
        {
            if (_buildings[i].Id != b.Id)
            {
                Rect rect1 = new(_buildings[i].CurrentX, _buildings[i].CurrentY, _buildings[i].Columns, _buildings[i].Rows);
                Rect rect2 = new(b.CurrentX, b.CurrentY, b.Columns, b.Rows);

                if (rect2.Overlaps(rect1))
                    return false;
            }
        }

        return true;
    }
}
