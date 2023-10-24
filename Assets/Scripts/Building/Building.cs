using System;
using UnityEngine;

public class Building : MonoBehaviour
{
    [Serializable]
    public class Level
    {
        public int level;
        public GameObject mesh;
    }

    [SerializeField] protected int id;
    [SerializeField] protected Data.BuildingId buildingId;
    [SerializeField] protected int rows = 1;
    [SerializeField] protected int columns = 1;
    [SerializeField] protected Level[] levels;
    [SerializeField] protected MeshRenderer baseArea;

    public Data.Building data;

    public int Id { get { return id; } set { id = value; } }
    public Data.BuildingId BuildingId { get { return buildingId; } set { buildingId = value; } }
    public int Rows => rows;
    public int Columns => columns;
    public int CurrentX { get; protected set; }
    public int CurrentY { get; protected set; }
    public int CurrentLevel { get { return data.level; } set { data.level = value; } }
    public int BuildTime => data.BuildTime;
    public int Capacity => data.capacity;

    public void Initialize(bool isStatusBaseArea = false)
    {
        StatusBaseArea(isStatusBaseArea);
    }

    public void Initialize(Data.Building data) 
    {
        Initialize();
        SetData(data);
    }

    public void SetData(Data.Building data)
    {
        this.data = data;
        SetPosition(data.x, data.y);

        for (int i = 0; i < levels.Length; i++)
        {
            if (levels[i].level != data.level)
                levels[i].mesh.SetActive(false);
            else
                levels[i].mesh.SetActive(true);
        }
    }

    public virtual void SetPosition(int x, int y)
    {
        CurrentX = x;
        CurrentY = y;
        transform.position = GameManager.Instance.Grid.GetCenterPosition(x, y, rows, columns);
    }

    public void StatusBaseArea(bool status)
    {
        baseArea.gameObject.SetActive(status);
    }
}
