using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;//singelton
    //parameters
    [SerializeField] [Min(1)] private float GameTime = 180;//in seconds
    //state 
    public bool IsPaused { get { return state == GameState.Paused; } }
    private GameState state = GameState.Playing;
    private float endGameTimer = 0;
    private bool IsTimeOver { get { return endGameTimer >= GameTime; } }
    private int playerPlanetsCount;
    private int enemyPlanetsCount;
    private int neutralPlanetsCount;
    private void Awake()
    {
        if (Instance != null && Instance != this)// implement singelton
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    void Update()
    {
        UpdateGameClock();
        CheckIfGameEnd();
    }

    private void UpdateGameClock()//up the clock if game is running
    {
        if (state == GameState.Playing)
        {
            endGameTimer += Time.deltaTime;
        }
    }

    private void CheckIfGameEnd()
    {
        if (state == GameState.Playing)//check if game is running
        {
            playerPlanetsCount = HiveController.Player.PlanetCount;
            enemyPlanetsCount = HiveController.Enemy.PlanetCount;
            if (playerPlanetsCount <= 0 || enemyPlanetsCount <= 0 || IsTimeOver)//game ends if player or enemy has no planets or if time over
            {
                //neutralPlanetsCount=?
                SceneManager.LoadScene(ParamManager.Instance.GAMEOVERSCENENAME); 
            }
        }
    }

    //get & set

    public enum GameState
    {
        MainMenu=0,
        Playing=1,
        Paused=2,
        EndScreen=3
    }
}
