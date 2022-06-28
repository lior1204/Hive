using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;//singelton
    //parameters
    [SerializeField] [Min(1)] private float gameTime = 180;//in seconds
    //state 
    public bool IsPaused { get { return state == GameState.Paused; }set { state = value ? GameState.Paused : GameState.Playing; } }
    private GameState state = GameState.Playing;
    public float endGameTimer { get; private set; }
    private bool IsTimeOver { get { return endGameTimer <=0; } }
    public int playerPlanetsCount { get; private set; }
    public int enemyPlanetsCount { get; private set; }
    public float screenRatio { get; private set; }
    public string levelName { get; private set; }
    TutorialManager _tutorialManager;
    private void Awake()
    {
        SetSingelton();
        screenRatio = Screen.width / Screen.height;
        endGameTimer = gameTime;
        _tutorialManager = FindObjectOfType<TutorialManager>();
        OnLevelWasLoaded(SceneManager.GetActiveScene().buildIndex);
    }
    private void OnLevelWasLoaded(int level)
    {
        screenRatio = Screen.width / Screen.height;
        if (SceneManager.GetActiveScene().name == ParamManager.Instance.MAINMENUSCENENAME)
        {
            state = GameState.MainMenu;
        }
        if (SceneManager.GetActiveScene().name == ParamManager.Instance.GAMEOVERSCENENAME)
        {
            state = GameState.EndScreen;
        }
        if (SceneManager.GetActiveScene().name.Contains("Level"))
        {
            state = GameState.Playing;
            levelName = SceneManager.GetActiveScene().name;
        }
    }
    private void SetSingelton()
    {
        if (Instance != null && Instance != this)// implement singelton
        {
            if (Application.isPlaying)
            {
                Instance.gameTime = this.gameTime;
                Instance.endGameTimer = this.gameTime;
                Destroy(this.gameObject);
            }
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    
    void Update()
    {
        if (state == GameState.Playing)
        {
            UpdateGameClock();
            UpdateHiveCount();
            CheckIfGameEnd();
        }
    }
    private void UpdateGameClock()//up the clock if game is running
    {
        if (state == GameState.Playing&&!_tutorialManager)
        {
            endGameTimer -= Time.deltaTime;
        }
    }
    private void UpdateHiveCount()
    {
        playerPlanetsCount = HiveController.Player.PlanetCount;
        enemyPlanetsCount = HiveController.Enemy.PlanetCount;
    }
    private void CheckIfGameEnd()//checks every update if game is over
    {
        if (state == GameState.Playing)//check if game is running
        {
            
            if (playerPlanetsCount <= 0 || enemyPlanetsCount <= 0 || IsTimeOver)//game ends if player or enemy has no planets or if time over
            {
                state = GameState.EndScreen;
                UpdateHiveCount();
                SceneManager.LoadScene(ParamManager.Instance.GAMEOVERSCENENAME); 
            }
        }
    }

    public enum GameState
    {
        MainMenu=0,
        Playing=1,
        Paused=2,
        EndScreen=3
    }
    
    
}
