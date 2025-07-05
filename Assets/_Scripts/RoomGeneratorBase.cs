using UnityEngine;
using System.Collections.Generic;

public class RoomGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    public int mapWidth = 50;
    public int mapHeight = 50;
    public int maxDepth = 4;
    public int minLeafSize = 10;
    public GameObject roomPrefab;
    
    [Header("Room Size Settings")]
    [Min(3)]
    public int minRoomWidth = 4;
    [Min(3)]
    public int minRoomHeight = 4;
    [Min(4)]
    public int maxRoomWidth = 12;
    [Min(4)]
    public int maxRoomHeight = 12;
    
    [Header("Visualization")]
    public bool showGizmos = true;
    
    protected List<BSPNode> leafNodes;
    protected BSPNode rootNode;
    // Add this to the class fields
    protected List<RoomInfo> roomInfos = new List<RoomInfo>();

    protected virtual void Start()
    {
        GenerateRooms();
    }

    [ContextMenu("Generate Rooms")]
    public virtual void GenerateRooms()
    {
        // Clear any existing rooms
        ClearExistingRooms();
        
        // Check if room prefab is assigned
        if (roomPrefab == null)
        {
            Debug.LogError("Room prefab is not assigned. Please assign a prefab in the inspector.");
            return;
        }
        
        // Validate room size parameters
        ValidateRoomSizeParameters();
        
        // Create root node
        rootNode = new BSPNode(0, 0, mapWidth, mapHeight);
        rootNode.Split(maxDepth, minLeafSize);

        // Get leaf nodes and create rooms
        leafNodes = rootNode.GetLeafNodes();
        
        // Generate rooms
        CreateRooms();
        
        // Call the method that will be implemented by child classes
        OnRoomsGenerated();
    }
    
    protected virtual void CreateRooms()
    {
        // Clear the previous room info list if we're regenerating
        roomInfos.Clear();
        
        foreach (var node in leafNodes)
        {
            RoomInfo roomInfo = node.CreateRoom(roomPrefab, 0, minRoomWidth, minRoomHeight, maxRoomWidth, maxRoomHeight);
            if (roomInfo != null)
            {
                roomInfos.Add(roomInfo);
            }
        }
    }
    
    // Changed from abstract to virtual with empty implementation
    protected virtual void OnRoomsGenerated()
    {
        // Empty default implementation
    }
    
    protected void ValidateRoomSizeParameters()
    {
        // Ensure min values are at least 3 (smallest reasonable room)
        minRoomWidth = Mathf.Max(3, minRoomWidth);
        minRoomHeight = Mathf.Max(3, minRoomHeight);
        
        // Ensure max values are greater than min values
        maxRoomWidth = Mathf.Max(minRoomWidth + 1, maxRoomWidth);
        maxRoomHeight = Mathf.Max(minRoomHeight + 1, maxRoomHeight);
    }
    
    protected virtual void ClearExistingRooms()
    {
        // Find and destroy all GameObjects with "Room" tag in the scene
        GameObject[] rooms = GameObject.FindGameObjectsWithTag("Room");
        foreach (GameObject room in rooms)
        {
            #if UNITY_EDITOR
            if (Application.isEditor && !Application.isPlaying)
            {
                DestroyImmediate(room);
            }
            else
            #endif
            {
                Destroy(room);
            }
        }
    }
    
    // Draw gizmos for visualization
    protected virtual void OnDrawGizmos()
    {
        if (!showGizmos || rootNode == null) return;
        
        rootNode.DrawGizmos();
    }
    
    // Add a method to access the room information
    public List<RoomInfo> GetRoomInfos()
    {
        return roomInfos;
    }
}