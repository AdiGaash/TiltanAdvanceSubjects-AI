using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scans the scene for quest-related content based on tags and layers.
/// </summary>
public class SceneContentScanner : MonoBehaviour
{
    [Header("Scan Settings")]
    public string enemyTag = "Enemy";
    public LayerMask questLocationLayer;

    public List<GameObject> foundEnemies = new List<GameObject>();
    public List<Transform> questLocations = new List<Transform>();

    /// <summary>
    /// Scans the scene and stores found enemies and locations.
    /// </summary>
    public void ScanScene()
    {
        foundEnemies.Clear();
        questLocations.Clear();

        // Find enemies by tag
        foundEnemies.AddRange(GameObject.FindGameObjectsWithTag(enemyTag));

        // Find quest locations by layer
        foreach (var obj in GameObject.FindObjectsOfType<Transform>())
        {
            if (((1 << obj.gameObject.layer) & questLocationLayer) != 0)
            {
                questLocations.Add(obj);
            }
        }

        Debug.Log($"[SceneContentScanner] Found {foundEnemies.Count} enemies and {questLocations.Count} quest locations.");
    }
}