using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Linq;
using System;

public class MenuManager : MonoBehaviour
{
    //references
    [SerializeField] private RectTransform mainMenu;
    [SerializeField] private RectTransform levelMenu;
    [SerializeField] private RectTransform optionslMenu;
    [SerializeField] private RectTransform pauseMenu;
    [SerializeField] private RectTransform advantageProgressBar;
    [SerializeField] private TextMeshProUGUI timer;
    [SerializeField] private RectTransform winPanel;

    private Stack<RectTransform> menusSeries = new Stack<RectTransform>();
    
    private int page = 0;
    [SerializeField] [Min(0)] int maxLevelPage = 0;
    private void Start()
    {
        DisablePanels();
        if (SceneManager.GetActiveScene().name == ParamManager.Instance.MAINMENUSCENENAME)
        {
            SetStartMenu();
        }
        if (SceneManager.GetActiveScene().name == ParamManager.Instance.GAMEOVERSCENENAME)
        {
            SetGameOverScreen();
        }
    }

    private void DisablePanels()
    {
        if (mainMenu)
            mainMenu.gameObject.SetActive(false);
        if (levelMenu)
            levelMenu.gameObject.SetActive(false);
        if (optionslMenu)
            optionslMenu.gameObject.SetActive(false);
         if (pauseMenu)
            pauseMenu.gameObject.SetActive(false);
         if (winPanel)
            winPanel.gameObject.SetActive(false);

    }

    private void SetStartMenu()
    {
        if (mainMenu)
        {
            mainMenu.gameObject.SetActive(true);
            menusSeries.Push(mainMenu);
        }
    }
    private void SetGameOverScreen()
    {
        if (mainMenu)
        {
            SetGaeOverMainMenu();
        }
        if (advantageProgressBar)
        {
            DrawAdvantageBar();
        }
        if (winPanel)
        {
            winPanel.gameObject.SetActive(true);
            SetGaemeEndText(winPanel.GetComponentInChildren<TextMeshProUGUI>())
;        }
    }

    private void SetGaeOverMainMenu()
    {
        mainMenu.gameObject.SetActive(true);
        menusSeries.Push(mainMenu);
        if (GameManager.Instance.playerPlanetsCount >= GameManager.Instance.enemyPlanetsCount)
        {
            mainMenu.GetChild(0).gameObject.SetActive(true);
            mainMenu.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            mainMenu.GetChild(0).gameObject.SetActive(false);
            mainMenu.GetChild(1).gameObject.SetActive(true);
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
    private void SetGaemeEndText(TextMeshProUGUI text)
    {
        if (text)
        {
            if(GameManager.Instance.playerPlanetsCount >= GameManager.Instance.enemyPlanetsCount)
            {
                text.text = ParamManager.Instance.WinText;
            }
            else
            {
                text.text = ParamManager.Instance.LooseText;
            }

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
        Debug.Log("GoTOLevel "+level);
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
        if (SceneManager.GetActiveScene().name.Contains(ParamManager.Instance.LEVELSCENENAME))
        {
            if (!GameManager.Instance.IsPaused)
            {
                GameManager.Instance.IsPaused = true;
                if (pauseMenu)
                {
                    if (menusSeries.Count>0)
                        menusSeries.Peek().gameObject.SetActive(false);
                    pauseMenu.gameObject.SetActive(true);
                    menusSeries.Push(pauseMenu);
                }
            }
            else
            {
                GameManager.Instance.IsPaused = false;
                if (menusSeries.Count > 0)
                    menusSeries.Peek().gameObject.SetActive(false);
                menusSeries.Clear();
            }
        }
    }
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void ReplayLeve()
    {
        Debug.Log("Replay " + GameManager.Instance.levelName);
        if (GameManager.Instance.levelName != "")
        {
            
            SceneManager.LoadScene(GameManager.Instance.levelName);
        }
    }
    public void NextLevel()
    {
        Debug.Log("Last Level: " + GameManager.Instance.levelName);
        if (GameManager.Instance.levelName != "")
        {
            try {
                int level = Int32.Parse(GameManager.Instance.levelName.Replace("Level", ""));
                Debug.Log("Next Play " + "Level" + (level + 1));
                SceneManager.LoadScene("Level"+(level+1));
            }
            catch (FormatException)
            {
                Debug.LogError("No such Level");
            }
            
        }
    }
    public void NextLevelPage()
    {
        if (levelMenu && levelMenu.gameObject.activeInHierarchy)
        {
            
            if (page < maxLevelPage)
            {
                page++;
                GoToLevelPage();
            }
        }
    }    
    public void BackLevelPage()
    {
        if (levelMenu && levelMenu.gameObject.activeInHierarchy)
        {
            
            if (page > 0)
            {
                page--;
                GoToLevelPage();
            }
        }
    }
    private void GoToLevelPage()
    {
        List<Button> buttons = levelMenu.GetComponentsInChildren<Button>().Where(b => b.GetComponentInChildren<TextMeshProUGUI>()).OrderByDescending(b => b.transform.position.y).ThenBy(b => b.transform.position.x).ToList();
        int i = 0;
        if (SceneManager.GetActiveScene().name == ParamManager.Instance.MAINMENUSCENENAME)
        {
            Debug.Log("Main");
            if (page == 0)
            {
                buttons[0].onClick.RemoveAllListeners();
                buttons[0].onClick.AddListener(() => GoToLevel("LevelTutorial"));
                buttons[0].GetComponentInChildren<TextMeshProUGUI>().text = "LevelTutorial";
                i++;
            }
            for (; i < 5; i++)
            {
                buttons[i].onClick.RemoveAllListeners();
                buttons[i].onClick.AddListener(() => GoToLevel("Level" + (page * 5 + i)));
                buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = "Level" + (page * 5 + i);
            }
        }

        else
        {
            for (; i < 5; i++)
            {
                
                int levNum = ((page * 5) + i + 1);
                Debug.Log("Button: " +levNum);
                buttons[i].onClick.RemoveAllListeners();
                buttons[i].onClick.AddListener(() => GoToLevel("Level" + levNum));
                buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = "Level" + levNum;
            }
        }

    }
}
