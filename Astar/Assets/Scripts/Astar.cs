using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Astar
{
    private List<Node> Open;    // list of Node that need to be evaluated
    private List<Node> Closed;  // list of Node that have already been evaluated

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
        // add start node to OPEN list
        
        // loop over Open list

        // set current node to the node in Open with the lowest f-cost. (is startNode in first iteration)
            // If there are multiple of the same f-cost, choose from those with the lowest h-cost
            
        // if current node == the end node, determine and return path
            // get end node in Closed list add it to a path list
            // check parent node and add parent node to path list
            // when start node is reached return the path list

        // remove current node from Open, and add to Closed list

        // check all neighbours
            // if neighbour is not traversable or in Closed list, continue with the next
        
        // if neighbours new path is shorter (f-cost is lower than before) or is not in Open list
            // calculate and set an g- and h-cost (determine f-cost)
            // set parent of neighbour to current node
            // if neigbour is not in Open list, add to Open list

        return null;
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
