using UnityEngine;

namespace DecisionGrid
{
    public class GridNode
    {
        private Vector2Int _gridPosition;
        public Vector2Int GridPosition { get { return _gridPosition; } }
        private Vector3 _position;
        public Vector3 Position { get { return _position; } }
        public int Value {get; private set;} = 0;

        public GridNode CameFromNode = null;
        public int gCost = int.MaxValue;
        /// <summary>
        /// DistanceRemaining
        /// </summary>
        public int hCost = 0; 
        public int fCost = int.MaxValue;
        public void CalculateFCost(){
            fCost = gCost + hCost;
        }
        public void ResetNavValues(){
            gCost = int.MaxValue;
            hCost = 0; 
            fCost = int.MaxValue;
            CameFromNode = null;
        }

        private SpriteRenderer _image = null;
        private bool _createNodeImage = false;
        public GridNode(Vector2Int gridPosition, Vector3 position, bool createNodeImage, float nodeImageScale = 1)
        {
            _gridPosition = gridPosition;
            _position = position;
            _createNodeImage = createNodeImage;
            if (_createNodeImage)
            {
                GameObject debugParent = GameObject.Find("DecisionDebug") ?? new GameObject("DecisionDebug");
                _image = Object.Instantiate(Resources.Load("GridLocationDebugPrefab") as GameObject).GetComponent<SpriteRenderer>();
                _image.name = string.Format("Grid: {0}, World: {1}", gridPosition, position);
                _image.transform.SetParent(debugParent.transform, false);
                _image.transform.position = position + Vector3.up * .1f;
                _image.transform.localScale *= nodeImageScale;
            }
        }
        public bool ShouldBreak(BreakType breakType){
            if(breakType == BreakType.Neutral && Value == 0)
                return true;
            if(breakType == BreakType.Positive && Value > 0)
                return true;
            if(breakType == BreakType.Negative && Value < 0)
                return true;
            return false;
        }
        public void UpdateGridPosition(Vector2Int gridPosition, Vector3 position){
            _gridPosition = gridPosition;
            _position = position;
            if (_createNodeImage)
                _image.transform.position = position;
        }
        private Color SetColor
        {
            set {if(_createNodeImage) _image.color = value; }
        }
        private Color postiveColor = Color.green;
        private Color neutralColor = Color.grey;
        private Color negativeColor = Color.red;
        /// <summary>
        /// Set Value of node, -1, 0, 1.
        /// Then set color if visual node exists.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="keepOverrides"></param>
        public void SetValue(int value)
        {
            Value = value;
            if (Value > 1)
                Value = 1;
            if (Value < -1)
                Value = -1;
            CheckSetColor();
        }
        public void CheckSetColor()
        {
            if (Value >= 0)
                SetColor = Color.Lerp(neutralColor, postiveColor, Value);
            else
                SetColor = Color.Lerp(neutralColor, negativeColor, Value * -1);
        }
        /// <summary>
        /// Set color of node, if node visually exists.
        /// </summary>
        /// <param name="color"></param>
        public void SetOwnColor(Color color){
            SetColor = color;
        }
        /// <summary>
        /// Set node value to 0 and set color if node visually exists.
        /// </summary>
        public void SetValueToNuetral()
        {
            Value = 0;
            CheckSetColor();
        }

        public void OverrideColor(Color color)
        {
            SetColor = color;
        }
        /// <summary>
        /// Destroy debug image if exists.
        /// </summary>
        public void CleanUp(){
            if(_image != null)
                Object.Destroy(_image.gameObject);
        }
    }
}
