using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] int _rows;
    [SerializeField] int _columns;
    [SerializeField] MeshRenderer _baseArea;

    int _idx;
    int _level;
    int _currentX;
    int _currentY;
}
