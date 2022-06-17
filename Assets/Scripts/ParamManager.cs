using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
public class ParamManager : MonoBehaviour
{
    public static ParamManager Instance;//singelton

    public bool nonPlayerMaskActive = false;

    [Header("Tags")]
    
    [SerializeField] private string playerTag = "Player";
    public string PLAYERTAG { get { return playerTag; } }

    [SerializeField] private string enemyTag = "Enemy";
    public string ENEMYTAG { get { return enemyTag; } }

    [SerializeField] private string planetTag = "Planet";
    public string PLANETTAG { get { return planetTag; } }
    
    [SerializeField] private string linkTag = "Planet";
    public string LINKTAG { get { return linkTag; } }
    
    [SerializeField] private string fogMaskTag = "FogMask";
    public string FOGMASKTAG { get { return fogMaskTag; } }

    [SerializeField] private string capturePoolerTag = "Capture";
    public string CAPTUREPOOLTAG { get { return capturePoolerTag; } }
     
    [SerializeField] private string reinforcementPoolerTag = "Reinforcement";
    public string REINFORCEMENTPOOLTAG { get { return reinforcementPoolerTag; } }
    
    [SerializeField] private string borderTopLeft = "Border_Top_Left";
    public string BORDERTOPLEFT { get { return borderTopLeft; } }
    
    [SerializeField] private string borderBottomRight = "Border_Bottom_Right";
    public string BORDERBOTTOMRIGHT { get { return borderBottomRight; } }
    
    [SerializeField] private string level01SceneName = "Level1";
    public string LEVEL01SCENENAME { get { return level01SceneName; } }
    
    [SerializeField] private string gameOverSceneName = "GameOverScreen";
    public string GAMEOVERSCENENAME { get { return gameOverSceneName; } }
    
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    public string MAINMENUSCENENAME { get { return mainMenuSceneName; } }

    [Space(3)]
    [Header("Prefabs")]
    [SerializeField] private TextMeshPro _strengthDisplayPrefab;
    public TextMeshPro _StrengthDisplayPrefab { get { return _strengthDisplayPrefab; } }

    [Space(3)]
    [Header ("Global Parameters")]

    [SerializeField] private Color playerColor = new Color(0x76, 0xAD, 0x75);
    public Color PlayerColor { get { return playerColor; } }

    [SerializeField] private Color enemyColor = new Color(0xB7, 0x52, 0x5C);
    public Color EnemyColor { get { return enemyColor; } }

    [SerializeField] private Color neutralColor = new Color(0x44, 0x37, 0x38);
    public Color NeutralColor { get { return neutralColor; } }

    [SerializeField] private Color playerHighlightColor = new Color(0x76, 0xAD, 0x75);
    public Color PlayerHighlightColor { get { return playerHighlightColor; } }

    [SerializeField] private Color enemyHighlightColor = new Color(0xB7, 0x52, 0x5C);
    public Color EnemyHighlightColor { get { return enemyHighlightColor; } }

    [SerializeField] private Color neutralHighlightColor = new Color(0x44, 0x37, 0x38);
    public Color NeutralHighlightColor { get { return neutralHighlightColor; } }

    [Header ("Capture Parameters")]
    [SerializeField] private float strengthUpdateRate = 0.8f;
    public float StrengthUpdateRate { get { return strengthUpdateRate; } }

    [SerializeField] private float captureStrengthOutcome = 3;
    public float CaptureStrengthOutcome { get { return captureStrengthOutcome; } }
           
    [SerializeField] private float zeroStrengthReducedOutcome = 1;
    public float ZeroStrengthReducedOutcome { get { return zeroStrengthReducedOutcome; } }

    [SerializeField] private float captureImunityTime = 3f;
    public float CaptureImunityTime { get { return captureImunityTime; } }

    [SerializeField] private float reinforceBonus = 1f;
    public float ReinforceBonus { get { return reinforceBonus; } }
    
    [SerializeField] private float reinforceCost = 0.5f;
    public float ReinforceCost { get { return reinforceCost; } }
    [SerializeField] private float strengthCap = 100f;
    public float StrengthCap { get { return strengthCap; } }

    [Header ("Planet Parameters")]
    public PlanetSizeParameters[] planetSizeSet =new PlanetSizeParameters[3];
    private void Awake()
    {
        SetSingelton();
    }

    [Header("Enemy Threshold Parameters")]
    [SerializeField][Range(1f,100)] private int smallSizeScore = 0;
    public float SmallSizeScore { get { return smallSizeScore; } }
    [SerializeField] [Range(1f, 100)] private int mediumSizeScore = 20;
    public float MediumSizeScore { get { return mediumSizeScore; } }
    [SerializeField] [Range(1f, 100)] private int bigSizeScore = 50;
    public float BigSizeScore { get { return bigSizeScore; } }
    [SerializeField] [Range(1f, 100)] private int strengthScore = 50;
    public float StrengthScore { get { return strengthScore; } }
    [SerializeField] [Range(1f, 100)] private int incomeScore = 50;
    public float IncomeScore { get { return incomeScore; } }
    [SerializeField] [Range(1f, 100)] private int neutralScore = 20;
    public float NeutralScore { get { return neutralScore; } }
    [SerializeField] [Range(1f, 100)] private int playerScore = 20;
    public float PlayerScore { get { return playerScore; } }
    [SerializeField] [Range(1f, 500)] private int randomScore = 100;
    public float RandomScore { get { return randomScore; } }
    [SerializeField] [Range(1f,2f)] private Vector2 relativityScoreModifier = new Vector2(1,2);
    public Vector2 RelativityScoreModifier { get { return relativityScoreModifier; } }

    private void SetSingelton()
    {
        if (Instance != null && Instance != this)// implement singelton
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            if(Application.isPlaying)
            DontDestroyOnLoad(this.gameObject);
        }
    }
    private void OnValidate()
    {
        SetSingelton();
    }
    [Serializable]
    public class PlanetSizeParameters
    {
        public Planet.PlanetSize size;
        public float strengthIncome = 2;
        public int maxActiveLinks = 2;
        public float orbitCycleTime = 5f;
        public float captureRange = 30;
        public float visibilityRange = 150;
    }
}
