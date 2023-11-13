using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DecisionGrid{
    abstract public class GridCollectionBase{

        /// <summary>
        /// Z is first [z][x]
        /// </summary>
        /// <returns></returns>
        protected List<List<GridNode>> _grids = new();
        public List<GridNode> AllNodes {get; protected set;}
        protected int _width;
        public int Width => _width;
        protected int _length;
        public int Length => _length;
        protected float _xCenter;
        protected float _zCenter;
        protected float _extentWidth;
        protected float _extentLength;

        protected float _detailMultiplier = 1;
        private GridShapeBuilder _decisionGrid;

        /// <summary>
        /// Functionality for pathfinding on Collection.
        /// </summary>
        /// <value></value>
        public Navigate PathFinding{get; private set;}
        /// <summary>
        /// Abstract class all collections extend from.
        /// </summary>
        /// <param name="decisionGrid"></param>
        /// <param name="grids"></param>
        /// <param name="nodes"></param>
        /// <param name="xCenter"></param>
        /// <param name="zCenter"></param>
        /// <param name="width"></param>
        /// <param name="length"></param>
        /// <param name="detailMultiplier"></param>
        internal GridCollectionBase(GridShapeBuilder decisionGrid, List<List<GridNode>> grids, List<GridNode> nodes, float xCenter, float zCenter, int width, int length, float detailMultiplier){
            _decisionGrid = decisionGrid;
            _detailMultiplier = detailMultiplier;
            _grids = grids;
            AllNodes = nodes;
            _width = width;
            _length = length;
            _xCenter = xCenter;
            _zCenter = zCenter;
            _extentWidth = Mathf.RoundToInt((width - 1) * .5f) * _detailMultiplier;
            _extentLength = Mathf.RoundToInt((length - 1) * .5f) * _detailMultiplier;
            PathFinding = new Navigate(this);
        }
        /// <summary>
        /// Place the center of the GridCollection at this position.
        /// </summary>
        /// <param name="position"></param>
        public virtual void UpdateCenter(Vector3 position){
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
                zPos = (z * _detailMultiplier) + -_extentLength + _zCenter;
                for(int x = 0; x<_width; x++)
                {
                    xPos = (x * _detailMultiplier) + -_extentWidth + _xCenter;
                    _grids[z][x].UpdateGridPosition(new Vector2Int(x, z), new Vector3(xPos, .3f, zPos));
                }
            }
        }
        /// <summary>
        /// Set the value on all located tiles. then return tiles that were set.
        /// </summary>
        /// <param name="value">Should probably by 1 or -1</param>
        /// <param name="position">The initial position the algorythm should start at.</param>
        /// <param name="spread">The amount of squares function should work through (radius).</param>
        /// <param name="shape">Style of spread to place on Collection</param>
        /// <param name="xDir">1 will have grid work right / -1 will have grid work left / 0 works through both directions./param>
        /// <param name="yDir">1 will have grid work forward / -1 will have grid work backward / 0 works through both directions.</param>
        /// <param name="breakType">Stop spread if comes accross node value.</param>
        /// <returns></returns>
        public List<GridNode> SetValueOnGridLocations
        (int value, Vector3 position, int spread, Shape shape, int xDir, int yDir, BreakType breakType = BreakType.None){
            List<GridNode> grids =  _decisionGrid.GetGridLocations(this, position, spread, shape, xDir, yDir, breakType);
            foreach(GridNode g in grids)
                g.SetValue(value);

            return grids;
        }
        /// <summary>
        /// Return all located nodes without setting any value on them.
        /// </summary>
        /// <param name="value">Should probably by 1 or -1</param>
        /// <param name="position">The initial position the algorythm should start at.</param>
        /// <param name="spread">The amount of squares function should work through (radius).</param>
        /// <param name="shape">Style of spread to place on Collection</param>
        /// <param name="xDir">1 will have grid work right / -1 will have grid work left / 0 works through both directions./param>
        /// <param name="yDir">1 will have grid work forward / -1 will have grid work backward / 0 works through both directions.</param>
        /// <param name="breakType">Stop spread if comes accross node value.</param>
        /// <returns></returns>
        public List<GridNode> ReturnGridPositions
        (Vector3 position, int spread, Shape shape, int xDir, int yDir, BreakType breakType = BreakType.None){
            return _decisionGrid.GetGridLocations(this, position, spread, shape, xDir, yDir, breakType);
        }
        /// <summary>
        /// Return a single node at position at position.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="returnClosestIfNull">Should I return closest node if none are found at position.</param>
        /// <returns></returns>
        public GridNode ReturnGridPosition(Vector3 position, bool returnClosestIfNull = false)
        {
            float z = Mathf.RoundToInt(position.z / _detailMultiplier) * _detailMultiplier;
            z -= _zCenter;
            z += _extentLength;
            float x = Mathf.RoundToInt(position.x / _detailMultiplier) * _detailMultiplier;
            x -= _xCenter;
            x += _extentWidth;

            int zIndex = Mathf.RoundToInt(z / _detailMultiplier);
            int xIndex = Mathf.RoundToInt(x / _detailMultiplier);
            if (zIndex < 0 || zIndex >= _length || xIndex < 0 || xIndex >= _width)
            {
                if(returnClosestIfNull){
                    if(zIndex < 0)
                        zIndex = 0;
                    else if(zIndex >= _length)
                        zIndex = _length -1;
                    if(xIndex < 0)
                        xIndex = 0;
                    else if(xIndex >= _width)
                        xIndex = _width -1;

                    return _grids[zIndex][xIndex];
                }
                return null;
            }

            return _grids[zIndex][xIndex];
        }
        /// <summary>
        /// Find node using multi-dimensional array.
        /// </summary>
        /// <param name="x">XPosition</param>
        /// <param name="y">YPosition</param>
        /// <param name="xDir">how many nodes to move on X.</param>
        /// <param name="yDir">How many nodes to move on Y/Z.</param>
        /// <returns>will return null if none exhist at location.</returns>
        public GridNode ReturnDirection(int x, int y, int xDir, int yDir){
            int newZ = y + yDir;
            int newX = x + xDir;
            if (newZ >= _length || newZ < 0 || newX >= _width || newX < 0)
                return null;

            return _grids[newZ][newX];
        }
        public void ResetNodeValues(){
            for (int z = 0; z < _grids.Count; z++)
            {
                for (int x = 0; x < _grids[z].Count; x++)
                {
                    _grids[z][x].SetValueToNuetral();
                }
            }
        }
        public void CleanUp(){
            for (int z = 0; z < _grids.Count; z++)
            {
                for (int x = 0; x < _grids[z].Count; x++)
                {
                    _grids[z][x].CleanUp();
                }
            }
            _grids = null;            
        }
        public List<GridNode> ReturnDirectionalGridLocations(Vector3 position, Vector3 addToPosition, int bSpread, int forward, Shape shape, BreakType breakType = BreakType.None){
            Vector3 loc = position + addToPosition;
            int xForward = 0;
            if(loc.x * forward < (position.x - .5f * forward) * forward){
                //Left Side
                if(loc.z * forward - .5f * forward > position.z * forward)//UpLeft
                {
                    xForward = -forward;
                }
                else if(loc.z * forward + .5f * forward < position.z * forward)//DownLeft
                {
                    xForward = -forward;
                    forward = -forward;
                }
                else//Left
                {
                    xForward = -forward;
                    forward = 0;
                }
            }else if(loc.x * forward> (position.x + .5f * forward) * forward){
                //Right Side
                if(loc.z * forward - .5f * forward > position.z * forward )//UpRight
                {
                    xForward = forward;
                }
                else if(loc.z * forward + .5f * forward < position.z * forward )//DownRight
                {
                    xForward = forward;
                    forward = -forward;
                }
                else//Right
                {
                    xForward = forward;
                    forward = 0;
                }
            }else{
                //Up or down
                if(loc.z * forward > position.z * forward )//up
                {

                }
                else//down
                {
                    xForward = 0;
                    forward = -forward;
                }
            }
            return ReturnGridPositions(position, bSpread, shape, xForward, forward, breakType);
        }
        public List<GridNode> DirectionalSetValueOnGridLocations(int value, Vector3 position, Vector3 addToPosition, int bSpread,  DecisionGrid.Shape shape, DecisionGrid.BreakType breakType = DecisionGrid.BreakType.None){
            Vector3 loc = position + addToPosition;
            int forward = 1;//(int)ActiveMatchup.Forward;
            int xForward = 0;
            if(loc.x * forward < (position.x - .5f * forward) * forward){
                //Left Side
                if(loc.z * forward - .5f * forward > position.z * forward)//UpLeft
                {
                    xForward = -forward;
                }
                else if(loc.z * forward + .5f * forward < position.z * forward)//DownLeft
                {
                    xForward = -forward;
                    forward = -forward;
                }
                else//Left
                {
                    xForward = -forward;
                    forward = 0;
                }
            }else if(loc.x * forward> (position.x + .5f * forward) * forward){
                //Right Side
                if(loc.z * forward - .5f * forward > position.z * forward)//UpRight
                {
                    xForward = forward;
                }
                else if(loc.z * forward + .5f * forward < position.z * forward)//DownRight
                {
                    xForward = forward;
                    forward = -forward;
                }
                else//Right
                {
                    xForward = forward;
                    forward = 0;
                }
            }else{
                //Up or down
                if(loc.z * forward > position.z * forward)//up
                {

                }
                else//down
                {
                    xForward = 0;
                    forward = -forward;
                }
            }
            return SetValueOnGridLocations(-1, position, bSpread, shape, xForward, forward, breakType);
        }
    }
}