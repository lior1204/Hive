using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ReferenceManager : MonoBehaviour
{
    public static ReferenceManager Instance;//singelton

    [Header("Tags")]
    [SerializeField] private string playerTag = "Player";
    public string PLAYERTAG { get { return playerTag; } private set { } }

    [SerializeField] private string enemyTag = "Enemy";
    public string ENEMYTAG { get { return enemyTag; } private set { } }
    
    [Space(3)]
    [Header("Prefabs")]
    [SerializeField] private TextMeshPro _strengthDisplayPrefab;
    public TextMeshPro _StrengthDisplayPrefab { get { return _strengthDisplayPrefab; }private set { } }

    [Space(3)]
    [Header("Global Parameters")]
    [SerializeField] private Color playerColor = new Color(0x76,0xAD,0x75);
    public Color PlayerColor { get { return playerColor; } private set { } }
    
    [SerializeField] private Color enemyColor = new Color(0xB7,0x52,0x5C);
    public Color EnemyColor { get { return enemyColor; } private set { } }

    [SerializeField] private Color neutralColor = new Color(0x44,0x37,0x38);
    public Color NeutralColor { get { return neutralColor; } private set { } }

    [SerializeField] private float captureDecreaseStrengthRate = 0.9f;
    public float CaptureLoseStrengthRate { get { return captureDecreaseStrengthRate; }private set { } }
   
    [SerializeField] private float captureImunityTime = 3f;
    public float CaptureImunityTime { get { return captureImunityTime; } private set { } }


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
}
