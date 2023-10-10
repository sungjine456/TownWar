using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController<T> : SingletonMonoBehaviour<T> where T : CameraController<T>
{
    [SerializeField] float _moveSpeed = 80f;
    [SerializeField] float _moveSmooth = 10f;
    [SerializeField] float _zoomSmooth = 10f;

    protected Camera _camera;
    protected Controls _inputs;

    protected bool _moving;
    protected bool _zooming;

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

    [HideInInspector] public Vector3 _planDownLeft;
    [HideInInspector] public Vector3 _planTopRight;

    protected override void OnAwake()
    {
        _inputs = new Controls();
        _root = new GameObject("CameraHelper").transform;
        _pivot = new GameObject("CameraPivot").transform;
        _target = new GameObject("CameraTarget").transform;

        _camera = Camera.main;

        _right = 40f;
        _left = 40f;
        _up = 40f;
        _down = 40f;
        _angle = 45f;

        _zoom = 10f;
        _zoomMin = 5f;
        _zoomMax = 20f;

        _pivot.SetParent(_root);
        _target.SetParent(_pivot);

        _root.position = _center;
        _root.localEulerAngles = Vector3.zero;

        _pivot.localPosition = Vector3.zero;
        _pivot.localEulerAngles = new Vector3(_angle, 0f, 0f);

        _target.localPosition = new Vector3(0f, 0f, -100f);
        _target.localEulerAngles = Vector3.zero;

        _camera.transform.rotation = _target.rotation;
        _camera.orthographicSize = _zoom;

        _planDownLeft = CameraScreenPositionToPlanePosition(Vector2.zero);
        _planTopRight = CameraScreenPositionToPlanePosition(new Vector2(Screen.width, Screen.height));
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

    protected bool IsUI()
    {
        Vector2 pos = _inputs.Main.PointerPosition.ReadValue<Vector2>();
        PointerEventData pointerData = new(EventSystem.current) { position = pos };
        List<RaycastResult> results = new();
        EventSystem.current.RaycastAll(pointerData, results);

        return results.Count != 0;
    }

    protected virtual void ScreenClicked() { }

    protected virtual void MoveStarted() { }

    protected virtual void MoveCanceled()
    {
        _moving = false;
    }

    void ZoomStarted()
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

    void ZoomCanceled()
    {
        _zooming = false;
    }

    protected virtual void Update()
    {
        if (!Input.touchSupported)
        {
            float mouseScroll = _inputs.Main.MouseScroll.ReadValue<float>();

            if (mouseScroll > 0)
                _zoom -= 15f * Time.deltaTime;
            else if (mouseScroll < 0)
                _zoom += 15f * Time.deltaTime;
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
                _root.position -= move.x * _moveSpeed * _zoom / _zoomMax * _root.right;
                _root.position -= move.y * _moveSpeed * _zoom / _zoomMax * _root.forward;
            }
        }

        AdjustBounds();

        if (_camera.orthographicSize != _zoom)
            _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, _zoom, _zoomSmooth * Time.deltaTime);

        if (_camera.transform.position != _target.position)
        {
            Vector3 velocity = Vector3.zero;
            _camera.transform.position = Vector3.SmoothDamp(_camera.transform.position, _target.position, ref velocity, _moveSmooth * Time.deltaTime);
        }

        _planDownLeft = CameraScreenPositionToPlanePosition(Vector2.zero);
        _planTopRight = CameraScreenPositionToPlanePosition(new Vector2(Screen.width, Screen.height));
    }

    void AdjustBounds()
    {
        if (_zoom < _zoomMin) _zoom = _zoomMin;
        if (_zoom > _zoomMax) _zoom = _zoomMax;

        float h = PlaneOrtographicSize();
        float w = h * _camera.aspect;

        if (h > (_up + _down) / 2f)
        {
            float v = (_up + _down) / 2f;
            _zoom = v * Mathf.Sin(_angle * Mathf.Deg2Rad);
        }

        if (w > (_right + _left) / 2f)
        {
            float v = (_right + _left) / 2f / _camera.aspect;
            _zoom = v * Mathf.Sin(_angle * Mathf.Deg2Rad);
        }

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

    float PlaneOrtographicSize()
    {
        float h = _zoom * 2f;

        return h / Mathf.Sin(_angle * Mathf.Deg2Rad) / 2f;
    }

    protected Vector3 CameraScreenPositionToWorldPosition(Vector2 position)
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
        float x = h / Mathf.Sin(_angle * Mathf.Deg2Rad);

        return point + _camera.transform.forward * x;
    }
}
