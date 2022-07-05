using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Linq;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;//singelton
    Coroutine tutorialCoroutine;
    [SerializeField] private List<RectTransform> tutorialPanels=new List<RectTransform>();
    [SerializeField] private List<Planet> planets = new List<Planet>();
    [SerializeField] private CameraController cameraInput;
    [SerializeField] private float camMoveXTrigger = 5;
    private float camStartingPos;

    private void Awake()
    {
        SetSingelton();
    }
    private void SetSingelton()
    {
        if (Instance != null && Instance != this)// implement singelton
        {
            if (Application.isPlaying)
            {
                Destroy(this);
            }
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        DisablePanels();
        SetCamera();
        if (SceneManager.GetActiveScene().name.Contains("1")){
            
            tutorialCoroutine = StartCoroutine(Tutorial1Sequence());
        }
        if (SceneManager.GetActiveScene().name.Contains("2")){
            tutorialCoroutine = StartCoroutine(Tutorial2Sequence());
        }
        if (SceneManager.GetActiveScene().name.Contains("3")){
            
            tutorialCoroutine = StartCoroutine(Tutorial3Sequence());
        }
    }

    private void SetCamera()
    {
        cameraInput.isCameraMovementDisabled = true;
    }
    private void DisablePanels()
    {
        foreach (RectTransform rec in tutorialPanels)
            rec.gameObject.SetActive(false);
    }
    private void DisableEnemyPlanets()
    {
        foreach (Planet p in planets.Where(planet => planet.HiveType == HiveController.Hive.Enemy))
        {
            p.DisablePlanet();
        }
    }
    IEnumerator Tutorial1Sequence()
    {
        yield return new WaitForSeconds(0.01f);
        DisableEnemyPlanets();
        tutorialPanels[0].gameObject.SetActive(true);
        yield return new WaitUntil(() => planets[0].isClicked);
        planets[1].EnablePlanet();
        tutorialPanels[0].gameObject.SetActive(false);
        tutorialPanels[1].gameObject.SetActive(true);
        yield return new WaitUntil(() => planets[0].IsCapturingTarget(planets[1]));
        tutorialPanels[1].gameObject.SetActive(false);
    }

    IEnumerator Tutorial2Sequence()
    {
        tutorialPanels[0].gameObject.SetActive(true);
        yield return new WaitUntil(() => planets[0].isClicked);
        tutorialPanels[0].gameObject.SetActive(false);
        
    }
    IEnumerator Tutorial3Sequence()
    {
        yield return new WaitForSeconds(0.01f);
        DisableEnemyPlanets();
        tutorialPanels[0].gameObject.SetActive(true);
        yield return new WaitUntil(() => SecondRingCapture());
        cameraInput.isCameraMovementDisabled = false;
        planets[13].EnablePlanet();
        tutorialPanels[0].gameObject.SetActive(false);
        tutorialPanels[1].gameObject.SetActive(true);
        yield return new WaitUntil(() => cameraInput.isPanPressed);
        tutorialPanels[1].gameObject.SetActive(false);
    }
    private bool SecondRingCapture()
    {
        if (planets.Count >= 14)
            return planets[5].HiveType == HiveController.Hive.Player || planets[6].HiveType == HiveController.Hive.Player ||
                planets[7].HiveType == HiveController.Hive.Player || planets[8].HiveType == HiveController.Hive.Player;
        return false;
    }
    
    public void DisableTutorialPanels()
    {
        tutorialPanels.ElementAt(0).parent.gameObject.SetActive(false);
    }
    public void EnableTutorialPanels()
    {
        tutorialPanels.ElementAt(0).parent.gameObject.SetActive(true);
    }
}
