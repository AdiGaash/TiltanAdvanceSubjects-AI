using UnityEngine;

public class RoomInfo
{
    public GameObject RoomObject { get; set; }
    public RectInt RoomRect { get; set; }
    public bool[,] TileGrid { get; private set; }

    
    public RoomInfo(GameObject roomObject, RectInt roomRect)
    {
        RoomObject = roomObject;
        RoomRect = roomRect;
    }
    
    public void SetTileGrid(bool[,] grid)
    {
        TileGrid = grid;
    }

}