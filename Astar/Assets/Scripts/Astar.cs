using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Astar
{
    private List<Node> openList;    // list of Node that need to be evaluated
    private List<Node> closedList;  // list of Node that have already been evaluated

    /// <summary>
    /// TODO: Implement this function so that it returns a list of Vector2Int positions which describes a path from the startPos to the endPos
    /// Note that you will probably need to add some helper functions
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="grid"></param>
    /// <returns></returns>
    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {

        openList = new List<Node>();
        closedList = new List<Node>();

        Node currentNode = null;

        // add start node to OPEN list
        openList.Add(new Node(startPos, null, 0, Heuristic(startPos, endPos)));
        Debug.Log($"Start node: {openList[0].position}; h-cost: {openList[0].HScore}");

        // loop
        int count = 0;
        while (openList.Count > 0)
        {
            for (int index = 0; index < openList.Count; index++)
            {
                int lowestIndex = 0;
                for (int i = openList.Count - 1; i >= 0; i--)
                {
                    if (openList[i].FScore < openList[lowestIndex].FScore)
                    {
                        lowestIndex = i;
                    }
                    else if (openList[i].FScore == openList[lowestIndex].FScore)    // if f-score of node at index i is not smaller, but equal to the lowest f cost 
                    {
                        if (openList[i].HScore < openList[lowestIndex].HScore)      // get node with the lowest h-cost (node closest to the end) 
                        {
                            lowestIndex = i;
                        }
                    }
                }
                currentNode = openList[lowestIndex];

                // set current node to the node in Open with the lowest f-cost. (is startNode in first iteration)
                // If there are multiple of the same f-cost, choose from those with the lowest h-cost
            }

            Debug.Log($"Currentnode position: {currentNode.position}");
            Debug.Log($"Currentnode g-score: {currentNode.GScore}");

            // if current node == the end node, determine and return path
            if (currentNode.position == endPos)
            {
                return GetPath(currentNode);
            }

            // remove current node from Open, and add to Closed list
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            List<Cell> neighbours = grid[currentNode.position.x, currentNode.position.y].GetNeighbours(grid);

            for (int i = 0; i < neighbours.Count; i++)
            {
                Debug.Log($"neighbours[{i}]: {neighbours[i].gridPosition}");

                Node neighbourNode = new Node();
                neighbourNode.position = neighbours[i].gridPosition;

                bool isTraversable = true;

                if (CheckForWall(neighbourNode, currentNode, grid) && isTraversable) { isTraversable = false; }

                Debug.Log($"Neighbour isTraversable: {isTraversable}");

                if (isTraversable)   // only do the next loop if the cell is still seen as traversable
                {
                    for (int closedIndex = closedList.Count - 1; closedIndex >= 0; closedIndex--)    // loop over each node in closedList and see if the neighbour is in it
                    {
                        if (closedList[closedIndex].position == neighbours[i].gridPosition)
                        {
                            isTraversable = false;
                            break;
                        }
                    }
                }

                Debug.Log($"Neighbour isTraversable end: {isTraversable}");

                // check all neighbours
                // if neighbour is not traversable or in Closed list, continue with the next
                // if there is a wall in the way of the neigbour, continue as well

                if (!isTraversable) { continue; }

                int tempGScore = CalculateGScore(currentNode);
                Debug.Log($"Temp G-score: {tempGScore}");
                Debug.Log($"currentNode G-score: {currentNode.GScore}");

                bool contains = false;
                for (int openIndex = openList.Count - 1; openIndex >= 0; openIndex--)    // loop over each node in closedList and see if the neighbour is in it
                {
                    if (openList[openIndex].position == neighbourNode.position)
                    {
                        contains = true;
                        break;
                    }
                }

                if (tempGScore < neighbourNode.GScore || !contains)
                {
                    Debug.Log("Hello?");
                    Debug.Log($"Current node that is assigned to neighbour node: {currentNode}");
                    neighbourNode.parent = currentNode;
                    neighbourNode.GScore = tempGScore;
                    neighbourNode.HScore = Heuristic(startPos, endPos);
                }

                if (!contains) { openList.Add(neighbourNode); }

                // if neighbours new path is shorter (g-cost is lower than before) or is not in Open list
                // calculate and set an g- and h-cost (determine f-cost)
                // set parent of neighbour to current node
                // if neigbour is not in Open list, add to Open list

            }

            if (count > 100)
            {
                Debug.LogError($"While loop count surpassed 100 iterations, Exiting!");
                return null;
            }

            count++;
            
            Debug.Log($"openList count: {openList.Count}");
            foreach (Node node in openList)
            {
                Debug.Log($"Openlist node: {node.position}");
            }
        }


        Debug.Log($"Returning: null");
        Debug.Log($"Count: {count}");
        return null;
    }

    // gets the exact distance between the given position vectors
    private int Heuristic(Vector2Int _v1, Vector2Int _v2)
    {
        return (int)Mathf.Floor(Vector2Int.Distance(_v1, _v2));
    }

    private List<Vector2Int> GetPath(Node _endNode)
    {
        Debug.Log("getting path");

        // get end node in Closed list add it to a path list
        // check parent node and add parent node to path list
        // when start node is reached return the path list

        List<Vector2Int> path = new List<Vector2Int>();


        // reccursively add each position to the path list
        path = AddPosToPath(_endNode, path);

        foreach(Vector2Int pos in path)
        {
            Debug.Log($"Path: {pos.x}; {pos.y}");
        }

        path.Reverse();

        return path;
    }

    private List<Vector2Int> AddPosToPath(Node _current, List<Vector2Int> _path)
    {
        Debug.Log($"Reccursion bois!!");

        _path.Add(_current.position);

        if (_current.parent != null)
        {
            AddPosToPath(_current.parent, _path);
        }

        return _path;
    }

    private bool CheckForWall(Node _neighbour, Node _current, Cell[,] _grid)
    {
        // TODO - implement wall check

        // check the direction of the neighbours location.
        int relativeXPosition = _neighbour.position.x - _current.position.x;
        int relativeYPosition = _neighbour.position.y - _current.position.y;

        Wall neighbourDirection = Wall.LEFT;
        Wall currentDirection = Wall.DOWN;

        if (relativeXPosition == 0)  // check up and down
        {
            if (relativeYPosition > 0)
            {
                neighbourDirection = Wall.UP;
                currentDirection = Wall.DOWN;
            }
            else
            {
                neighbourDirection = Wall.DOWN;
                currentDirection = Wall.UP;
            }
        }
        else if (relativeYPosition == 0) // check left and right
        {
            if (relativeXPosition > 0)
            {
                neighbourDirection = Wall.RIGHT;
                currentDirection = Wall.LEFT;
            }
            else
            {
                neighbourDirection = Wall.LEFT;
                currentDirection = Wall.RIGHT;
            }
        }

        if (NodeToCell(_current, _grid).HasWall(neighbourDirection) || NodeToCell(_neighbour, _grid).HasWall(currentDirection)) 
        {
            return true;
        }

        return false;
    }

    private int CalculateGScore(Node _current) 
    {
        int gScore = (int)(_current.GScore + 1);
        return gScore;    // 1 = distance between the currently selected node and the selected neigbour neighbour
    }

    private Cell NodeToCell(Node _node, Cell[,] _grid)
    {
        return _grid[_node.position.x, _node.position.y];
    }

    /// <summary>
    /// This is the Node class you can use this class to store calculated FScores for the cells of the grid, you can leave this as it is
    /// </summary>
    public class Node
    {
        public Vector2Int position; //Position on the grid
        public Node parent; //Parent Node of this node

        public float FScore { //GScore + HScore
            get { return GScore + HScore; }
        }
        public float GScore; //Current Travelled Distance
        public float HScore; //Distance estimated based on Heuristic

        public Node() { }
        public Node(Vector2Int position, Node parent, int GScore, int HScore)
        {
            this.position = position;
            this.parent = parent;
            this.GScore = GScore;
            this.HScore = HScore;
        }
    }
}
