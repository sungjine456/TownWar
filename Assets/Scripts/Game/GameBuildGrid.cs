using System.Collections.Generic;
using UnityEngine;

public class GameBuildGrid : BuildGrid
{
    public List<GameBuilding> _buildings;

    public GameBuilding GetBuilding(int id)
    {
        for (int i = 0; i < _buildings.Count; i++)
        {
            if (_buildings[i].Id == id)
                return _buildings[i];
        }

        return null;
    }

    public override void AddBuilding(Data.Building buildingData, bool nowConstruct = false)
    {
        GameBuilding prefab = GameManager.Instance.GetBuildingPrefab(buildingData.buildingId);

        if (prefab)
        {
            var building = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            building.Initialize(buildingData);
            building.Id = buildingData.id;
            building.IsConstructing = nowConstruct;

            _buildings?.Add(building);

            switch (building.BuildingId)
            {
                case Data.BuildingId.townHall:
                    UIMain.Instance.AddMaxGold(building.Capacity);
                    UIMain.Instance.AddMaxElixir(building.Capacity);
                    break;
                case Data.BuildingId.goldStorage:
                    UIMain.Instance.AddMaxGold(building.Capacity);
                    break;
                case Data.BuildingId.elixirStorage:
                    UIMain.Instance.AddMaxElixir(building.Capacity);
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
