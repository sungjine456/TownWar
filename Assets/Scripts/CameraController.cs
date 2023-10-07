using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 10f;
    [SerializeField] float _moveSmooth = 5f;
    [SerializeField] float _zoomSpeed = 15f;
    [SerializeField] float _zoomSmooth = 5f;

    Camera _camera;
    Controls _inputs;

    bool _moving;
    bool _zooming;

    Vector3 _center;
    float _right;
    float _left;
    float _up;
    float _down;
    float _angle;

    float _zoom;
    float _zoomMax;
    float _zoomMin;
    Vector2 _zoomPositionOnScreen;
    Vector3 _zoomPositionInWorld;
    float _zoomBaseValue;
    float _zoomBaseDistance;

    Transform _root;
    Transform _pivot;
    Transform _target;

    Vector3 _replaceBasePos;
    Vector3 _buildBasePos;

    void Awake()
    {
        _inputs = new Controls();
        _root = new GameObject("CameraHelper").transform;
        _pivot = new GameObject("CameraPivot").transform;
        _target = new GameObject("CameraTarget").transform;
    }

    void OnEnable()
    {
        _inputs.Enable();
        _inputs.Main.Move.started += _ => MoveStarted();
        _inputs.Main.Move.canceled += _ => MoveCanceled();
        _inputs.Main.TouchZoom.started += _ => ZoomStarted();
        _inputs.Main.TouchZoom.canceled += _ => ZoomCanceled();
        _inputs.Main.PointerClick.performed += _ => ScreenClicked();
    }

    void OnDisable()
    {
        _inputs.Main.Move.started -= _ => MoveStarted();
        _inputs.Main.Move.canceled -= _ => MoveCanceled();
        _inputs.Main.TouchZoom.started -= _ => ZoomStarted();
        _inputs.Main.TouchZoom.canceled -= _ => ZoomCanceled();
        _inputs.Main.PointerClick.performed -= _ => ScreenClicked();
        _inputs.Disable();
    }

    void ScreenClicked()
    {
        if (IsScreenPointOverBuilding())
        {
            bool found = false;

            Vector2 pos = _inputs.Main.PointerPosition.ReadValue<Vector2>();
            Vector3 planePos = CameraScreenPositionToPlanePosition(pos);

            for (int i = 0; i < GameManager.Instance.Grid.Buildings.Count; i++)
            {
                if (GameManager.Instance.Grid.IsWorldPositionIsOnPlane(planePos, GameManager.Instance.Grid.Buildings[i]))
                {
                    found = true;
                    BuildingController.Instance.SelectBuilding(GameManager.Instance.Grid.Buildings[i]);
                    break;
                }
            }
            if (!found)
                BuildingController.Instance.DeselectBuilding();
        }
        else
            BuildingController.Instance.DeselectBuilding();
    }

    bool IsScreenPointOverBuilding()
    {
        return Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), 150);
    }

    void MoveStarted()
    {
        if (GameManager.Instance.IsPlacing)
        {
            _buildBasePos = CameraScreenPositionToPlanePosition(_inputs.Main.PointerPosition.ReadValue<Vector2>());

            if (GameManager.Instance.Grid.IsWorldPositionIsOnPlane(_buildBasePos, BuildManager.Instance.CurrentTarget))
                BuildManager.Instance.CurrentTarget.StartMovingOnGrid();
        }

        if (BuildingController.Instance.SelectedBuilding != null)
        {
            _replaceBasePos = CameraScreenPositionToPlanePosition(_inputs.Main.PointerPosition.ReadValue<Vector2>());
            Building building = BuildingController.Instance.SelectedBuilding;

            if (GameManager.Instance.Grid.IsWorldPositionIsOnPlane(_replaceBasePos, building))
            {
                building.StartMovingOnGrid();
                GameManager.Instance.IsReplacing = true;
            }
        }

        if (!GameManager.Instance.IsMoveingBuilding && !GameManager.Instance.IsReplacing)
        {
            _moving = true;
        }
    }

    void MoveCanceled()
    {
        _moving = false;
        GameManager.Instance.IsMoveingBuilding = false;
        GameManager.Instance.IsReplacing = false;
    }

    void ZoomStarted()
    {
        if (UIMain.Instance.IsActive)
        {
            Vector2 touch0 = _inputs.Main.TouchPosition0.ReadValue<Vector2>();
            Vector2 touch1 = _inputs.Main.TouchPosition1.ReadValue<Vector2>();

            _zoomPositionOnScreen = Vector2.Lerp(touch0, touch1, 0.5f);
            _zoomPositionInWorld = CameraScreenPositionToPlanePosition(_zoomPositionOnScreen);
            _zoomBaseValue = _zoom;

            touch0.x /= Screen.width;
            touch1.x /= Screen.width;
            touch0.y /= Screen.height;
            touch1.y /= Screen.height;

            _zoomBaseDistance = Vector2.Distance(touch0, touch1);
            _zooming = true;
        }
    }

    void ZoomCanceled()
    {
        _zooming = false;
    }

    void Start()
    {
        _right = 50f;
        _left = 50f;
        _up = 40f;
        _down = 40f;
        _angle = 45f;

        _zoom = 10f;
        _zoomMin = 5;
        _zoomMax = 15;

        _pivot.SetParent(_root);
        _target.SetParent(_pivot);

        _root.position = _center;
        _root.localEulerAngles = Vector3.zero;

        _pivot.localPosition = Vector3.zero;
        _pivot.localEulerAngles = new Vector3(_angle, 0f, 0f);

        _target.localPosition = new Vector3(0f, 0f, -100f);
        _target.localEulerAngles = Vector3.zero;

        _camera = Camera.main;
        _camera.transform.rotation = _target.rotation;
        _camera.orthographicSize = _zoom;
    }

    void Update()
    {
        if (!Input.touchSupported)
        {
            float mouseScroll = _inputs.Main.MouseScroll.ReadValue<float>();

            if (mouseScroll > 0)
                _zoom -= _zoomSpeed * Time.deltaTime;
            else if (mouseScroll < 0)
                _zoom += _zoomSpeed * Time.deltaTime;
        }

        if (_zooming)
        {
            Vector2 touch0 = _inputs.Main.TouchPosition0.ReadValue<Vector2>();
            Vector2 touch1 = _inputs.Main.TouchPosition1.ReadValue<Vector2>();

            touch0.x /= Screen.width;
            touch0.y /= Screen.height;
            touch1.x /= Screen.width;
            touch1.y /= Screen.height;

            float currentDistance = Vector2.Distance(touch0, touch1);
            _zoom = _zoomBaseValue - (currentDistance - _zoomBaseDistance) * _zoomSmooth;

            Vector3 zoomCenter = CameraScreenPositionToPlanePosition(_zoomPositionOnScreen);
            _root.position += _zoomPositionInWorld - zoomCenter;
        }
        else if (_moving)
        {
            Vector2 move = _inputs.Main.MoveDelta.ReadValue<Vector2>();

            if (move != Vector2.zero)
            {
                move.x /= Screen.width;
                move.y /= Screen.height;
                _root.position -= move.x * _moveSpeed * _root.right;
                _root.position -= move.y * _moveSpeed * _root.forward;
            }
        }

        AdjustBounds();

        if (_camera.orthographicSize != _zoom)
            _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _zoom, _zoomSmooth * Time.deltaTime);
        if (_camera.transform.position != _target.position)
            _camera.transform.position = Vector3.Lerp(_camera.transform.position, _target.position, _moveSmooth * Time.deltaTime);

        if (GameManager.Instance.IsMoveingBuilding)
        {
            Vector3 pos = CameraScreenPositionToPlanePosition(_inputs.Main.PointerPosition.ReadValue<Vector2>());
            BuildManager.Instance.CurrentTarget.UpdateFromGrid(_buildBasePos, pos);
        }
        if (GameManager.Instance.IsReplacing)
        {
            Vector3 pos = CameraScreenPositionToPlanePosition(_inputs.Main.PointerPosition.ReadValue<Vector2>());
            BuildingController.Instance.SelectedBuilding.UpdateFromGrid(_replaceBasePos, pos);
        }
    }

    void AdjustBounds()
    {
        if (_zoom < _zoomMin) _zoom = _zoomMin;
        if (_zoom > _zoomMax) _zoom = _zoomMax;

        float h = PlaneOrtographicSize();
        float w = h * _camera.aspect;

        if (h > (_up + _down) / 2f)
            _zoom = (_up + _down) / 2f * Mathf.Sin(_angle * Mathf.Deg2Rad);
        if (w > (_right + _left) / 2f)
            _zoom = (_right + _left) / 2f / _camera.aspect * Mathf.Sin(_angle * Mathf.Deg2Rad);

        h = PlaneOrtographicSize();
        w = h * _camera.aspect;

        Vector3 tr = _root.position + _root.right * w + _root.forward * h;
        Vector3 tl = _root.position - _root.right * w + _root.forward * h;
        Vector3 dl = _root.position - _root.right * w - _root.forward * h;

        if (tr.x > _center.x + _right)
            _root.position += Vector3.left * Mathf.Abs(tr.x - (_center.x + _right));
        else if (tl.x < _center.x - _left)
            _root.position += Vector3.right * Mathf.Abs(_center.x - _left - tl.x);
        else if (tr.z > _center.z + _up)
            _root.position += Vector3.back * Mathf.Abs(tr.z - (_center.z + _up));
        else if (dl.z < _center.z - _down)
            _root.position += Vector3.forward * Mathf.Abs(_center.z - _down - dl.z);
    }

    float PlaneOrtographicSize() => _zoom * 2f / Mathf.Sin(_angle * Mathf.Deg2Rad) / 2f;

    Vector3 CameraScreenPositionToWorldPosition(Vector2 position)
    {
        float h = _camera.orthographicSize * 2f;
        float w = _camera.aspect * h;

        Vector3 ancher = _camera.transform.position - (_camera.transform.right * w / 2f) - (_camera.transform.up * h / 2f);

        return ancher + (_camera.transform.right * position.x / Screen.width * w) + (_camera.transform.up * position.y / Screen.height * h);
    }

    public Vector3 CameraScreenPositionToPlanePosition(Vector2 position)
    {
        Vector3 point = CameraScreenPositionToWorldPosition(position);

        float h = point.y - _root.position.y;

        return point + _camera.transform.forward * h / Mathf.Sin(_angle * Mathf.Deg2Rad);
    }
}
