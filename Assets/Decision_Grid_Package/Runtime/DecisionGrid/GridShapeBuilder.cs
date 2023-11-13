using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DecisionGrid
{
    internal class GridShapeBuilder : MonoBehaviour
    {
        static private GridShapeBuilder _createdInstance;
        static public GridShapeBuilder Instance{
            get{
                if(_createdInstance == null)
                    new GameObject("GridShapeBuilder").AddComponent<GridShapeBuilder>();

                return _createdInstance;
            }
        }
        static public readonly bool DEBUG = false;
        static public readonly bool FIELD_DEBUG = false;
        
        
        private void Awake()
        {
            _createdInstance = this;
            DontDestroyOnLoad(this);
            transform.position = Vector3.zero;
        }

        private List<GridNode> _usedTiles = new();
        private List<int> _verticalTileCount = new();
        private List<GridNode> _verticalTiles = new();
        private List<int> _horizontalTileCount = new();
        private List<GridNode> _horizontalTiles = new();
        private List<GridNode> _singleDirectionTiles = new();
        private List<float> _singleDirectionCount = new();
        private BreakType _breakType;
        private GridCollectionBase _activeCollection;
        public List<GridNode> GetGridLocations(GridCollectionBase collection, Vector3 position, int spread, Shape shape, int xDir, int yDir, BreakType breakType)
        {
            _activeCollection = collection;
            _breakType = breakType;
            List<GridNode> roundLocations = new();

            GridNode location = _activeCollection.ReturnGridPosition(position);
            if (location == null)
                return roundLocations;

            _verticalTiles.Clear();
            _verticalTileCount.Clear();
            _horizontalTiles.Clear();
            _horizontalTileCount.Clear();
            _usedTiles.Clear();
            _singleDirectionTiles.Clear();
            _singleDirectionCount.Clear();
            
            if(shape == Shape.Circle){
                SetVerticalTiles(location, spread, 0, yDir);
                SetHorizontalTiles(location, spread, 0, xDir);
                float max = spread + .25f;
                float cost = 1.75f;
                for(int x = 0;x < _verticalTiles.Count; x++){
                    if(_verticalTiles[x] == location){
                        if(yDir >= 0){
                            if(xDir >= 0)
                                SetSingleDirectionTiles(_verticalTiles[x], max, _verticalTileCount[x], cost, 1, 1); 
                            if(xDir <= 0)
                                SetSingleDirectionTiles(_verticalTiles[x], max, _verticalTileCount[x], cost, -1, 1); 
                        }
                        if(yDir <= 0){
                            if(xDir >= 0)
                                SetSingleDirectionTiles(_verticalTiles[x], max, _verticalTileCount[x], cost, 1, -1); 
                            if(xDir <= 0)
                                SetSingleDirectionTiles(_verticalTiles[x], max, _verticalTileCount[x], cost, -1, -1); 
                        }
                    }
                    else if(_verticalTiles[x].Position.z > location.Position.z && yDir >= 0){
                        if(xDir >= 0) 
                            SetSingleDirectionTiles(_verticalTiles[x], max, _verticalTileCount[x], cost, 1, 1);
                        if(xDir <= 0)
                            SetSingleDirectionTiles(_verticalTiles[x], max, _verticalTileCount[x], cost, -1, 1);
                    }else if(yDir <= 0){
                        if(xDir >= 0)
                            SetSingleDirectionTiles(_verticalTiles[x], max, _verticalTileCount[x], cost, 1, -1); 
                        if(xDir <= 0)
                            SetSingleDirectionTiles(_verticalTiles[x], max, _verticalTileCount[x], cost, -1, -1); 
                    }
                }
                for(int x = 0;x < _horizontalTiles.Count; x++){
                    if(_horizontalTiles[x] == location){
                        continue;
                    }
                    else if(_horizontalTiles[x].Position.x > location.Position.x && xDir >= 0){
                        if(yDir >= 0)
                            SetSingleDirectionTiles(_horizontalTiles[x], max, _horizontalTileCount[x], cost, 1, 1); 
                        if(yDir <= 0)
                            SetSingleDirectionTiles(_horizontalTiles[x], max, _horizontalTileCount[x], cost, 1, -1); 
                    }else if(xDir<=0){
                        if(yDir >= 0)
                            SetSingleDirectionTiles(_horizontalTiles[x], max, _horizontalTileCount[x], cost, -1, 1); 
                        if(yDir <= 0)
                            SetSingleDirectionTiles(_horizontalTiles[x], max, _horizontalTileCount[x], cost, -1, -1); 
                    }
                }
            }
            else if(shape == Shape.Square)
            {
                SetVerticalTiles(location, spread, 0, yDir);
                for(int vert = 0;vert < _verticalTiles.Count; vert++){
                    SetHorizontalTiles(_verticalTiles[vert], spread, 0, xDir);
                }
            }else if(shape == Shape.Diamond)
            {
                SetVerticalTiles(location, spread, 0, yDir);
                for(int vert = 0;vert < _verticalTiles.Count; vert++){
                    SetHorizontalTiles(_verticalTiles[vert], spread, _verticalTileCount[vert], xDir);
                }
            }else if(shape == Shape.Line)
            {
                int initialExpansion = Mathf.RoundToInt(spread * 1);
                if(initialExpansion > 2)
                    initialExpansion = 2;
                ShapeAlgorythm(location, spread, xDir, yDir, initialExpansion, 0);
            }else if(shape == Shape.Cone)
            {
                int initialExpansion = Mathf.RoundToInt(spread * .11f);
                if(initialExpansion > 2)
                    initialExpansion = 2;
                ShapeAlgorythm(location, spread, xDir, yDir, initialExpansion, -.55f);
            }
            roundLocations.AddRange(_usedTiles.Where(x => !roundLocations.Contains(x)).ToList());
            
            return roundLocations;
        }
        private void ShapeAlgorythm(GridNode location, int spread, int xDir, int yDir, float initialExpansion, float expansionMultiplier, float moveCost = 1){
            if((yDir==1 && xDir==0) ||(yDir==-1 && xDir==0))//Up or Down
            {
                SetVerticalTiles(location, spread, 0, yDir);
                for(int x = 0;x<_verticalTiles.Count; x++){
                    SetHorizontalTiles(_verticalTiles[x], Mathf.RoundToInt((initialExpansion-(x*expansionMultiplier))), 0, 0);
                }
            }else if(yDir!=0 && xDir!=0)//Diagnals
            {  
                SetSingleDirectionTiles(location, spread, 0, moveCost, xDir, yDir);
                for(int x = 0;x<_singleDirectionTiles.Count; x++){
                    SetVerticalTiles(_singleDirectionTiles[x], Mathf.RoundToInt((initialExpansion - (x*expansionMultiplier))), 0, -yDir);
                    SetHorizontalTiles(_singleDirectionTiles[x], Mathf.RoundToInt((initialExpansion - (x*expansionMultiplier))), 0, -xDir);
                }
            }else if((yDir==0 && xDir==1) || (yDir==0 && xDir==-1))//Right or Left
            {
                SetHorizontalTiles(location, spread, 0, xDir);
                for(int x = 0;x<_horizontalTiles.Count; x++){
                    SetVerticalTiles(_horizontalTiles[x], Mathf.RoundToInt((initialExpansion - (x*expansionMultiplier))), 0, 0);
                }
            }
        }
        private bool ShouldBreakBeforeAdd(GridNode location){
            if(_breakType == BreakType.Neutral && location.Value == 0)
                return true;
            if(_breakType == BreakType.Positive && location.Value > 0)
                return true;
            if(_breakType == BreakType.Negative && location.Value < 0)
                return true;
            return false;
        }
        private bool ShouldBreakAfterAdd(GridNode location){
            if(_breakType == BreakType.NeutralSetValue && location.Value == 0)
                return true;
            if(_breakType == BreakType.PositiveSetValue && location.Value > 0)
                return true;
            if(_breakType == BreakType.NegativeSetValue && location.Value < 0)
                return true;
            return false;
        }
        public void SetAllAround(GridNode location, int max, int currentCount, int xDir = 0, int zDir = 0)
        {
            if (location == null || _usedTiles.Contains(location) || (ShouldBreakBeforeAdd(location) && currentCount > 0 ))
                return;

            _usedTiles.Add(location);
            if(currentCount > 0 && ShouldBreakAfterAdd(location))
                return;
            currentCount++;
            
            if (currentCount >= max)
                return;
            int x = (int)location.GridPosition.x;
            int z = (int)location.GridPosition.y;
            
            SetAllAround(_activeCollection.ReturnDirection(x, z, 0, 1), max, currentCount, xDir, zDir);
            SetAllAround(_activeCollection.ReturnDirection(x, z, 0, -1), max, currentCount,xDir, zDir);
            SetAllAround(_activeCollection.ReturnDirection(x, z, 1, 0), max, currentCount, xDir, zDir);
            SetAllAround(_activeCollection.ReturnDirection(x, z, -1, 0), max, currentCount, xDir, zDir);
        }
        public void SetVerticalTiles(GridNode location, int max, int currentCount, int zDir = 0){
            if(location == null|| _verticalTiles.Contains(location)|| (ShouldBreakBeforeAdd(location) && currentCount > 0 ))
                return;
            if(!_usedTiles.Contains(location))
                _usedTiles.Add(location);
            _verticalTiles.Add(location);
            _verticalTileCount.Add(currentCount);
            if(currentCount > 0 && ShouldBreakAfterAdd(location))
                return;
            currentCount++;
            if (currentCount >= max)
                return;
            int x = (int)location.GridPosition.x;
            int z = (int)location.GridPosition.y;

            if(zDir >= 0)
                SetVerticalTiles(_activeCollection.ReturnDirection(x, z, 0, 1), max, currentCount,  zDir);
            if(zDir <= 0)
                SetVerticalTiles(_activeCollection.ReturnDirection(x, z, 0, -1), max, currentCount, zDir);
        }
        public void SetHorizontalTiles(GridNode location, int max, int currentCount, int xDir = 0){
            if(location == null|| _horizontalTiles.Contains(location)|| (ShouldBreakBeforeAdd(location) && currentCount > 0 ))
                return;
            if(!_usedTiles.Contains(location))
                _usedTiles.Add(location);
            _horizontalTiles.Add(location);
            _horizontalTileCount.Add(currentCount);

            if(currentCount > 0 && ShouldBreakAfterAdd(location))
                return;
            currentCount++;
            if (currentCount >= max)
                return;
            int x = (int)location.GridPosition.x;
            int z = (int)location.GridPosition.y;

            if(xDir >= 0)
                SetHorizontalTiles(_activeCollection.ReturnDirection(x, z, 1, 0), max, currentCount, xDir);
            if(xDir <= 0)
                SetHorizontalTiles(_activeCollection.ReturnDirection(x, z, -1, 0), max, currentCount, xDir);
        }
        public void SetSingleDirectionTiles(GridNode location, float max, float currentCount, float moveCost, int xDir, int zDir){
            if(location == null || (ShouldBreakBeforeAdd(location) && currentCount > 0 ))
                return;
            if(!_usedTiles.Contains(location))
                _usedTiles.Add(location);

            _singleDirectionTiles.Add(location);
            _singleDirectionCount.Add(currentCount);
            if(currentCount > 0 && ShouldBreakAfterAdd(location))
                return;
            currentCount+= moveCost;
            
            if (currentCount >= max)
                return;
            int x = (int)location.GridPosition.x;
            int z = (int)location.GridPosition.y;

            SetSingleDirectionTiles(_activeCollection.ReturnDirection(x, z, xDir, zDir), max, currentCount, moveCost, xDir, zDir);
        }
    }
}
