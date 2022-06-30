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
    [Header("Menus")]
    [SerializeField] private RectTransform mainMenu;
    [SerializeField] private RectTransform levelMenu;
    [SerializeField] private RectTransform optionslMenu;
    [SerializeField] private RectTransform pauseMenu;
    [Header("Options")]
    [SerializeField] private RectTransform volumePanel;
    [SerializeField] private RectTransform colorsPanel;
    [SerializeField] private RectTransform controlsPanel;
    [Header("Components")]
    [SerializeField] private RectTransform advantageBar;
    [SerializeField] private TextMeshProUGUI timer;
    [SerializeField] private RectTransform titlePanel;
    [SerializeField] private Button pauseButton;


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
        if (SceneManager.GetActiveScene().name.Contains(ParamManager.Instance.LEVELSCENENAME))
        {
            UpdateColors();
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
         if (titlePanel)
            titlePanel.gameObject.SetActive(false);

    }

    private void SetStartMenu()
    {
        if (mainMenu)
        {
            mainMenu.gameObject.SetActive(true);
            menusSeries.Push(mainMenu);
        }
        if (titlePanel)
            titlePanel.gameObject.SetActive(true);
    }
    private void SetGameOverScreen()
    {
        if (mainMenu)
        {
            SetGameOverMainMenu();
        }
        if (advantageBar)
        {
            DrawAdvantageBar();
        }
        if (titlePanel)
        {
            titlePanel.gameObject.SetActive(true);
            SetGameEndText(titlePanel.GetComponentInChildren<TextMeshProUGUI>())
;        }
    }

    private void SetGameOverMainMenu()
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
        UpdateColors();
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

        Vector3 newScale = advantageBar.GetChild(0).localScale;
        if (GameManager.Instance.enemyPlanetsCount <= 0)//if enemy is zero dont devide by 0
        {
            newScale.x = 1;
        }
        else
        {
            //scale is playercount divided by sum of player and enemy count
            newScale.x = (float)GameManager.Instance.playerPlanetsCount /(GameManager.Instance.playerPlanetsCount+ GameManager.Instance.enemyPlanetsCount);
        }
        advantageBar.GetChild(0).localScale = newScale;
    }

    private void DrawTimer()//update the timer text
    {
        int time = (int)GameManager.Instance.endGameTimer;
        if (timer)
        {
            timer.text = ((time / 60)).ToString("00") + ":" + (time % 60).ToString("00");
        }
    }
    private void SetGameEndText(TextMeshProUGUI text)
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
            HideTitle();
            menusSeries.Peek().gameObject.SetActive(false);
            levelMenu.gameObject.SetActive(true);
            menusSeries.Push(levelMenu);
            GoToLevelPage();
        }
    }
    public void OptionsMenu()//go to option menu
    {
        if (optionslMenu)
        {
            HideTitle();
            menusSeries.Peek().gameObject.SetActive(false);
            optionslMenu.gameObject.SetActive(true);
            menusSeries.Push(optionslMenu);
            ControlsOptions();
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
            UnHideTitle();
            menusSeries.Pop().gameObject.SetActive(false);
            menusSeries.Peek().gameObject.SetActive(true);
        }
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
                if (pauseButton)
                    pauseButton.image.sprite = ParamManager.Instance.UnpauseButtonSprite;
            }
            else
            {
                GameManager.Instance.IsPaused = false;
                if (menusSeries.Count > 0)
                    menusSeries.Peek().gameObject.SetActive(false);
                menusSeries.Clear();
                if (pauseButton)
                    pauseButton.image.sprite = ParamManager.Instance.PauseButtonSprite;
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
    public void ColorOptions()
    {
        if (menusSeries.Peek() == optionslMenu)
        {
            DisableAllOptions();
            if (colorsPanel)
            {
                colorsPanel.gameObject.SetActive(true);
                float h;
                float s;
                float v;
                Color.RGBToHSV(ParamManager.Instance.PlayerColor, out h, out s, out v);
                colorsPanel.GetComponentsInChildren<Scrollbar>().ElementAt(0).value = h;
                Color.RGBToHSV(ParamManager.Instance.EnemyColor, out h, out s, out v);
                colorsPanel.GetComponentsInChildren<Scrollbar>().ElementAt(1).value = h;
                Color.RGBToHSV(ParamManager.Instance.NeutralColor, out h, out s, out v);
                colorsPanel.GetComponentsInChildren<Scrollbar>().ElementAt(2).value = h;
            }
        }
    }
    public void VolumeOptions()
    {
        if (menusSeries.Peek() == optionslMenu)
        {
            DisableAllOptions();
            if (volumePanel)
                volumePanel.gameObject.SetActive(true);
        }
    }
    public void ControlsOptions()
    {
        if (menusSeries.Peek() == optionslMenu)
        {
            DisableAllOptions();
            if (controlsPanel)
                controlsPanel.gameObject.SetActive(true);
        }
    }
    public void CahngeVolume(float value)
    {

    }
    public void ChangePlayerColor(float value)
    {
        float h;
        float s;
        float v;
        Color.RGBToHSV(ParamManager.Instance.PlayerColor, out h, out s, out v);
        ParamManager.Instance.PlayerColor= Color.HSVToRGB(value, s, v);
        Color.RGBToHSV(ParamManager.Instance.PlayerHighlightColor, out h, out s, out v);
        ParamManager.Instance.PlayerHighlightColor = Color.HSVToRGB(value, s, v);
        colorsPanel.GetComponentsInChildren<Image>().Where(i=>i.CompareTag(ParamManager.Instance.COLORSAMPLETAG)).ElementAt(0).color = ParamManager.Instance.PlayerColor;
        UpdateColors();
    }
    public void ChangeEnemyColor(float value)
    {
        float h;
        float s;
        float v;
        Color.RGBToHSV(ParamManager.Instance.EnemyColor, out h, out s, out v);
        ParamManager.Instance.EnemyColor = Color.HSVToRGB(value, s, v);
        Color.RGBToHSV(ParamManager.Instance.EnemyHighlightColor, out h, out s, out v);
        ParamManager.Instance.EnemyHighlightColor = Color.HSVToRGB(value, s, v);
        colorsPanel.GetComponentsInChildren<Image>().Where(i => i.CompareTag(ParamManager.Instance.COLORSAMPLETAG)).ElementAt(1).color = ParamManager.Instance.EnemyColor;
        UpdateColors();
    }
    public void ChangeNeutralColor(float value)
    {
        float h;
        float s;
        float v;
        Color.RGBToHSV(ParamManager.Instance.NeutralColor, out h, out s, out v);
        ParamManager.Instance.NeutralColor = Color.HSVToRGB(value, s, v);
        Color.RGBToHSV(ParamManager.Instance.NeutralHighlightColor, out h, out s, out v);
        ParamManager.Instance.NeutralHighlightColor = Color.HSVToRGB(value, s, v);
        colorsPanel.GetComponentsInChildren<Image>().Where(i => i.CompareTag(ParamManager.Instance.COLORSAMPLETAG)).ElementAt(2).color = ParamManager.Instance.NeutralColor;
        UpdateColors();
    }
    private void UpdateColors()
    {
        foreach(MouseInteractable obj in FindObjectsOfType<MouseInteractable>())
        {
            obj.UpdateColorAndHighlight();
        }
        advantageBar.GetComponent<Image>().color = ParamManager.Instance.EnemyColor;
        advantageBar.GetChild(0).GetComponent<Image>().color = ParamManager.Instance.PlayerColor;
    }

    private void DisableAllOptions()
    {
        if (menusSeries.Peek() == optionslMenu)
        {
            if (colorsPanel)
                colorsPanel.gameObject.SetActive(false);
            if (volumePanel)
                volumePanel.gameObject.SetActive(false);
            if (controlsPanel)
                controlsPanel.gameObject.SetActive(false);
        }
    }
    private void GoToLevelPage()
    {
        List<Button> buttons = levelMenu.GetComponentsInChildren<Button>().Where(b => b.CompareTag(ParamManager.Instance.LEVELBUTTONTAG)).OrderByDescending(b => b.transform.position.y).ThenBy(b => b.transform.position.x).ToList();
        int i = 0;
        if (SceneManager.GetActiveScene().name == ParamManager.Instance.MAINMENUSCENENAME)
        {
            if (page == 0)
            {
                buttons[0].onClick.RemoveAllListeners();
                buttons[0].onClick.AddListener(() => GoToLevel(ParamManager.Instance.LevelTutorialSceneName+"1"));
                buttons[0].GetComponentInChildren<TextMeshProUGUI>().text = ParamManager.Instance.LevelTutorialSceneName + "1";
                buttons[1].onClick.RemoveAllListeners();
                buttons[1].onClick.AddListener(() => GoToLevel(ParamManager.Instance.LevelTutorialSceneName + "2"));
                buttons[1].GetComponentInChildren<TextMeshProUGUI>().text = ParamManager.Instance.LevelTutorialSceneName + "2";
                buttons[2].onClick.RemoveAllListeners();
                buttons[2].onClick.AddListener(() => GoToLevel(ParamManager.Instance.LevelTutorialSceneName + "3"));
                buttons[2].GetComponentInChildren<TextMeshProUGUI>().text = ParamManager.Instance.LevelTutorialSceneName + "3";
                i=3;
            }
            for (; i < buttons.Count; i++)
            {
                int levelNum = ((page * buttons.Count) + i -2);
                buttons[i].onClick.RemoveAllListeners();
                buttons[i].onClick.AddListener(() => GoToLevel(ParamManager.Instance.LEVELSCENENAME + levelNum));
                buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = ParamManager.Instance.LEVELSCENENAME + levelNum;
            }
        }

        else
        {
            for (; i < buttons.Count; i++)
            {
                int levelNum = ((page * buttons.Count) + i + 1);
                buttons[i].onClick.RemoveAllListeners();
                buttons[i].onClick.AddListener(() => GoToLevel(ParamManager.Instance.LEVELSCENENAME + levelNum));
                buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = ParamManager.Instance.LEVELSCENENAME + levelNum;
            }
        }

    }
    private void HideTitle()
    {
        if (SceneManager.GetActiveScene().name == ParamManager.Instance.MAINMENUSCENENAME ||
                SceneManager.GetActiveScene().name == ParamManager.Instance.GAMEOVERSCENENAME)
        {
            if (titlePanel)
                titlePanel.gameObject.SetActive(false);
            if (advantageBar)
                advantageBar.gameObject.SetActive(false);
        }
    }
    private void UnHideTitle()
    {
        if (SceneManager.GetActiveScene().name == ParamManager.Instance.MAINMENUSCENENAME ||
                SceneManager.GetActiveScene().name == ParamManager.Instance.GAMEOVERSCENENAME)
        {
            if (menusSeries.Peek() == levelMenu || menusSeries.Peek() == optionslMenu)
            {
                if (titlePanel)
                    titlePanel.gameObject.SetActive(true);
                if (advantageBar)
                    advantageBar.gameObject.SetActive(true);
            }
        }
    }
}
