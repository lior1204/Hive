using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class MenuManager : MonoBehaviour
{
    //references
    [SerializeField] private RectTransform mainMenu;
    [SerializeField] private RectTransform levelMenu;
    [SerializeField] private RectTransform optionslMenu;
    [SerializeField] private RectTransform advantageProgressBar;
    [SerializeField] private TextMeshProUGUI timer;
    private Stack<RectTransform> menusSeries = new Stack<RectTransform>();
    private void Start()
    {
        if (mainMenu)
            mainMenu.gameObject.SetActive(false);
        if (levelMenu)
            levelMenu.gameObject.SetActive(false);
        if (optionslMenu)
            optionslMenu.gameObject.SetActive(false);
        if (SceneManager.GetActiveScene().name == ParamManager.Instance.MAINMENUSCENENAME)
        {
            if (mainMenu)
            {
                mainMenu.gameObject.SetActive(true);
                menusSeries.Push(mainMenu);
            }
        }
        if (SceneManager.GetActiveScene().name == ParamManager.Instance.GAMEOVERSCENENAME)
        {
            if (mainMenu)
            {
                mainMenu.gameObject.SetActive(true);
                menusSeries.Push(mainMenu);
            }
            if (advantageProgressBar)
            {
                DrawAdvantageBar();
            }
        }
    }
    private void Update()
    {
        if (SceneManager.GetActiveScene().name.Contains("Level"))
        {
            DrawTimer();
            DrawAdvantageBar();
        }
    }

    private void DrawAdvantageBar()//change the scale of the progress
    {

        Vector3 newScale = advantageProgressBar.localScale;
        if (GameManager.Instance.enemyPlanetsCount <= 0)//if enemy is zero dont devide by 0
        {
            newScale.x = 1;
        }
        else
        {
            //scale is playercount divided by sum of player and enemy count
            newScale.x = (float)GameManager.Instance.playerPlanetsCount /(GameManager.Instance.playerPlanetsCount+ GameManager.Instance.enemyPlanetsCount);
        }
        advantageProgressBar.localScale = newScale;
    }

    private void DrawTimer()//update the timer text
    {
        int time = (int)GameManager.Instance.endGameTimer;
        if (timer)
        {
            timer.text = ((time / 60)).ToString("00") + ":" + (time % 60).ToString("00");
        }
    }

    public void LevelsMenu()//go to level menu
    {
        if (levelMenu)
        {
            menusSeries.Peek().gameObject.SetActive(false);
            levelMenu.gameObject.SetActive(true);
            menusSeries.Push(levelMenu);
        }
    }
    public void OptionsMenu()//go to option menu
    {
        if (optionslMenu)
        {
            menusSeries.Peek().gameObject.SetActive(false);
            optionslMenu.gameObject.SetActive(true);
            menusSeries.Push(optionslMenu);
        }
    }
    public void QuitGame()//exit game
    {
        Application.Quit();
    }
    public void GoToLevel(string level)
    {
        Scene newScene = SceneManager.GetSceneByName(level);
        if (newScene != null)
            SceneManager.LoadScene(level);
    }
    public void GoBack()
    {
        if (menusSeries.Count > 1)
        {
            menusSeries.Pop().gameObject.SetActive(false);
            menusSeries.Peek().gameObject.SetActive(true);
        }
    }
    public void CahngeVolume()
    {

    }
    public void PauseGame()
    {

    }
}
