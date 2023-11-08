using UnityEngine;

public class BattleCameraCtrl : CameraController<BattleCameraCtrl>
{
    protected override void ScreenClicked()
    {
        if (!IsUI())
        {
            if (UIBattleUnits.Instance._target)
            {
                Vector2 pos = _inputs.Main.PointerPosition.ReadValue<Vector2>();
                Vector3 planePos = CameraScreenPositionToPlanePosition(pos);

                planePos = BattleManager.Instance.Grid.transform.InverseTransformPoint(planePos);

                if (planePos.x >= (0 - Data.battleGridOffset) &&
                    planePos.x < (Data.GRID_SIZE + Data.battleGridOffset) &&
                    planePos.z >= (0 - Data.battleGridOffset) &&
                    planePos.z < (Data.GRID_SIZE + Data.battleGridOffset))
                {
                    UIBattleMain.Instance.PlaceUnit(Mathf.FloorToInt(planePos.x), Mathf.FloorToInt(planePos.z));
                }
            }
            else
                AlertManager.Instance.Error("배치할 유닛을 선택하세요!");
        }
    }

    protected override void MoveStarted()
    {
        if (!IsUI())
            _moving = true;
    }
}
