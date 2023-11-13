using System.Collections.Generic;
using UnityEngine;

namespace DecisionGrid{
    public class GridCollectionDetail:GridCollectionBase{

        /// <summary>
        /// CollectionGrid With 2X more boxes per unit.
        /// Collection of GridNodes and functions to use this collection for pathing/positioning.
        /// </summary>
        /// <param name="decisionGrid"></param>
        /// <param name="grids"></param>
        /// <param name="nodes"></param>
        /// <param name="xCenter"></param>
        /// <param name="zCenter"></param>
        /// <param name="width"></param>
        /// <param name="length"></param>
        internal GridCollectionDetail(GridShapeBuilder decisionGrid, List<List<GridNode>> grids, List<GridNode> nodes, float xCenter, float zCenter, int width, int length):
        base(decisionGrid, grids, nodes, xCenter, zCenter, width, length, .5f){
            
        }
        /// <summary>
        /// Place the center of the GridCollection at this position.
        /// </summary>
        /// <param name="position"></param>
        public override void UpdateCenter(Vector3 position){
            float xCenter = Mathf.RoundToInt(position.x / _detailMultiplier) * _detailMultiplier;
            float zCenter = Mathf.RoundToInt(position.z / _detailMultiplier) * _detailMultiplier;
            if(xCenter == _xCenter && zCenter == _zCenter)
                return;
            
            _xCenter = xCenter;
            _zCenter = zCenter;
            float xPos;
            float zPos;
            for(int z = 0; z<_length; z++)
            {
                zPos = (z * _detailMultiplier) + (-_extentLength) + _zCenter;
                for(int x = 0; x<_width; x++)
                {
                    xPos = (x * _detailMultiplier) + (-_extentWidth) + _xCenter;
                    _grids[z][x].UpdateGridPosition(new Vector2Int(x, z), new Vector3(xPos, .3f, zPos));
                }
            }
        }
        /// <returns></returns> <summary>
        /// Returns a Collection of GridLocations which are clustered around the center point.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="width"></param>
        /// <param name="length"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        static public GridCollectionDetail CreateNewCollection(Vector3 center, int width, int length, bool debug = false){
            float xCenter = Mathf.RoundToInt(center.x * 2) * .5f;
            float zCenter = Mathf.RoundToInt(center.z * 2) * .5f;

            if(width % 2 == 0)
                width += 1;
            float extentWidth = Mathf.RoundToInt((width - 1) * .5f) * .5f;
            if(length % 2 == 0)
                length += 1;
            float extentLength = Mathf.RoundToInt((length - 1) * .5f) * .5f;
            List<List<GridNode>> gridNodesMultiList = new();
            List<GridNode> nodes = new();
            float xPos;
            float zPos;

            for(int z = 0; z<length; z++)
            {
                List<GridNode> newRow = new();
                zPos = (z * .5f) + (-extentLength) + zCenter;
                for(int x = 0; x < width; x++)
                {
                    xPos = (x * .5f) + (-extentWidth) + xCenter;
                    GridNode location = new(new Vector2Int(x, z), new Vector3(xPos, .3f, zPos), debug, .5f);
                    nodes.Add(location);
                    newRow.Add(location);
                }
                gridNodesMultiList.Add(newRow);
            }
            return new GridCollectionDetail(GridShapeBuilder.Instance, gridNodesMultiList, nodes, xCenter, zCenter, width, length);
        }
        /// <summary>
        /// Returns a Collection of GridLocations which are clustered around the center point.
        /// </summary>
        /// <param name="center"></param>
        /// <param name="size"></param>
        /// <param name="debug"></param>
        /// <returns></returns>
        static public GridCollectionDetail CreateNewCollection(Vector3 center, int size, bool debug = false){
            return CreateNewCollection(center, size, size, debug);
        }
    }
}