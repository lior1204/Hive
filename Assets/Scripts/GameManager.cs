using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;//singelton

    //state 
    public bool IsPaused { get; private set; } = false;
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
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //get & set
    
}
