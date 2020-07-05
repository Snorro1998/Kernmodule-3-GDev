using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class V2Pathfinding : MonoBehaviour
{
	private V2Grid grid;
    private V2Node startNode, endNode;
    private V2Node.NodeMode editMode;

	private void Awake()
    {
		grid = GetComponent<V2Grid>();
	}

    /// <summary>
    /// Returns the absolute value of a Vector3.
    /// </summary>
    private Vector3 VectorAbs(Vector3 vec)
    {
        return new Vector3(Mathf.Abs(vec.x), Mathf.Abs(vec.y), Mathf.Abs(vec.z));
    }

    /// <summary>
    /// Returns true if the left mouse button was pressed.
    /// </summary>
    private bool LeftMouseButtonPressed
    {
        get { return Input.GetMouseButtonDown(0); }
    }

    /// <summary>
    /// Updates the editing mode depending on which key is pressed.
    /// </summary>
    private void UpdateEditMode()
    {
        editMode = V2Node.NodeMode.normal;

        if (Input.GetKey("q"))
        {
            editMode = V2Node.NodeMode.startPoint;
        }

        if (Input.GetKey("w"))
        {
            editMode = V2Node.NodeMode.endPoint;
        }

        if (Input.GetKey("a"))
        {
            editMode = V2Node.NodeMode.obstacle;
        }
    }

    /// <summary>
    /// Updates the current node.
    /// </summary>
    private void UpdateNode(ref V2Node node)
    {
        if (editMode == V2Node.NodeMode.startPoint)
        {
            if (startNode != null)
            {
                if (node != startNode)
                {
                    startNode.mode = V2Node.NodeMode.normal;
                }
            }

            startNode = node;
        }

        else if (editMode == V2Node.NodeMode.endPoint)
        {
            if (endNode != null)
            {
                if (node != endNode)
                {
                    endNode.mode = V2Node.NodeMode.normal;
                }
            }

            endNode = node;
        }

        node.mode = editMode;
    }

	private void Update()
    {
        bool somethingChanged = false;

        if (LeftMouseButtonPressed)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 pos = hit.point + VectorAbs(grid.worldBottomLeft);
                int xPos = Mathf.FloorToInt(pos.x);
                int yPos = Mathf.FloorToInt(pos.z);

                UpdateEditMode();

                if (grid.CheckInBounds(xPos, yPos))
                {
                    somethingChanged = true;
                    UpdateNode(ref grid.grid[xPos, yPos]);
                }
            }   
        }

        if (startNode == null || endNode == null || !somethingChanged)
        {
            return;
        }

		FindPath(startNode, endNode);
	}

    /// <summary>
    /// Attempts to find a path from startNode to targetNode using the A* algorithm.
    /// </summary>
	private void FindPath(V2Node startNode, V2Node targetNode)
    {
		List<V2Node> openSet = new List<V2Node>();
        List<V2Node> closedSet = new List<V2Node>();
		openSet.Add(startNode);

		while (openSet.Count > 0) {
			V2Node node = openSet[0];
			for (int i = 1; i < openSet.Count; i ++)
            {
				if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost)
                {
					if (openSet[i].hCost < node.hCost)
                    {
                        node = openSet[i];
                    }	
				}
			}

			openSet.Remove(node);
			closedSet.Add(node);

			if (node == targetNode)
            {
				RetracePath(startNode,targetNode);
				return;
			}

			foreach (V2Node neighbour in grid.GetNeighbours(node))
            {
				if (neighbour.mode == V2Node.NodeMode.obstacle || closedSet.Contains(neighbour))
                {
					continue;
				}

				int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
				if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
					neighbour.gCost = newCostToNeighbour;
					neighbour.hCost = GetDistance(neighbour, targetNode);
					neighbour.parent = node;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }		
				}
			}
		}
	}

    /// <summary>
    /// Gives a new path to the grid.
    /// </summary>
	private void RetracePath(V2Node startNode, V2Node endNode)
    {
        grid.ClearPath();

		List<V2Node> path = new List<V2Node>();
		V2Node currentNode = endNode.parent;

		while (currentNode != startNode)
        {
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}

		path.Reverse();
		grid.path = path;
        grid.setPath();
	}

    /// <summary>
    /// Returns the distance between nodeA and nodeB.
    /// </summary>
	private int GetDistance(V2Node nodeA, V2Node nodeB)
    {
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }
        
		return 14 * dstX + 10 * (dstY - dstX);
	}
}
