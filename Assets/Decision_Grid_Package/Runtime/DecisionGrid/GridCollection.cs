using System.Collections.Generic;
using UnityEngine;

namespace DecisionGrid{
    public class GridCollection: GridCollectionBase{

        /// <summary>
        /// Collection of GridNodes and functions to use this collection for pathing/positioning.
        /// </summary>
        /// <param name="decisionGrid"></param>
        /// <param name="grids"></param>
        /// <param name="nodes"></param>
        /// <param name="xCenter"></param>
        /// <param name="zCenter"></param>
        /// <param name="width"></param>
        /// <param name="length"></param>
        internal GridCollection(GridShapeBuilder decisionGrid, List<List<GridNode>> grids, List<GridNode> nodes, int xCenter, int zCenter, int width, int length):
        base(decisionGrid, grids, nodes, xCenter, zCenter, width, length, 1){
            
        }
        /// <returns></returns> <summary>
        /// Returns a Collection of GridLocations which are clustered around the center point. 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="width"></param>
        /// <param name="length"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        static public GridCollection CreateNewCollection(Vector3 center, int width, int length, bool debug){
            int xCenter = Mathf.RoundToInt(center.x);
            int zCenter = Mathf.RoundToInt(center.z);
            if(width % 2 == 0)
                width += 1;
            int extentWidth = Mathf.RoundToInt((width - 1) * .5f);
            if(length % 2 == 0)
                length += 1;
            int extentLength = Mathf.RoundToInt((length - 1) * .5f);
            List<List<GridNode>> gridNodesMultiList = new();
            List<GridNode> nodes = new();
            int xPos;
            int zPos;
            for(int z = 0; z<length; z++)
            {
                List<GridNode> newRow = new();
                zPos = z + -extentLength + zCenter;
                for(int x = 0; x<width; x++)
                {
                    xPos = x + -extentWidth + xCenter;
                    GridNode location = new(new Vector2Int(x, z), new Vector3(xPos, .3f, zPos), debug);
                    nodes.Add(location);
                    newRow.Add(location);
                }

                gridNodesMultiList.Add(newRow);
            }
            return new GridCollection(GridShapeBuilder.Instance, gridNodesMultiList, nodes, xCenter, zCenter, width, length);
        }
        /// <summary>
        /// Returns a Collection of GridLocations which are clustered around the center point.
        /// </summary>
        /// <param name="center">Value to cluster around</param>
        /// <param name="size">Size of grid, will force to odd number.</param>
        /// <param name="singleDetail">Should the grid cluster contain space between gridLocations</param>
        /// <returns></returns>
        static public GridCollection CreateNewCollection(Vector3 center, int size, bool debug = false){
            return CreateNewCollection(center, size, size, debug);
        }
    }
}