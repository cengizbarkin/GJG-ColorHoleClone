using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject ground1 = default;
    [SerializeField] private GameObject ground2 = default;

    private int _numberOfGoodObjects = 0;
    private int _collectedGoodObjectsCount = 0;
        
    private void Awake()
    {
        gameObject.SetActive(false);
    }
    
    public void InitLevel()
    {
        GameManager.Instance.OnGameStateStarted += GameStateStarted;
        GameManager.Instance.OnGameStateCompleted += GameStateCompleted;
    }
    
    private void GameStateStarted(string state)
    {
        switch (state)
        {
            case "Ground1":
                CalculateGoodObjectsCount(ground1);
                break;
            case "Ground2":
                CalculateGoodObjectsCount(ground2);
                break;
        }
    }
    
    private void GameStateCompleted(string state)
    {
        _numberOfGoodObjects = 0;
        _collectedGoodObjectsCount = 0;
    }
    
    private void CalculateGoodObjectsCount(GameObject ground)
    {
        foreach (Transform child in ground.transform)
        {
            if (child.tag.Equals("Good"))
                _numberOfGoodObjects++;
        }
        GameManager.Instance.Progress(_numberOfGoodObjects, _collectedGoodObjectsCount);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        var otherTag = other.tag;
        if (otherTag.Equals("Good"))
        {
            _collectedGoodObjectsCount++;
            GameManager.Instance.Progress(_numberOfGoodObjects, _collectedGoodObjectsCount);
            if (_collectedGoodObjectsCount != 0 && _collectedGoodObjectsCount == _numberOfGoodObjects)
            {
                GameManager.Instance.GameStateCompleted(other.transform.parent.name);
            }
        }
        if (otherTag.Equals("Bad"))
        {
            GameManager.Instance.GameOver();
        }
        Destroy(other.gameObject);
    }
    
    private void OnDisable()
    {
        if (GameManager.Instance)
        {
            GameManager.Instance.OnGameStateStarted -= GameStateStarted;
            GameManager.Instance.OnGameStateCompleted -= GameStateCompleted; 
        }
    }
    
}
