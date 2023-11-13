using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DecisionGrid;

public class Enemy : MonoBehaviour
{
    private Vector3 _startPosition;
    private Vector3 _moveHere;
    [SerializeField]
    private bool _move = false;
    [SerializeField]
    private int _size = 4;
    [SerializeField]
    private Shape _shape = Shape.Circle;
    void Start()
    {
        _startPosition = transform.position;
    }

    private float _timer = 0;
    void Update()
    {
        if(!_move)
            return;
        _timer -= Time.deltaTime;
        if(_timer < 0)
        {
            _timer = Random.value * 4;
            _moveHere = _startPosition + new Vector3(Random.value * 10 - 5, _startPosition.y, Random.value * 10 - 5);
        }
        transform.position = Vector3.MoveTowards(transform.position, _moveHere, Time.deltaTime * 3);
    }

    public void DrawShapeOnCollection(GridCollectionBase grid){
        grid.SetValueOnGridLocations(-1, transform.position, _size, _shape, 0, 0, BreakType.Positive);
    }
}
