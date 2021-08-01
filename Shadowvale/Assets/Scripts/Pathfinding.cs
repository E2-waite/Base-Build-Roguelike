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

    private static Node[,] followerGrid, enemyGrid;
    public static void UpdateNodeGrid()
    {
        followerGrid = new Node[Grid.size, Grid.size];
        enemyGrid = new Node[Grid.size, Grid.size];
        if (Grid.tiles != null)
        {
            for (int y = 0; y < Grid.size; y++)
            {
                for (int x = 0; x < Grid.size; x++)
                {
                    Tile tile = Grid.tiles[x, y];
                    if (tile == null)
                    {
                        followerGrid[x, y] = new Node(true, new Vector2Int(x, y));
                        enemyGrid[x, y] = new Node(true, new Vector2Int(x, y));
                        continue;
                    }
                    else
                    {
                        if (tile.type == Tile.Type.water)
                        {
                            followerGrid[x, y] = new Node(true, new Vector2Int(x, y));
                            enemyGrid[x, y] = new Node(true, new Vector2Int(x, y));
                        }
                        else if (tile.structure == null)
                        {
                            followerGrid[x, y] = new Node(false, new Vector2Int(x, y));
                            enemyGrid[x, y] = new Node(false, new Vector2Int(x, y));
                        }
                        else
                        {
                            if (tile.structure is Resource)
                            {
                                followerGrid[x, y] = new Node(true, new Vector2Int(x, y));
                                enemyGrid[x, y] = new Node(true, new Vector2Int(x, y));
                            }
                            else if (tile.structure is Building && (tile.structure as Building).isConstructed)
                            {
                                followerGrid[x, y] = new Node(true, new Vector2Int(x, y));
                                enemyGrid[x, y] = new Node(false, new Vector2Int(x, y));
                            }
                            else
                            {
                                followerGrid[x, y] = new Node(false, new Vector2Int(x, y));
                                enemyGrid[x, y] = new Node(false, new Vector2Int(x, y));
                            }
                        }
                    }

                }
            }

            // Update all AI paths after updating grid
            for (int i = 0; i < Followers.followers.Count; i++)
            {
                if (Followers.followers[i] != null)
                {
                    Followers.followers[i].FindPath();
                }
            }

            for (int i = 0; i < Enemies.enemies.Count; i++)
            {
                if (Enemies.enemies[i] != null)
                {
                    Enemies.enemies[i].FindPath();
                }
            }

            for (int i = 0; i < Creatures.creatures.Count; i++)
            {
                if (Creatures.creatures[i] != null)
                {
                    (Creatures.creatures[i] as Creature).FindPath();
                }
            }
        }
    }

    public static bool FindPath(ref List<Vector2Int> path, Vector2 startPos, Vector2Int endPos, int maxDist = 0)
    {

        path = IsPath(new Vector2Int((int)startPos.x, (int)startPos.y), endPos, maxDist, followerGrid);

        if (path.Count > 0)
        {
            return true;
        }
        return false;
    }


    public static bool FindPath(ref List<Vector2Int> path, ref List<Action> newTargets, Vector2 startPos, Vector2Int endPos, int maxDist = 0)
    {
        // Attempt to find path, around any walls etc.
        path = IsPath(new Vector2Int((int)startPos.x, (int)startPos.y), endPos, maxDist, followerGrid);

        if (path.Count > 0)
        {
            return true;
        }

        // If no clear path exists, return path including buildings and return a list of building targets
        path = IsPath(new Vector2Int((int) startPos.x, (int) startPos.y), endPos, maxDist, enemyGrid);
        if (path.Count > 0)
        {
            for (int i = path.Count - 1; i >= 0; i--)
            {
                Tile tile = Grid.tiles[path[i].x, path[i].y];
                if (tile.structure != null)
                {
                    path.RemoveRange(i, path.Count - i);
                    newTargets.Add(new Action(new Target(tile.structure)));
                }
            }
            return true;
        }
        return false;
    }

    private static List<Vector2Int> IsPath(Vector2 startPos, Vector2Int endPos, int maxDist, Node[,] grid)
    {
        Node startNode = grid[(int)startPos.x, (int)startPos.y];
        if (startNode.isWall)
        {
            //return new List<Vector2Int>();
            startNode = grid[Mathf.RoundToInt(startPos.x), Mathf.RoundToInt(startPos.y)];
        }
        Node endNode = grid[endPos.x, endPos.y];

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

            foreach (Node neighbour in GetNeighbourNodes(grid, current_node))
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
