using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DecisionGrid;

public class Player : MonoBehaviour
{
    private GridCollectionBase _collection;
    private Vector3 _startPosition;
    private Vector3 _moveHere;

    private Enemy[] _enemies;
    [SerializeField]
    private Transform _goHere = null;
    [SerializeField]
    private Shape _shape = Shape.Circle;
    [SerializeField]
    private int _size = 4;

    [SerializeField]
    private NavigationCheckFor[] _checkFor = new NavigationCheckFor[]{ NavigationCheckFor.Negative };
    [SerializeField]
    private int[] _penalty = new int[]{ 10 };
    void Start()
    {
        _startPosition = transform.position;
        _collection = GridCollectionDetail.CreateNewCollection(transform.position, 50, true);
        _enemies = FindObjectsOfType<Enemy>();
    }

    private float _timer = 0;
    void Update()
    {
        _timer -= Time.deltaTime;
        if(_timer < 0)
        {
            //_timer = .5f;
            _collection.ResetNodeValues();
            _collection.UpdateCenter(transform.position);
            _collection.SetValueOnGridLocations(1, transform.position, _size, _shape, 0, 0);
            foreach(Enemy enemy in _enemies){
                enemy.DrawShapeOnCollection(_collection);
            }
            _collection.PathFinding.FindPath(_collection.AllNodes, transform.position, _goHere.position, _checkFor, _penalty);
            if(_collection.PathFinding.Path != null){
                foreach(GridNode node in _collection.PathFinding.Path){
                    node.SetOwnColor(Color.blue);
                }
            }
            // _moveHere = _startPosition + new Vector3(Random.value * 10 - 5, _startPosition.y, Random.value * 10 - 5);
        }
        // transform.position = Vector3.MoveTowards(transform.position, _moveHere, Time.deltaTime * 3);
    }
}
