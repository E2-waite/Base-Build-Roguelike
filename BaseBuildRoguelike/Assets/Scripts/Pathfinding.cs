using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pathfinding
{
    public class Node
    {
        public Vector2Int pos;
        public bool isWall;
        public Node parent; // Stores the parent node, allows for looping from the end node back to the beginning node

        public int gCost;
        public int hCost;
        public int fCost { get { return gCost + hCost; } }

        public Node(bool _isWall, Vector2Int _pos)
        {
            isWall = _isWall;
            pos = _pos;
        }
    }

    private static Node[,] nodeGrid;

    public static void UpdateNodeGrid()
    {
        nodeGrid = new Node[Grid.size, Grid.size];

        for (int y = 0; y < Grid.size; y++)
        {
            for (int x = 0; x < Grid.size; x++)
            {
                bool isObstacle = false;
                Tile tile = Grid.tiles[x, y];
                if ((tile.structure != null && (tile.structure is Resource || (tile.structure is Building && (tile.structure as Building).isConstructed))) || tile.type == Tile.Type.water)
                {
                    isObstacle = true;
                }
                nodeGrid[x, y] = new Node(isObstacle, new Vector2Int(x, y));
            }
        }
    }

    public static bool FindPath(ref List<Vector2Int> path, Vector2 startPos, Vector2Int endPos, int maxDist = 0)
    {
        path = IsPath(new Vector2Int((int)startPos.x, (int)startPos.y), endPos, maxDist);

        if (path.Count > 0)
        {
            return true;
        }
        return false;
    }


    private static List<Vector2Int> IsPath(Vector2 startPos, Vector2Int endPos, int maxDist)
    {
        Node startNode = nodeGrid[(int)startPos.x, (int)startPos.y];
        if (startNode.isWall)
        {
            //return new List<Vector2Int>();
            startNode = nodeGrid[Mathf.RoundToInt(startPos.x), Mathf.RoundToInt(startPos.y)];
        }
        Node endNode = nodeGrid[endPos.x, endPos.y];

        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            // Looks at node at the bottom of the open list
            Node current_node = openList[0];

            // Then look through all the following nodes in the open list
            for (int i = 1; i < openList.Count; i++)
            {
                if ((openList[i].fCost < current_node.fCost || openList[i].fCost == current_node.fCost) && openList[i].hCost < current_node.hCost)
                {
                    // If open list F Cost and H Cost is lower than the current node, it is closer to the goal
                    current_node = openList[i];
                }
            }

            openList.Remove(current_node);
            closedList.Add(current_node);


            if (maxDist == 0)
            {
                if (current_node == endNode)
                {
                    // Path has been found
                    return GetFinalPath(startNode, endNode);
                }
            }
            else
            {
                int dist = Mathf.Abs(current_node.pos.x - endNode.pos.x) + Mathf.Abs(current_node.pos.y - endNode.pos.y);
                if (dist <= maxDist)
                {
                    return GetFinalPath(startNode, current_node);
                }
            }

            foreach (Node neighbour in GetNeighbourNodes(nodeGrid, current_node))
            {
                // If the neighbour is a wall, or is on the closed list (already checked) skip over it
                if (neighbour.isWall || closedList.Contains(neighbour))
                {
                    continue;
                }

                int move_cost = current_node.gCost + GetManhattenDistance(current_node, neighbour);
                if (move_cost < neighbour.gCost || !openList.Contains(neighbour))
                {
                    neighbour.gCost = move_cost;
                    neighbour.hCost = GetManhattenDistance(neighbour, endNode);
                    neighbour.parent = current_node;

                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                }
            }
        }
        // Path has not been found
        return new List<Vector2Int>();
    }

    private static int GetManhattenDistance(Node nodeA, Node nodeB)
    {
        int xDist = Mathf.Abs(nodeA.pos.x - nodeB.pos.x);
        int yDist = Mathf.Abs(nodeA.pos.y - nodeB.pos.y);
        return xDist + yDist;
    }

    private static List<Node> GetNeighbourNodes(Node[,] nodeGrid, Node node)
    {
        // Gets all neighbouring nodes (ensuring none are outside of the grid)
        List<Node> neighbourNodes = new List<Node>();

        Vector2Int[] neighbourPos = Params.Get8Neighbours(node.pos);

        for (int i = 0; i < neighbourPos.Length; i++)
        {
            Vector2Int pos = neighbourPos[i];
            if (pos.x >= 0 && pos.x < Grid.size &&
                pos.y >= 0 && pos.y < Grid.size)
            {
                neighbourNodes.Add(nodeGrid[pos.x, pos.y]);
            }
        }

        return neighbourNodes;
    }

    private static List<Vector2Int> GetFinalPath(Node startNode, Node endNode)
    {
        List<Vector2Int> posPath = new List<Vector2Int>();
        Node currentNode = endNode;

        // Work backwards from the end node to the start node to create the final path
        while (currentNode != startNode)
        {
            posPath.Add(currentNode.pos);
            currentNode = currentNode.parent;
        }

        // Path needs to be flipped (worked backwards from the end)
        posPath.Reverse();
        return posPath;
    }
}
