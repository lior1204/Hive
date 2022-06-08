using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
public class ParamManager : MonoBehaviour
{
    public static ParamManager Instance;//singelton

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


    public PlanetSizeParameters[] planetSizeSet =new PlanetSizeParameters[3];
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
