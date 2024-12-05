using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGraph : MonoBehaviour
{
    public LayerMask platformLayer; 
    public Vector2Int gridWorldSize = new Vector2Int(10, 10);
    public float raycast_distance = 2f;
    public float nodeRadius = 0.4f; 
    public Node[,] nodes; 

    private float nodeDiameter;
    internal int gridSizeX, gridSizeY;

    public Transform amelia_transform;

    public TeamRAstar ai_target;

    internal List<Node> path;

    private void Start()
    {
        //InvokeRepeating("InitializeGraph", 0, 1f);

        amelia_transform = GameObject.Find("New Amelia AI").transform;
        InitializeGraph();
    }

    internal void InitializeGraph()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY= Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    void CreateGrid()
    {
        nodes = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                bool walkable = IsNodeWalkable(worldPoint);
                nodes[x, y] = new Node(worldPoint,walkable,x,y);
            }
        }

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                if( x > 0 && y > 0 && x < gridSizeX-1 && y < gridSizeY-1)
                {
                    if (nodes[x,y].isWalkable)
                    {
                        if (!nodes[x+1,y].isWalkable && !nodes[x,y-1].isWalkable)
                        {
                            create_fall_nodes(x + 1, y);
                        }
                        if (!nodes[x-1,y].isWalkable && !nodes[x,y-1].isWalkable)
                        {
                            create_fall_nodes(x - 1, y);
                        }
                    }
                }
            }
        }
    }

    private void create_fall_nodes(int x,int y)
    {
        while(true)
        {
            if(y > 0 && !nodes[x,y].isWalkable && isInsideCollider(nodes[x,y].position))
            {
                nodes[x, y].isWalkable = true;
                y--;
            }
            else
                break;
        }
    }

    private bool isInsideCollider(Vector3 position)
    {
        return !(Physics2D.OverlapCircle(position, nodeRadius, platformLayer));
    }

    private bool IsNodeWalkable(Vector3 position)
    {
        bool isinsideCollider = isInsideCollider(position);
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down,raycast_distance, platformLayer);

        bool iswalkable = isinsideCollider && hit.collider != null;

        return iswalkable;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float x_offset = gridWorldSize.x / 2 + nodeRadius;
        float y_offset = gridWorldSize.y / 2 + nodeRadius;

        int x = Mathf.Clamp(Mathf.FloorToInt((worldPosition.x - transform.position.x + x_offset) / nodeDiameter), 0, gridSizeX - 1);
        int y = Mathf.Clamp(Mathf.FloorToInt((worldPosition.y - transform.position.y + y_offset) / nodeDiameter), 0, gridSizeY - 1);

        return nodes[x, y];
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for(int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 & y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if(checkX >= 0 && checkX < gridSizeX & checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(nodes[checkX,checkY]);
                }

            }
        }

        return neighbours;
    }


    private void OnDrawGizmos()
    {
        DrawNodeGraph();
        DrawGrid();
    }
    private void DrawNodeGraph()
    {
        if (nodes != null)
        {
            Node playerNode = NodeFromWorldPoint(amelia_transform.position);
            foreach (Node node in nodes)
            {
                Gizmos.color = node.isWalkable ? Color.green : Color.red;
                if (path != null)
                    if (path.Contains(node))
                        Gizmos.color = Color.blue;
                if (playerNode == node)
                    Gizmos.color = Color.magenta;
                if (ai_target.targetNode == node)
                    Gizmos.color = Color.white;
                if (ai_target.checkNode == node)
                    Gizmos.color = Color.yellow;

                Gizmos.DrawWireCube(node.position, Vector3.one * (nodeDiameter-0.1f)); // Visualize nodes as small cubes
            }
        }
    }

    private void DrawGrid()
    {
        Gizmos.color = Color.blue;
        Vector3 gridSizeInWorld = new Vector3(gridWorldSize.x, gridWorldSize.y, 0);
        Gizmos.DrawWireCube(transform.position, gridSizeInWorld);
    }
}
public class Node
{
    public Vector3 position;
    public bool isWalkable;

    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;

    public Node parent;

    public Node(Vector3 pos, bool walkable, int _gridX, int _gridY)
    {
        position = pos;
        isWalkable = walkable;
        gridX = _gridX;
        gridY = _gridY;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}
