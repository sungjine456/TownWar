using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

public class GameCameraCtrl : CameraController<GameCameraCtrl>
{
    Vector3 _buildBasePos;
    Vector3 _replaceBasePos;
    
    bool _isMoveingBuilding;

    public bool IsPlacingBuilding { get; set; } = false;
    public bool IsReplacing { get; set; } = false;
    public bool IsReplacingBuilding { get; set; } = false;

    protected override void ScreenClicked()
    {
        Vector2 pos = _inputs.Main.PointerPosition.ReadValue<Vector2>();
        PointerEventData pointerData = new(EventSystem.current) { position = pos };
        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(pointerData, results);

        if (UIMain.Instance != null && results.Count <= 0)
        {
            bool found = false;
            GameBuilding building;
            Vector3 planePos = CameraScreenPositionToPlanePosition(pos);

            for (int i = 0; i < UIMain.Instance.Grid._buildings.Count; i++)
            {
                building = UIMain.Instance.Grid._buildings[i];

                if (UIMain.Instance.Grid.IsWorldPositionOnPlane(planePos, building))
                {
                    found = true;
                    BuildingController.Instance.SelectBuilding(building);
                    break;
                }
            }

            if (!found)
                BuildingController.Instance.DeselectBuilding();
        }
    }

    protected override void MoveStarted()
    {
        if (!IsUI())
        {
            if (IsPlacingBuilding)
            {
                _buildBasePos = CameraScreenPositionToPlanePosition(_inputs.Main.PointerPosition.ReadValue<Vector2>());

                if (UIMain.Instance.Grid.IsWorldPositionOnPlane(_buildBasePos, UIBuild.Instance.Target))
                {
                    UIBuild.Instance.Target.StartMovingOnGrid();
                    _isMoveingBuilding = true;
                }
            }

            if (BuildingController.Instance != null && BuildingController.Instance.SelectedBuilding != null)
            {
                _replaceBasePos = CameraScreenPositionToPlanePosition(_inputs.Main.PointerPosition.ReadValue<Vector2>());

                if (UIMain.Instance.Grid.IsWorldPositionOnPlane(_replaceBasePos, BuildingController.Instance.SelectedBuilding))
                {
                    if (!IsReplacing)
                        IsReplacing = true;

                    BuildingController.Instance.SelectedBuilding.StartMovingOnGrid();
                    IsReplacingBuilding = true;
                }
            }

            if (!_isMoveingBuilding && !IsReplacingBuilding)
                _moving = true;
        }
    }

    protected override void MoveCanceled()
    {
        base.MoveCanceled();

        _isMoveingBuilding = false;
        IsReplacingBuilding = false;
    }

    protected override void Update()
    {
        base.Update();

        if (IsPlacingBuilding && _isMoveingBuilding)
        {
            Vector3 pos = CameraScreenPositionToPlanePosition(_inputs.Main.PointerPosition.ReadValue<Vector2>());
            UIBuild.Instance.Target.UpdateFromGrid(_buildBasePos, pos);
        }

        if (IsReplacing && IsReplacingBuilding)
        {
            Vector3 pos = CameraScreenPositionToPlanePosition(_inputs.Main.PointerPosition.ReadValue<Vector2>());
            BuildingController.Instance.SelectedBuilding.UpdateFromGrid(_replaceBasePos, pos);
        }
    }
}
