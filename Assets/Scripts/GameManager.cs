using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [SerializeField] private GameObject resultPanel = default;
    [SerializeField] private Text resultText = default;
    
    [SerializeField] private List<GameObject> levels = new List<GameObject>();
    
    public static GameManager Instance { get; private set; }
    
    public event Action<string> OnGameStateStarted;
    public event Action<string> OnGameStateCompleted;
    public event Action<int> OnLevelChanged;
    public event Action<int, int> OnProgressChanged;
    public event Action OnGameOver;

    private LevelManager _currentLevel;
    private int _currentLevelNumber;
    
    public string currentState;
    
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
    }
    
    // Start is called before the first frame update
    private void Start()
    {
        Time.timeScale = 1;
        _currentLevelNumber = 0;
        currentState = "Ground1";
        InitLevel();
    }
    public void GameStateStarted(string state)
    {
        OnGameStateStarted?.Invoke(state);
    }

    public void GameStateCompleted(string state)
    {
        OnGameStateCompleted?.Invoke(state);
        
        switch (state)
        {
            case "Ground1":
                break;
            case "Bridge":
                break;
            case "Ground2":
                NextLevel();
                break;
        }
    }

    public void Progress(int total, int collected)
    {
        OnProgressChanged?.Invoke(total, collected);
    }
    
    private void NextLevel()
    {
        Destroy(levels[_currentLevelNumber]);
        _currentLevelNumber++;
        
        if (_currentLevelNumber >= levels.Count)
        {
            resultText.text = "WIN";
            resultPanel.SetActive(true);
        }
        else
        {
            InitLevel();
        }
    }
    private void InitLevel()
    {
        levels[_currentLevelNumber].SetActive(true);
        _currentLevel = levels[_currentLevelNumber].GetComponent<LevelManager>();
        _currentLevel.InitLevel();
        GameStateStarted(currentState);
        OnLevelChanged?.Invoke(_currentLevelNumber);
    }
    
    public void GameOver()
    { 
        OnGameOver?.Invoke();
        resultText.text = "LOST";
        resultPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void ReplayBtnPressed()
    {
        SceneManager.LoadScene(0);
    }
}
