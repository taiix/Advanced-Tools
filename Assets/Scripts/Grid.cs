using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] private LayerMask unwalkable;
    [SerializeField] private GameObject gridSpawnObject;

    private int width;
    private int height;

    private Node[,] nodes;

    private Transform floor;
    private AStar aStar;

    private void Awake()
    {
        aStar = GetComponent<AStar>();
        GridGeneration();
    }

    /// <summary>
    /// Creates a grid of nodes. Walkability is detected with Physics.CheckBox
    /// against the provided unwalkable layer.
    /// </summary>
    void GridGeneration()
    {
        var floorGO = GameObject.FindGameObjectWithTag("Floor");
        if (floorGO == null)
        {
            Debug.LogError("Floor GameObject with tag 'Floor' not found.");
            return;
        }

        floor = floorGO.transform;

        if (aStar == null)
        {
            Debug.LogError("AStar component not found on the same GameObject.");
            return;
        }

        Bounds bounds = floor.GetComponent<Renderer>().bounds;

        if(bounds == null)
            {
            Debug.LogError("Floor GameObject does not have a Renderer component.");
            return;
        }

        width = Mathf.Max(1, Mathf.RoundToInt(bounds.size.x));
        height = Mathf.Max(1, Mathf.RoundToInt(bounds.size.z));

        aStar.openDictionary.Clear();

        nodes = new Node[width, height];

        int originX = Mathf.RoundToInt(bounds.center.x - width / 2f);
        int originZ = Mathf.RoundToInt(bounds.center.z - height / 2f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int cellPos = new Vector3Int(originX + x, 0, originZ + y);

                bool walkable = !Physics.CheckBox(
                    new Vector3(cellPos.x, 1f, cellPos.z),
                    new Vector3(.49f, .49f, .49f),
                    Quaternion.identity,
                    unwalkable
                );

                var node = new Node(cellPos, walkable);
                nodes[x, y] = walkable ? node : null;

                aStar.openDictionary[cellPos] = node;

                if (walkable && gridSpawnObject != null)
                {
                    Instantiate(gridSpawnObject, (Vector3)cellPos, Quaternion.identity, this.transform);
                }
            }
        }
    }
}