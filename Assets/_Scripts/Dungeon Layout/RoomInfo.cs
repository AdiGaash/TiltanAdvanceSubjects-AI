using UnityEngine;

public class RoomInfo
{
    public GameObject RoomObject { get; set; }
    public RectInt RoomRect { get; set; }
    
    public RoomInfo(GameObject roomObject, RectInt roomRect)
    {
        RoomObject = roomObject;
        RoomRect = roomRect;
    }
}