using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{

    private Vector3 _initialPosition;
    private Vector3 _targetPosition;
    
    private bool _animateDoor;

    [SerializeField] private float animationSpeed = default;
    
    //Game state completed'ı dinle
    
    private void Start()
    {
        
        GameManager.Instance.OnGameStateCompleted += GameStateCompleted;
        
        _initialPosition = transform.position;
        _targetPosition.x = _initialPosition.x;
        _targetPosition.y = _initialPosition.y - 1.0f;
        _targetPosition.z = _initialPosition.z;
    }

    
    
    private void GameStateCompleted(string state)
    {
        switch (state)
        {
            case "Ground1":
                _animateDoor = true;
                break;
            case "Bridge":
                break;
            case "Ground2":
                transform.position = _initialPosition;
                break;
        }
    }

    private void Update()
    {
        if (_animateDoor)
        {
            float step = animationSpeed * Time.deltaTime;
            var currentPos = transform.position;
            currentPos = Vector3.MoveTowards(currentPos, _targetPosition, step);
            if (currentPos.y <= _targetPosition.y)
            {
                _animateDoor = false;
                GameManager.Instance.GameStateStarted("Bridge");
            }
            transform.position = currentPos;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance)
        {
            GameManager.Instance.OnGameStateCompleted -= GameStateCompleted;
        }
    }
}
