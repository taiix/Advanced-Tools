using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    //hCOST = from start to end pos
    //gCOST = distance this node to next node
    private readonly Vector3[] dirs = { new Vector3(-1, 0, 0), new Vector3(0, 0, -1), new Vector3(1, 0, 0), new Vector3(0, 0, 1),
                                new Vector3(-1, 0, 1), new Vector3(-1, 0, -1), new Vector3(1, 0, -1), new Vector3(1, 0, 1)};

    public GameObject start;
    public GameObject end;
    public Dictionary<Vector3, Node> openDictionary = new Dictionary<Vector3, Node>();
    public GameObject searchingPathObj, displayPathObj;

    Vector3 startPos;
    Vector3 endPos;

    List<Node> closeList = new List<Node>();

    List<Node> checkList = new List<Node>();

    List<Vector3> path = new();

    Node currentNode = null;

    bool no = true;

    int steps = 0; 
    Vector3 endSgot;


    private void Awake()
    {
        startPos = start.transform.position;
        endPos = end.transform.position;
    }

    private void Start()
    {
        endSgot = endPos;
        openDictionary[startPos].gCost = 0;
        openDictionary[startPos].hCost = 0;

        checkList.Add(openDictionary[startPos]);
        currentNode = checkList[0];
    }

    private void Update()
    {
        for (int i = 0; i < 10; i++)
        {
            if (currentNode.worldPos != endPos)
            {
                currentNode = SearchPath(currentNode);
                Instantiate(searchingPathObj, currentNode.worldPos, Quaternion.identity);
                no = true;

                steps++;
            }
            else if (no)
            {
                if (steps > 0)
                {
                    Debug.Log("Computation steps taken: " + steps);
                    steps = -1;
                }

                DisplayPath();
            }
        }
    }
    /// <summary>
    /// 
    /// The first part checks for current node's neighbours with checking positions
    /// It is using 8 positions so it can check diagonally
    /// 
    /// If the neighbour node is in openDictionary which is a dictionary with all the nodes as value and Vector3 as a Key
    /// and if it is not in closedList which means if already has been checked
    /// 
    /// If the new cost is lower than current cost set the new cost and if it is not in checkList, add it
    /// 
    /// Check the checkList for a node with lower cost, if there is one, set it as current node
    /// 
    /// </summary>
    /// <param name="current"></param>
    /// <returns></returns>
    Node SearchPath(Node current)
    {
        // loop through all 8 neighbors of the currentNode, and update their (h)cost:
        for (int i = 0; i < 8; i++)
        {
            Vector3Int p = new Vector3Int((int)(current.worldPos.x + dirs[i].x), 0, (int)(current.worldPos.z + dirs[i].z));

            if (openDictionary.ContainsKey(p) && !closeList.Contains(openDictionary[p]))
            {
                Node neighbor = openDictionary[p];
                
                if (!neighbor.isWalkable || closeList.Contains(neighbor))
                    continue;

                if (neighbor.gCost <= 0) // here we initialize the gCost
                {
                    neighbor.gCost = GetDistanceEuclidian(neighbor, openDictionary[endPos]);
                }

                // change gCost to hCost...:
                float newCostToNeighbour = current.hCost + GetDistance(current, neighbor);

                if (newCostToNeighbour < neighbor.hCost || !checkList.Contains(neighbor))
                {
                    neighbor.hCost = newCostToNeighbour;
                    neighbor.parent = current;
                }

                if (!checkList.Contains(neighbor))
                {
                    checkList.Add(neighbor);
                }
            }
        }

        closeList.Add(openDictionary[current.worldPos]);

        // Find the node with the lowest fCost:

        current = checkList[0];

        for (int i = 0; i < checkList.Count; i++)
        {
            if (checkList[i].fCost <= current.fCost)
            {
                current = checkList[i];
            }
        }
        closeList.Add(current);
        checkList.Remove(current);
        return current;
    }

    /// <summary>
    /// Returns the Euclidian distance between two points (=Pythagoras, like we REMEMBER from physics)
    /// </summary>
    /// <param name="currentNode"></param>
    /// <param name="nextNode"></param>
    /// <returns></returns>
    float GetDistanceEuclidian(Node currentNode, Node nextNode)
    {
        float dx = Mathf.Abs(currentNode.worldPos.x - nextNode.worldPos.x);
        float dy = Mathf.Abs(currentNode.worldPos.z - nextNode.worldPos.z);

        return Mathf.Sqrt(dx * dx + dy * dy);
    }


    /// <summary>
    /// Returns the "8-neighbor-grid" distance between two points 
    /// </summary>
    /// <param name="currentNode"></param>
    /// <param name="nextNode"></param>
    /// <returns></returns>
    float GetDistance(Node currentNode, Node nextNode)
    {
        float dx = Mathf.Abs(currentNode.worldPos.x - nextNode.worldPos.x);
        float dy = Mathf.Abs(currentNode.worldPos.z - nextNode.worldPos.z);

        return (dx + dy) + (Mathf.Sqrt(2) - 2) * Mathf.Min(dx, dy);
    }


    /// <summary>
    /// Display the path if there is one
    /// </summary>
    void DisplayPath()
    {
        int steps = 0;
        while (endSgot != startPos)
        {
            GameObject go = Instantiate(displayPathObj, new Vector3(endSgot.x, endSgot.y + 1, endSgot.z), Quaternion.identity);
            endSgot = openDictionary[endSgot].parent.worldPos;

            path.Add(endSgot);
            if (steps == 200)
                return;
            steps++;
        }

        EventManager.OnPathReceived?.Invoke(path);
        Debug.Log("TOTAL STEPS TO FIND THE GOAL = " + steps);
        no = false;
    }
}