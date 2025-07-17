using UnityEngine;
using System.Collections.Generic;

public class NoiseCutoutRoomGenerator : RoomGenerator
{
    [Header("Noise Settings")]
    public float noiseScale = 5f;
    public float noiseThreshold = 0.5f;
    public float seedOffsetX = 100f;
    public float seedOffsetY = 100f;
    
    [SerializeField] private RoomMeshGenerator meshGenerator;
    
    /// <summary>
    /// Override the CreateRooms method to create rooms with noise-based cutouts
    /// </summary>
    protected override void CreateRooms()
    {
        // Clear the previous room info list if we're regenerating
        roomInfos.Clear();
        
        // First, generate regular rooms from BSP leaf nodes
        foreach (var node in leafNodes)
        {
            RoomInfo roomInfo = node.CreateRoom(roomPrefab, 0, minRoomWidth, minRoomHeight, maxRoomWidth, maxRoomHeight);
            if (roomInfo != null)
            {
                // Apply noise cutout to the room
                ApplyNoiseCutoutToRoom(roomInfo);
                roomInfos.Add(roomInfo);
            }
        }
    }
    
    /// <summary>
    /// Applies noise-based cutouts to a room and creates a custom floor mesh
    /// </summary>
    private void ApplyNoiseCutoutToRoom(RoomInfo roomInfo)
    {
        if (roomInfo == null || roomInfo.RoomObj == null) return;
        
        // Get room dimensions
        int width = roomInfo.RoomRect.width;
        int height = roomInfo.RoomRect.height;
        
        // Generate the noise-based grid for this room
        bool[,] roomGrid = GenerateRoomWithNoiseCutout(width, height);
        
        // Store the grid in the room for later use (e.g., for navigation or other systems)
        roomInfo.SetGridData(roomGrid);
        
        // Create a custom mesh for the room based on the noise cutout grid
        CreateCustomRoomMesh(roomInfo, roomGrid);
    }
    
    /// <summary>
    /// Generate a room grid and remove parts using noise
    /// </summary>
    public bool[,] GenerateRoomWithNoiseCutout(int w, int h)
    {
        bool[,] grid = new bool[w, h];
        float seedX = Random.Range(0f, 9999f) + seedOffsetX;
        float seedY = Random.Range(0f, 9999f) + seedOffsetY;

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                // Generate Perlin noise value
                float noiseValue = Mathf.PerlinNoise((x + seedX) / noiseScale, (y + seedY) / noiseScale);
                
                // Make sure the room border is always solid
                if (x == 0 || y == 0 || x == w - 1 || y == h - 1)
                {
                    grid[x, y] = true; // Border is always solid
                }
                else
                {
                    grid[x, y] = noiseValue > noiseThreshold; // true = keep, false = cut out
                }
            }
        }

        return grid;
    }
    
    /// <summary>
    /// Create a custom mesh for the room based on the noise cutout grid
    /// </summary>
    private void CreateCustomRoomMesh(RoomInfo roomInfo, bool[,] grid)
    {
        // If meshGenerator is not assigned, try to find it
        if (meshGenerator == null)
        {
            meshGenerator = FindObjectOfType<RoomMeshGenerator>();
            if (meshGenerator == null)
            {
                Debug.LogError("RoomMeshGenerator not found! Please assign it in the inspector.");
                return;
            }
        }
        
        // Remove any existing children from the room object
        foreach (Transform child in roomInfo.RoomObj.transform)
        {
            #if UNITY_EDITOR
            if (Application.isEditor && !Application.isPlaying)
            {
                DestroyImmediate(child.gameObject);
            }
            else
            #endif
            {
                Destroy(child.gameObject);
            }
        }
        
        // Generate the room mesh with the standard method
        GameObject roomMesh = meshGenerator.GenerateRoomMesh(roomInfo);
        roomMesh.transform.SetParent(roomInfo.RoomObj.transform, false);
        
        // Now, we need to modify the floor mesh to match our noise cutout pattern
        // Find the floor object in the generated mesh
        Transform floorTransform = roomMesh.transform.Find("Floor");
        if (floorTransform == null)
        {
            Debug.LogError("Floor object not found in the generated room mesh!");
            return;
        }
        
        // Replace the floor mesh with our custom one based on the grid
        ReplaceFloorMesh(floorTransform.gameObject, grid, roomInfo.RoomRect);
        
        // Debug visualization (can be removed in final version)
        if (Application.isEditor)
        {
            PrintRoomToConsole(grid);
        }
    }
    
    /// <summary>
    /// Replace the standard floor mesh with a custom one based on the noise grid
    /// </summary>
    private void ReplaceFloorMesh(GameObject floorObject, bool[,] grid, RectInt roomRect)
    {
        MeshFilter meshFilter = floorObject.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter not found on floor object!");
            return;
        }
        
        // Create a new mesh for the cutout floor
        Mesh mesh = new Mesh();
        
        // Lists to store vertices, triangles and UVs
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        
        // For each grid cell that should be solid, create a quad (2 triangles)
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x, y]) // If this cell should be solid
                {
                    // Add vertices for this cell
                    int vertexIndex = vertices.Count;
                    
                    // Create a quad for this cell (clockwise order)
                    vertices.Add(new Vector3(x, 0, y)); // Bottom left
                    vertices.Add(new Vector3(x + 1, 0, y)); // Bottom right
                    vertices.Add(new Vector3(x, 0, y + 1)); // Top left
                    vertices.Add(new Vector3(x + 1, 0, y + 1)); // Top right
                    
                    // Add triangles (two triangles to form a quad)
                    triangles.Add(vertexIndex);
                    triangles.Add(vertexIndex + 2);
                    triangles.Add(vertexIndex + 1);
                    
                    triangles.Add(vertexIndex + 1);
                    triangles.Add(vertexIndex + 2);
                    triangles.Add(vertexIndex + 3);
                    
                    // Add UVs
                    uvs.Add(new Vector2((float)x / roomRect.width, (float)y / roomRect.height));
                    uvs.Add(new Vector2((float)(x + 1) / roomRect.width, (float)y / roomRect.height));
                    uvs.Add(new Vector2((float)x / roomRect.width, (float)(y + 1) / roomRect.height));
                    uvs.Add(new Vector2((float)(x + 1) / roomRect.width, (float)(y + 1) / roomRect.height));
                }
            }
        }
        
        // Assign vertices, triangles and UVs to the mesh
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        
        // Calculate normals and other mesh properties
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        
        // Assign the new mesh to the mesh filter
        meshFilter.mesh = mesh;
        
        // Update the mesh collider if there is one
        MeshCollider meshCollider = floorObject.GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            meshCollider.sharedMesh = mesh;
        }
    }
    
    /// <summary>
    /// Simple way to visualize the generated room in the console
    /// </summary>
    private void PrintRoomToConsole(bool[,] grid)
    {
        string output = "Noise Cutout Room Layout:\n";
        for (int y = grid.GetLength(1) - 1; y >= 0; y--) // Print top to bottom
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                output += grid[x, y] ? "#" : "."; // '#' = tile, '.' = empty
            }
            output += "\n";
        }

        Debug.Log(output);
    }
    
    /// <summary>
    /// Override the OnRoomsGenerated method to perform any additional actions
    /// </summary>
    protected override void OnRoomsGenerated()
    {
        // You can add custom behavior here for when rooms are generated
        Debug.Log($"Generated {roomInfos.Count} rooms with noise cutout patterns");
    }
}