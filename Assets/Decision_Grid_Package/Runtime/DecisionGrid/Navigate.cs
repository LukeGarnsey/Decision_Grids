namespace DecisionGrid{
    using System.Collections.Generic;
    using UnityEngine;
    public class Navigate{
        private const int MOVE_COST = 10;
        private const int MOVE_COST_DIAG = 14;

        private readonly GridCollectionBase _gridCollection;
        // private readonly GridNode _startNode;
        // private readonly GridNode _endNode;

        public List<GridNode> Path {get; private set;}
        /// <summary>
        /// If the hCost goes up this many times in a row, break out of the loop.
        /// </summary>
        /// <value></value>
        private int _breakAtHCostGoingUpAmount => (int)NavPersistence;
        /// <summary>
        /// How persistent FindPath() will be if direct routes from start to end are not found.
        /// </summary>
        /// <value></value>
        public NavigationPersistence NavPersistence {get; set;} = NavigationPersistence.Medium;
        public Navigate(GridCollectionBase gridCollection){
            _gridCollection = gridCollection;
        }
        /// <summary>
        /// Build a list of Grid Nodes that represent a path to the end.
        /// </summary>
        /// <param name="nodes">Nodes calculate path from.</param>
        /// <param name="start">Start of path.</param>
        /// <param name="end">End of path.</param>
        /// <param name="checkFor">Have function check for grid node with value. Corresponds with penalty</param>
        /// <param name="penalty">Move cost penalty to apply if CheckFor finds node with value. Value of 0 means it will avoid.</param>
        public void FindPath(List<GridNode> nodes, Vector3 start, Vector3 end, NavigationCheckFor[] checkFor, int[] penalty){
            FindPath(nodes, _gridCollection.ReturnGridPosition(start, true), _gridCollection.ReturnGridPosition(end, true), checkFor, penalty);
        }
        /// <summary>
        /// Build a list of Grid Nodes that represent a path to the endNode.
        /// </summary>
        /// <param name="nodes">Nodes calculate path from.</param>
        /// <param name="startNode">Start of path.</param>
        /// <param name="endNode">End of path.</param>
        /// <param name="checkFor">Have function check for grid node with value. Corresponds with penalty</param>
        /// <param name="penalty">Move cost penalty to apply if CheckFor finds node with value. Value of 0 means it will avoid.</param>
        public void FindPath(List<GridNode> nodes, GridNode startNode, GridNode endNode, NavigationCheckFor[] checkFor, int[] penalty){
            if(startNode == null || endNode == null)
                return;
            
            foreach(GridNode node in nodes){
                node.ResetNavValues();
            }

            startNode.hCost = CalculateDistance(startNode.GridPosition, endNode.GridPosition);
            startNode.CalculateFCost();

            HashSet<GridNode> openNodes = new();
            HashSet<GridNode> closedNodes = new();
            openNodes.Add(startNode);
            GridNode bestOption = startNode;
            int hCostGoingUp = 0;
            int lowest = int.MaxValue;

            while(openNodes.Count > 0){
                GridNode currentNode = GetLowestFCostNode(openNodes);

                //Check if the current hCost is above the lowest hCost
                //This is to prevent the path from going in the wrong direction
                if(currentNode.hCost <= lowest){
                    hCostGoingUp = 0;
                    lowest = currentNode.hCost;
                }else{
                    hCostGoingUp++;
                    if(hCostGoingUp > _breakAtHCostGoingUpAmount)
                        break;
                }
                

                if(currentNode == endNode){
                    Path = CalculatePath(currentNode);
                    return;
                }
                openNodes.Remove(currentNode);
                closedNodes.Add(currentNode);
                //Debug
                //currentNode.SetOwnColor(Color.blue);
                GetSurroundingTiles(currentNode.GridPosition, nodes, checkFor, penalty);
                foreach(GridNode node in _surroundingTiles){
                    
                    if(closedNodes.Contains(node))continue;
                    int p = 0;
                    for(int i = 0;i<checkFor.Length;i++){
                        if(penalty[i] == 0) continue;
                        if(checkFor[i] == NavigationCheckFor.Negative && node.Value < 0)p = penalty[i];
                        if(checkFor[i] == NavigationCheckFor.Neutral && node.Value == 0)p = penalty[i];
                        if(checkFor[i] == NavigationCheckFor.Positive && node.Value > 0)p = penalty[i];
                        
                    }

                    int tentativeGCost = currentNode.gCost + CalculateDistance(node.GridPosition, currentNode.GridPosition) + p;
                    if(tentativeGCost < node.gCost){
                        node.CameFromNode = currentNode;
                        node.gCost = tentativeGCost;
                        node.hCost = CalculateDistance(node.GridPosition, endNode.GridPosition) + p;

                        //Debug
                        //node.SetOwnColor(Color.green);

                        if(node.hCost < bestOption.hCost)
                            bestOption = node;
                        
                        node.CalculateFCost();
                        if(!openNodes.Contains(node))
                            openNodes.Add(node);
                    }
                }
            }
            
            Path = CalculatePath(bestOption);
        }
        private Vector2Int[] Directions = new Vector2Int[]{
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(1, 1),
            new Vector2Int(-1, -1),
            new Vector2Int(-1, 1),
            new Vector2Int(1, -1)
        };
        private HashSet<GridNode> _surroundingTiles = new();
        private void GetSurroundingTiles(Vector2Int position, List<GridNode> nodes, NavigationCheckFor[] checkFor, int[] penalty){
            _surroundingTiles.Clear();
            foreach(Vector2Int dir in Directions){
                GridNode node = _gridCollection.ReturnDirection(position.x, position.y, dir.x, dir.y);
                if(node != null && nodes.Contains(node)){
                    bool continueCheck = false;
                    for(int i = 0;i<checkFor.Length;i++){
                        if(penalty[i] > 0)continue;
                        if(checkFor[i] == NavigationCheckFor.Negative && node.Value < 0)continueCheck = true;
                        if(checkFor[i] == NavigationCheckFor.Neutral && node.Value == 0)continueCheck = true;
                        if(checkFor[i] == NavigationCheckFor.Positive && node.Value > 0)continueCheck = true;
                    }
                    if(continueCheck)continue;
                        _surroundingTiles.Add(node);
                }
            }
        }
        private List<GridNode> CalculatePath(GridNode endNode){
            List<GridNode> path = new(){endNode};
            GridNode currentNode = endNode;
            while(currentNode.CameFromNode != null){
                //Debug.Log(currentNode.Position  + " :Path");
                path.Add(currentNode.CameFromNode);
                currentNode = currentNode.CameFromNode;
            }
            path.Reverse();
            return path;
        }
        private GridNode GetLowestFCostNode(HashSet<GridNode> set){
            GridNode lowest = null;
            foreach(GridNode node in set){
                if(lowest == null){
                    lowest = node;
                    continue;
                }
                if(node.fCost < lowest.fCost)
                    lowest = node;
            }
            return lowest;
        }
        private GridNode GetLowestHCostNode(HashSet<GridNode> set){
            GridNode lowest = null;
            foreach(GridNode node in set){
                if(lowest == null){
                    lowest = node;
                    continue;
                }
                if(node.hCost < lowest.hCost)
                    lowest = node;
            }
            return lowest;
        }
        private int CalculateDistance(Vector2Int a, Vector2Int b){
            int xDist = Mathf.Abs(a.x - b.x);
            int yDist = Mathf.Abs(a.y - b.y);
            int remainder = Mathf.Abs(xDist - yDist);
            return MOVE_COST_DIAG * Mathf.Min(xDist, yDist) + MOVE_COST * remainder;
        }
    }
}