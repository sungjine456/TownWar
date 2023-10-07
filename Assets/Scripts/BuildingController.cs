public class BuildingController : SingletonMonoBehaviour<BuildingController>
{
    int _originalX;
    int _originalY;

    public Building SelectedBuilding { get; private set; }

    public void SelectBuilding(Building building)
    {
        _originalX = building.X;
        _originalY = building.Y;

        if (SelectedBuilding != null && SelectedBuilding != building)
            DeselectBuilding();

        SelectedBuilding = building;
        SelectedBuilding.SetActiveBaseArea(true);

        UIBuildingOption.Instance.SetActive(true);
    }

    public void DeselectBuilding()
    {
        UIBuildingOption.Instance.SetActive(false);

        if (SelectedBuilding != null)
        {
            if (_originalX != SelectedBuilding.X || _originalY != SelectedBuilding.Y)
            {
                if (!GameManager.Instance.Grid.CanPlaceBuilding(SelectedBuilding))
                {
                    SelectedBuilding.PlacedOnGrid(_originalX, _originalY);
                }
                else
                {
                    SelectedBuilding.PlacedOnGrid(SelectedBuilding.X, SelectedBuilding.Y);
                    Player.Instance.UpdateBuilding(SelectedBuilding);
                }
            }

            SelectedBuilding.SetActiveBaseArea(false);
        }

        SelectedBuilding = null;
        GameManager.Instance.IsReplacing = false;
    }
}
