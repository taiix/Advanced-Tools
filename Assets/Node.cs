using UnityEngine;

public class Node
{
    public Vector3Int worldPos;
    public bool isWalkable;

    public Node parent;

    /// <summary>
    /// hCost is the length of the shortest path from start to current. 
    /// For nodes in the closed set, it is the proven shortest path length,
    /// for other nodes it's the current best found path.
    /// </summary>
    public float hCost;

    /// <summary>
    /// This is the estimated cost from the node to the target (end node).
    /// It is a lower bound for the real shortest path length.
    /// </summary>
    public float gCost = 0;

    /// <summary>
    /// fCost is a heuristic that we use to select the next node in the closed set.
    /// </summary>
    public float fCost { get { return gCost + hCost; } }

    public Node(Vector3Int worldPos, bool walkable)
    {
        this.worldPos = worldPos;
        this.isWalkable = walkable;
    }
}