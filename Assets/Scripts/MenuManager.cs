using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    //references
    [SerializeField] private RectTransform mainMenu;
    [SerializeField] private RectTransform levelMenu;
    [SerializeField] private RectTransform optionslMenu;

    private Stack<RectTransform> menus = new Stack<RectTransform>();
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
                menus.Push(mainMenu);
            }
        }
    }
    public void LevelsMenu()//go to level menu
    {
        if (levelMenu)
        {
            menus.Peek().gameObject.SetActive(false);
            levelMenu.gameObject.SetActive(true);
            menus.Push(levelMenu);
        }
    }
    public void OptionsMenu()//go to option menu
    {
        if (optionslMenu)
        {
            menus.Peek().gameObject.SetActive(false);
            optionslMenu.gameObject.SetActive(true);
            menus.Push(optionslMenu);
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
        if (menus.Count > 1)
        {
            menus.Pop().gameObject.SetActive(false);
            menus.Peek().gameObject.SetActive(true);
        }
    }
    public void CahngeVolume()
    {

    }
    public void PauseGame()
    {

    }
}
