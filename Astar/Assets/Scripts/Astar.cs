using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Astar
{
    private List<Node> openList;    // list of nodes to be evaluated
    private List<Node> closedList;  // list of nodes that have been evaluated

    /// <summary>
    /// TODO: Implement this function so that it returns a list of Vector2Int positions which describes a path from the startPos to the endPos
    /// <br></br>
    /// Note that you will probably need to add some helper functions
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="grid"></param>
    /// <returns></returns>
    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        openList = new List<Node>();    // initialize a list of nodes to be evaluated
        closedList = new List<Node>();  // initialize a list of nodes that have been evaluated

        Node currentNode = null;        // initialize the node that a single iteration looks at

        // add start node to OPEN list
        openList.Add(new Node(startPos, null, 0, Heuristic(startPos, endPos)));

        // loop
        int count = 0;
        while (openList.Count > 0)
        {
            // set the current node to the node in openList with the lowest f-score. (is startNode in first iteration)
                // if there are multiple nodes with the same f-score, choose from the node with the lowest h-score
            for (int index = 0; index < openList.Count; index++)
            {
                int lowestIndex = 0;
                for (int i = openList.Count - 1; i >= 0; i--)
                {
                    if (openList[i].FScore < openList[lowestIndex].FScore)
                    {
                        lowestIndex = i;
                    }
                    else if (openList[i].FScore == openList[lowestIndex].FScore)    // if f-score of node at index i is not smaller, but equal to the lowest f-score 
                    {
                        if (openList[i].HScore < openList[lowestIndex].HScore)      // get node with the lowest h-score (node closest to the end) 
                        {
                            lowestIndex = i;
                        }
                    }
                }
                currentNode = openList[lowestIndex];

            }

            // if current node == the end node (algorithm reached the end), determine and return a path
            if (currentNode.position == endPos)
            {
                return GetPath(currentNode);
            }

            // remove current node from Open, and add to Closed list
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            // Neighbour handling
            List<Cell> neighbours = NodeToCell(currentNode, grid).GetNeighbours(grid);
            for (int i = 0; i < neighbours.Count; i++)
            {
                // create temporary node for neighbour
                Node neighbourNode = new Node();
                neighbourNode.position = neighbours[i].gridPosition;

                // if there is a wall in between the current- and neighbouring node, continue to the next neighbour
                    // NOTE - I initially changed the traversable boolean here as well, but I now continue immediately for a performance improvement.
                if (CheckForWall(neighbourNode, currentNode, grid)) { continue; }

                // if closedList does not contain the current neighbour, continue to the next neighbour
                bool isTraversable = true;
                for (int closedIndex = closedList.Count - 1; closedIndex >= 0; closedIndex--)    // loop over each node in closedList and see if the neighbour is in it
                {
                    if (closedList[closedIndex].position == neighbours[i].gridPosition)
                    {
                        isTraversable = false;
                        break;
                    }
                }
                if (!isTraversable) { continue; }

                // if the current neighbour is not in openList, contains is set false
                    // TODO - continue out of this neighbour iteration
                bool contains = false;
                for (int openIndex = openList.Count - 1; openIndex >= 0; openIndex--)    // loop over each node in closedList and see if the neighbour is in it
                {
                    if (openList[openIndex].position == neighbourNode.position)
                    {
                        contains = true;
                        break;
                    }
                }

                // if current neighbours new path is shorter (g-score is lower than before) or openList does not contain the current neighbour
                int tempGScore = CalculateGScore(currentNode);
                if (tempGScore < neighbourNode.GScore || !contains)
                {
                    // set parent of neighbour to current node
                    neighbourNode.parent = currentNode;
                    // calculate and set a g- and h-score (determine f-score)
                    neighbourNode.GScore = tempGScore;
                    neighbourNode.HScore = Heuristic(startPos, endPos);
                }

                // if neigbour is not yet in Open list, add to Open list
                if (!contains) { openList.Add(neighbourNode); }
            }

            // while exit
            if (count > 999) 
            {
                Debug.LogWarning("While loop exceeded 9999 iterations; Exiting!");
                return GetPath(currentNode);
            }

            count++;
        }

        // if open list is empty, return path from latest point
            // TODO - find lowest f-score in closed list and return a path there
        return GetPath(currentNode);
    }

    /// <summary>
    /// returns the g-score for a node with specified parent node
    /// </summary>
    /// <param name="_parent"></param>
    /// <returns></returns>
    private int CalculateGScore(Node _parent)
    {
        int gScore = (int)(_parent.GScore + 1); // 1 = distance between two nodes
        return gScore;
    }

    /// <summary>
    /// Heuristic that returns the exact distance between the specified positions
    /// <br></br>
    /// Used to determine the h-score of a node
    /// </summary>
    /// <param name="_v1"></param>
    /// <param name="_v2"></param>
    /// <returns></returns>
    private int Heuristic(Vector2Int _v1, Vector2Int _v2)
    {
        return (int)Mathf.Floor(Vector2Int.Distance(_v1, _v2));
    }

    /// <summary>
    /// Returns a list of grid positions of specified nodes and their respective parents
    /// </summary>
    /// <param name="_endNode"></param>
    /// <returns></returns>
    private List<Vector2Int> GetPath(Node _endNode)
    {
        List<Vector2Int> path = new List<Vector2Int>();

        // reccursively add each position to the path list
        path = AddPosToPath(_endNode, path);

        path.Reverse(); // reverse list, so list starts from the start position instead of the end position

        return path;
    }

    /// <summary>
    /// Recursive function that adds node position to specified list and calls itself while passing in its parent node
    /// </summary>
    /// <param name="_node"></param>
    /// <param name="_path"></param>
    /// <returns></returns>
    private List<Vector2Int> AddPosToPath(Node _node, List<Vector2Int> _path)
    {
        _path.Add(_node.position);

        if (_node.parent != null)
        {
            AddPosToPath(_node.parent, _path);
        }

        return _path;
    }

    /// <summary>
    /// returns true when there is a wall between the two specified nodes
    /// </summary>
    /// <param name="_neighbour"></param>
    /// <param name="_current"></param>
    /// <param name="_grid"></param>
    /// <returns></returns>
    private bool CheckForWall(Node _neighbour, Node _current, Cell[,] _grid)
    {
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

        // if there is a wall in between the current and neigbouring tile, return true
        if (NodeToCell(_current, _grid).HasWall(neighbourDirection) || NodeToCell(_neighbour, _grid).HasWall(currentDirection)) 
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// returns cell at position of the specified node
    /// </summary>
    /// <param name="_node"></param>
    /// <param name="_grid"></param>
    /// <returns></returns>
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
