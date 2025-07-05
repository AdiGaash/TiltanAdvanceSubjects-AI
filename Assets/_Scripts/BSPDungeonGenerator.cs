using UnityEngine;
using System.Collections.Generic;

public class BSPDungeonGenerator : MonoBehaviour
{
    [Header("Map Settings")]
    public int mapWidth = 50;
    public int mapHeight = 50;
    public int maxDepth = 4;
    public GameObject roomPrefab;
    public GameObject corridorPrefab;

    [Header("Multi-Floor Settings")]
    public int numberOfFloors = 1;
    public GameObject stairPrefab;

    private List<BSPNode>[] dungeonFloors;

    void Start()
    {
        GenerateDungeon();
    }

    [ContextMenu("Generate Dungeon")]
    void GenerateDungeon()
    {
        // Clear any existing dungeon first
        ClearExistingDungeon();
        
        // Check if room prefab is assigned
        if (roomPrefab == null)
        {
            Debug.LogError("Room prefab is not assigned. Please assign a prefab in the inspector.");
            return;
        }
        
        dungeonFloors = new List<BSPNode>[numberOfFloors];

        for (int floor = 0; floor < numberOfFloors; floor++)
        {
            // Create root node for this floor
            BSPNode root = new BSPNode(0, 0, mapWidth, mapHeight);
            root.Split(maxDepth);

            List<BSPNode> leafNodes = root.GetLeafNodes();
            dungeonFloors[floor] = leafNodes;

            // Generate rooms
            foreach (var node in leafNodes)
            {
                node.CreateRoom(roomPrefab, floor, transform);
            }

            // Generate corridors only if corridor prefab is assigned
            if (corridorPrefab != null)
            {
                root.CreateCorridors(corridorPrefab, floor, transform);
            }
            else
            {
                Debug.LogWarning("Corridor prefab is not assigned. Corridors will not be generated.");
            }
        }

        // Optional: Connect floors with stairs
        ConnectFloorsWithStairs();
    }

    void ConnectFloorsWithStairs()
    {
        // Check if stair prefab is assigned and we have multiple floors
        if (stairPrefab == null)
        {
            Debug.LogWarning("Stair prefab is not assigned. Floors will not be connected with stairs.");
            return;
        }
        
        // Only add stairs if we have more than one floor
        if (numberOfFloors <= 1)
        {
            return;
        }

        for (int i = 0; i < numberOfFloors - 1; i++)
        {
            // Pick random room on current floor
            var roomA = dungeonFloors[i][Random.Range(0, dungeonFloors[i].Count)];
            var roomB = dungeonFloors[i + 1][Random.Range(0, dungeonFloors[i + 1].Count)];

            Vector3 stairPos = new Vector3(roomA.GetRoomCenter().x, i * 5f, roomA.GetRoomCenter().y);
            Instantiate(stairPrefab, stairPos, Quaternion.identity);
        }
    }
    
    private void ClearExistingDungeon()
    {
        // Find all rooms and corridors and destroy them
        foreach (Transform child in transform)
        {
            #if UNITY_EDITOR
            if (Application.isEditor && !Application.isPlaying)
            {
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    if (child != null)
                        DestroyImmediate(child.gameObject);
                };
            }
            else
            #endif
            {
                Destroy(child.gameObject);
            }
        }
    }
}