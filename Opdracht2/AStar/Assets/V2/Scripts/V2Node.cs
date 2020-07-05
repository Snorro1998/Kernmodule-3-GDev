using UnityEngine;
using System.Collections;

public class V2Node
{
	public Vector3 worldPosition;
	public int gridX;
	public int gridY;

	public int gCost;
	public int hCost;
	public V2Node parent;

    public enum NodeMode
    {
        normal,
        obstacle,
        startPoint,
        endPoint,
        path
    }

    public NodeMode mode;
	
	public V2Node(NodeMode _mode, Vector3 _worldPos, int _gridX, int _gridY)
    {
        mode = _mode;
		worldPosition = _worldPos;
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
