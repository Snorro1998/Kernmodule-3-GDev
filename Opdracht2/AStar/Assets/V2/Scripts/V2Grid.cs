using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class V2Grid : MonoBehaviour
{
	public Vector2 gridWorldSize;
	public float nodeRadius;
	public V2Node[,] grid;

	float nodeDiameter;
	int gridSizeX, gridSizeY;
    internal Vector3 worldBottomLeft;
    public List<V2Node> path;

    public Color defaultColor;
    public Color obstacleColor;
    public Color startPointColor;
    public Color endPointColor;
    public Color pathColor;

    private void Awake()
    {
		nodeDiameter = nodeRadius * 2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        CreateGrid();
	}

    /// <summary>
    /// Checks if the asked position is inside the grid.
    /// </summary>
    internal bool CheckInBounds(int x, int y)
    {
        return x >= 0 && x < gridSizeX && y >= 0 && y < gridSizeY;
    }

    /// <summary>
    /// Creates a new grid.
    /// </summary>
	private void CreateGrid()
    {
		grid = new V2Node[gridSizeX,gridSizeY];

		for (int x = 0; x < gridSizeX; x ++)
        {
			for (int y = 0; y < gridSizeY; y ++)
            {
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                int i = Random.Range(0, 3);
                V2Node.NodeMode mod = i == 0 ? V2Node.NodeMode.obstacle : V2Node.NodeMode.normal;
				grid[x,y] = new V2Node(mod, worldPoint, x,y);
			}
		}
	}

    /// <summary>
    /// Returns a list with all neigbouring nodes for a node.
    /// </summary>
	public List<V2Node> GetNeighbours(V2Node node)
    {
		List<V2Node> neighbours = new List<V2Node>();

		for (int x = -1; x <= 1; x++)
        {
			for (int y = -1; y <= 1; y++)
            {
				if (x == 0 && y == 0)
                {
                    continue;
                }			

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
					neighbours.Add(grid[checkX,checkY]);
				}
			}
		}

		return neighbours;
	}

    /// <summary>
    /// Updates the mode of the nodes from the previous path, so that they won't be shown.
    /// </summary>
    public void ClearPath()
    {
        if (path == null)
        {
            return;
        }

        foreach (V2Node nod in path)
        {
            if (nod.mode == V2Node.NodeMode.path)
            {
                nod.mode = V2Node.NodeMode.normal;
            }        
        }
    }

    /// <summary>
    /// Updates the mode of the nodes from the current path, so that they will be shown.
    /// </summary>
    public void setPath()
    {
        if (path == null)
        {
            return;
        }

        foreach (V2Node nod in path)
        {
            nod.mode = V2Node.NodeMode.path;
        }
    }

	//public List<V2Node> path;

	private void OnDrawGizmos()
    {
		Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

		if (grid != null)
        {
			foreach (V2Node n in grid)
            {
                Gizmos.color = GetColor(n.mode);
				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
			}
		}
	}

    /// <summary>
    /// Decides which color to use for drawing the nodes.
    /// </summary>
    private Color GetColor(V2Node.NodeMode mode)
    {
        Color col = Color.white;

        switch (mode)
        {
            case V2Node.NodeMode.obstacle:
                col = obstacleColor != null ? obstacleColor : Color.black;
                break;
            case V2Node.NodeMode.startPoint:
                col = startPointColor != null ? startPointColor : Color.red;
                break;
            case V2Node.NodeMode.endPoint:
                col = endPointColor != null ? endPointColor : Color.green;
                break;
            case V2Node.NodeMode.path:
                col = pathColor != null ? pathColor : Color.yellow;
                break;
        }

        return col;
    }
}