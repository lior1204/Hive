using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;//singelton
    Coroutine tutorialCoroutine;
    [SerializeField] private List<RectTransform> tutorialPanels=new List<RectTransform>();
    [SerializeField] private List<Planet> planets = new List<Planet>();
    [SerializeField] private CameraController cameraInput;
    [SerializeField] private float panel1Time = 4f;
    [SerializeField] private float panel5Time = 4f;
    [SerializeField] private float panel9Time = 4f;
    [SerializeField] private float camMoveXTrigger = 5;
    private float camStartingPos;

    //events
    public bool isClickPlanet0 { get { return planets[0].isClicked; } }
    public bool istAttackPlanet1 { get { return planets[0].IsCapturingTarget(planets[1]); } }
    public bool isPlanet1Captured { get { return planets[1].HiveType==HiveController.Hive.Player; } }
    public bool isPlanet2Captured { get { return planets[2].HiveType==HiveController.Hive.Player; } }
    public bool isPlanet2Reinforced { get { return planets[1].IsReinforcingTarget(planets[2]) || planets[3].IsReinforcingTarget(planets[2]) ; } }
    public bool isPlanet4Captured { get { return planets[4].HiveType == HiveController.Hive.Player; } }
    public bool isMoveCamera { get { return Camera.main.transform.position.x>camStartingPos+camMoveXTrigger; } }
    public bool isPlanet5Captured { get { return planets[5].HiveType == HiveController.Hive.Player; } }
    public bool isPlanet678Captured { get { return planets[6].HiveType == HiveController.Hive.Player|| planets[7].HiveType == HiveController.Hive.Player|| planets[8].HiveType == HiveController.Hive.Player; } }
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
        //DisablePlanets();
        SetCamera();
        tutorialCoroutine = StartCoroutine(TutorialSequence());
    }

    private void SetCamera()
    {
        cameraInput.isCameraMovementDisabled = true;
        camStartingPos = Camera.main.transform.position.x;
    }

    private void DisablePanels()
    {
        foreach (RectTransform rec in tutorialPanels)
            rec.gameObject.SetActive(false);
    }
    private void DisablePlanets()
    {
        Debug.Log("Disable Enemies");
        //planets[5].enabled = false;
        //planets[9].enabled = false;
        //planets[11].enabled = false;
    }
    IEnumerator TutorialSequence()
    {
        tutorialPanels[0].gameObject.SetActive(true);
        yield return new WaitForSeconds(panel1Time);
        tutorialPanels[0].gameObject.SetActive(false);
        tutorialPanels[1].gameObject.SetActive(true);
        yield return new WaitUntil(() => isClickPlanet0);
        tutorialPanels[1].gameObject.SetActive(false);
        tutorialPanels[2].gameObject.SetActive(true);
        yield return new WaitUntil(() => istAttackPlanet1);
        tutorialPanels[2].gameObject.SetActive(false);
        tutorialPanels[3].gameObject.SetActive(true);
        yield return new WaitUntil(() => isPlanet1Captured); 
        tutorialPanels[3].gameObject.SetActive(false);
        tutorialPanels[4].gameObject.SetActive(true);
        yield return new WaitForSeconds(panel5Time);
        tutorialPanels[4].gameObject.SetActive(false);
        tutorialPanels[5].gameObject.SetActive(true);
        yield return new WaitUntil(() => isPlanet2Captured);
        tutorialPanels[5].gameObject.SetActive(false);
        tutorialPanels[6].gameObject.SetActive(true);
        yield return new WaitUntil(() => isPlanet2Reinforced||isPlanet4Captured);
        tutorialPanels[6].gameObject.SetActive(false);
        if (isPlanet2Reinforced && !isPlanet4Captured)
        {
            tutorialPanels[7].gameObject.SetActive(true);
            yield return new WaitUntil(() => isPlanet4Captured);
        }
        tutorialPanels[7].gameObject.SetActive(false);
        tutorialPanels[8].gameObject.SetActive(true);
        cameraInput.isCameraMovementDisabled = false;
        yield return new WaitUntil(() => isMoveCamera);
        tutorialPanels[8].gameObject.SetActive(false);
        tutorialPanels[9].gameObject.SetActive(true);
        planets[5].enabled = true;
        yield return new WaitForSeconds(panel9Time);
        tutorialPanels[9].gameObject.SetActive(false);
        tutorialPanels[10].gameObject.SetActive(true);
        yield return new WaitUntil(() => isPlanet5Captured);
        tutorialPanels[10].gameObject.SetActive(false);
        tutorialPanels[11].gameObject.SetActive(true);
        yield return new WaitUntil(() => isPlanet678Captured);
        planets[9].enabled = true;
        planets[11].enabled = true;
        tutorialPanels[11].gameObject.SetActive(false);
        tutorialPanels[12].gameObject.SetActive(true);
    }
    
}
