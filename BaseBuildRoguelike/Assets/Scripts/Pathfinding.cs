using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Pathfinding
{
    public class Node
    {
        public Vector2Int pos;
        public bool is_wall;
        public Node parent; // Stores the parent node, allows for looping from the end node back to the beginning node

        public int g_cost;
        public int h_cost;
        public int f_cost { get { return g_cost + h_cost; } }

        public Node(bool _is_wall, Vector2Int _pos)
        {
            is_wall = _is_wall;
            pos = _pos;
        }
    }

    private static Node[,] nodeGrid;
    private static Grid mapGrid;
    public static void UpdateNodeGrid(Grid grid)
    {
        mapGrid = grid;
        Vector2Int size = new Vector2Int(grid.mapSize, grid.mapSize);
        nodeGrid = new Node[size.x, size.y];

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                bool isObstacle = false;
                if (grid.tiles[x, y].structure != null || grid.tiles[x, y].type == Tile.Type.water)
                {
                    isObstacle = true;
                }
                nodeGrid[x, y] = new Node(isObstacle, new Vector2Int(x, y));
            }
        }
    }

    public static void UpdateNodeGrid()
    {
        Vector2Int size = new Vector2Int(mapGrid.mapSize, mapGrid.mapSize);
        nodeGrid = new Node[size.x, size.y];

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                bool isObstacle = false;
                if (mapGrid.tiles[x, y].structure != null || mapGrid.tiles[x, y].type == Tile.Type.water)
                {
                    isObstacle = true;
                }
                nodeGrid[x, y] = new Node(isObstacle, new Vector2Int(x, y));
            }
        }
    }

    public static bool FindPath(ref List<Vector2Int> path, Vector2 start_pos, Vector2 end_pos)
    {

        path = IsPath(new Vector2Int((int)start_pos.x, (int)start_pos.y), new Vector2Int((int)end_pos.x, (int)end_pos.y));

        if (path.Count > 0)
        {
            return true;
        }
        return false;
    }


    private static List<Vector2Int> IsPath(Vector2Int start_pos, Vector2Int end_pos)
    {
        Node start_node = nodeGrid[start_pos.x, start_pos.y];
        Node end_node = nodeGrid[end_pos.x, end_pos.y];

        List<Node> open_list = new List<Node>();
        HashSet<Node> closed_list = new HashSet<Node>();

        open_list.Add(start_node);

        while (open_list.Count > 0)
        {
            // Looks at node at the bottom of the open list
            Node current_node = open_list[0];

            // Then look through all the following nodes in the open list
            for (int i = 1; i < open_list.Count; i++)
            {
                if ((open_list[i].f_cost < current_node.f_cost || open_list[i].f_cost == current_node.f_cost) && open_list[i].h_cost < current_node.h_cost)
                {
                    // If open list F Cost and H Cost is lower than the current node, it is closer to the goal
                    current_node = open_list[i];
                }
            }

            open_list.Remove(current_node);
            closed_list.Add(current_node);

            if (current_node == end_node)
            {
                // Path has been found
                return GetFinalPath(start_node, end_node);
            }

            foreach (Node neighbour in GetNeighbourNodes(nodeGrid, current_node))
            {
                // If the neighbour is a wall, or is on the closed list (already checked) skip over it
                if (neighbour.is_wall || closed_list.Contains(neighbour))
                {
                    continue;
                }

                int move_cost = current_node.g_cost + GetManhattenDistance(current_node, neighbour);
                if (move_cost < neighbour.g_cost || !open_list.Contains(neighbour))
                {
                    neighbour.g_cost = move_cost;
                    neighbour.h_cost = GetManhattenDistance(neighbour, end_node);
                    neighbour.parent = current_node;

                    if (!open_list.Contains(neighbour))
                    {
                        open_list.Add(neighbour);
                    }
                }
            }
        }
        // Path has not been found
        return new List<Vector2Int>();
    }

    private static int GetManhattenDistance(Node node_a, Node node_b)
    {
        int x_dist = Mathf.Abs(node_a.pos.x - node_b.pos.x);
        int y_dist = Mathf.Abs(node_a.pos.y - node_b.pos.y);
        return x_dist + y_dist;
    }

    private static List<Node> GetNeighbourNodes(Node[,] nodeGrid, Node node)
    {
        // Gets all neighbouring nodes (ensuring none are outside of the grid)
        List<Node> neighbour_nodes = new List<Node>();

        // Check up
        if (node.pos.x >= 0 && node.pos.x < nodeGrid.GetLength(0) &&
            node.pos.y - 1 >= 0 && node.pos.y - 1 < nodeGrid.GetLength(1))
        {
            neighbour_nodes.Add(nodeGrid[node.pos.x, node.pos.y - 1]);
        }
        // Check right
        if (node.pos.x + 1 >= 0 && node.pos.x + 1 < nodeGrid.GetLength(0) &&
            node.pos.y >= 0 && node.pos.y < nodeGrid.GetLength(1))
        {
            neighbour_nodes.Add(nodeGrid[node.pos.x + 1, node.pos.y]);
        }
        // Check down
        if (node.pos.x >= 0 && node.pos.x < nodeGrid.GetLength(0) &&
            node.pos.y + 1 >= 0 && node.pos.y + 1 < nodeGrid.GetLength(1))
        {
            neighbour_nodes.Add(nodeGrid[node.pos.x, node.pos.y + 1]);
        }
        // Check left
        if (node.pos.x - 1 >= 0 && node.pos.x - 1 < nodeGrid.GetLength(0) &&
            node.pos.y >= 0 && node.pos.y < nodeGrid.GetLength(1))
        {
            neighbour_nodes.Add(nodeGrid[node.pos.x - 1, node.pos.y]);
        }

        return neighbour_nodes;
    }

    private static List<Vector2Int> GetFinalPath(Node start_node, Node end_node)
    {
        List<Vector2Int> pos_path = new List<Vector2Int>();
        Node current_node = end_node;

        // Work backwards from the end node to the start node to create the final path
        while (current_node != start_node)
        {
            pos_path.Add(current_node.pos);
            current_node = current_node.parent;
        }

        // Path needs to be flipped (worked backwards from the end)
        pos_path.Reverse();
        return pos_path;
    }
}
