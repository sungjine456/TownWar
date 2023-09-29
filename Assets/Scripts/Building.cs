using UnityEngine;

using static Data;

public class Building : MonoBehaviour
{
    [SerializeField] BuildingId _id;
    [SerializeField] int _rows;
    [SerializeField] int _columns;
    [SerializeField] MeshRenderer _baseArea;

    int _level;
    int _currentX;
    int _currentY;

    public BuildingId Id { get { return _id; } }
}
