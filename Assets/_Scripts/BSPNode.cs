using System.Collections.Generic;
using UnityEngine;

public class BSPNode
{
    public RectInt area;
    public BSPNode left;
    public BSPNode right;
    public RectInt room;

    public BSPNode(int x, int y, int width, int height)
    {
        area = new RectInt(x, y, width, height);
    }

    public void Split(int depth)
    {
        if (depth <= 0) return;

        bool splitHorizontally = Random.value > 0.5f;
        int splitPos = splitHorizontally
            ? Random.Range(5, area.height - 5)
            : Random.Range(5, area.width - 5);

        if (splitHorizontally)
        {
            left = new BSPNode(area.x, area.y, area.width, splitPos);
            right = new BSPNode(area.x, area.y + splitPos, area.width, area.height - splitPos);
        }
        else
        {
            left = new BSPNode(area.x, area.y, splitPos, area.height);
            right = new BSPNode(area.x + splitPos, area.y, area.width - splitPos, area.height);
        }

        left.Split(depth - 1);
        right.Split(depth - 1);
    }

    public List<BSPNode> GetLeafNodes()
    {
        List<BSPNode> leaves = new List<BSPNode>();
        if (left == null && right == null)
            leaves.Add(this);
        else
        {
            if (left != null) leaves.AddRange(left.GetLeafNodes());
            if (right != null) leaves.AddRange(right.GetLeafNodes());
        }
        return leaves;
    }

    
    
    public void CreateRoom(GameObject prefab, int floor, Transform parent)
    {
        int padding = 2; // Ensures room is not too close to the leaf edges

        int maxRoomWidth = area.width - padding * 2;
        int maxRoomHeight = area.height - padding * 2;

        // Ensure minimum room size
        if (maxRoomWidth < 4 || maxRoomHeight < 4)
            return;

        int roomWidth = Random.Range(4, maxRoomWidth);
        int roomHeight = Random.Range(4, maxRoomHeight);

        int roomX = area.x + Random.Range(padding, area.width - roomWidth - padding);
        int roomY = area.y + Random.Range(padding, area.height - roomHeight - padding);

        room = new RectInt(roomX, roomY, roomWidth, roomHeight);

        Vector3 pos = new Vector3(room.x + room.width / 2, floor * 5f, room.y + room.height / 2);
        GameObject newRoom = GameObject.Instantiate(prefab, pos, Quaternion.identity);
        newRoom.transform.localScale = new Vector3(room.width, 1, room.height);
        newRoom.transform.SetParent(parent);
    }

    public void CreateCorridors(GameObject prefab, int floor, Transform parent)
    {
        if (left != null && right != null)
        {
            Vector2Int leftCenter = left.GetRoomCenter();
            Vector2Int rightCenter = right.GetRoomCenter();

            Vector3 startPos = new Vector3(leftCenter.x, floor * 5f, leftCenter.y);
            Vector3 endPos = new Vector3(rightCenter.x, floor * 5f, rightCenter.y);

            Vector3 corridorPos = (startPos + endPos) / 2;
            float corridorLength = Vector3.Distance(startPos, endPos);

            GameObject corridor = GameObject.Instantiate(prefab, corridorPos, Quaternion.identity);
            corridor.transform.localScale = new Vector3(1, 1, corridorLength);
            corridor.transform.SetParent(parent);
            
            left.CreateCorridors(prefab, floor, parent);
            right.CreateCorridors(prefab, floor, parent);
        }
    }

    public Vector2Int GetRoomCenter()
    {
        if (room != null)
            return new Vector2Int(room.x + room.width / 2, room.y + room.height / 2);
        else
            return new Vector2Int(area.x + area.width / 2, area.y + area.height / 2);
    }
}