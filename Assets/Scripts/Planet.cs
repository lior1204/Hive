using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;


public class Planet : MouseInteractable
{
    private static int IDCount = 0;

    //parameters
    [SerializeField] private HiveController.Hive hiveType = HiveController.Hive.Neutral;
    public HiveController.Hive HiveType { get { return hiveType; } }
    [SerializeField] private bool isQueen = false;
    [SerializeField] private int startingStrength = 6;
    [SerializeField] private PlanetSize planetSize = PlanetSize.Small;

    [SerializeField] private float strengthIncome = 2;
    [SerializeField] private int maxActiveLinks = 2;
    [SerializeField] private float speed = 5f;
    [SerializeField] [Range(5f, 20f)] private float captureRange = 30;
    [SerializeField] [Range(100f, 300f)] private float visibilityRange = 150;
    [SerializeField] [Range(1, 5)] private int maximumActiveCaptures = 2;

    public int PlanetID { get; private set; } = 0;

    //state variables
    private float strength = 0;
    private List<Capture> captureLinks = new List<Capture>();
    private List<Reinforcement> reinforceLinks = new List<Reinforcement>();
    private List<Link> activeLinks = new List<Link>();
    //private List<Link> myLinks = new List<Link>();
    

    private float captureImunity;
    public bool IsImune { get { return captureImunity <= 0; } }
    

    //references
    private TextMeshPro _strengthDisplay = null;
    private SpriteRenderer _spriteRenderer = null;
    private Coroutine strenghtGrowthCoroutine;
    private HiveController HiveRef //dynamic HiveControllerReference based on controllingHive
    {
        get
        {
            if (hiveType == HiveController.Hive.Player) return HiveController.Player;
            else if (hiveType == HiveController.Hive.Enemy) return HiveController.Enemy;
            else return null;
        }
    }

    void Start()
    {
        PlanetID = IDCount;//set id
        IDCount++;//increase id count
        _strengthDisplay = Instantiate(ParamManager.Instance._StrengthDisplayPrefab);//create strength display
        _spriteRenderer = GetComponent<SpriteRenderer>();//sprite renderer reference
        strength = startingStrength;//st starting strength
        strenghtGrowthCoroutine = StartCoroutine(GenerateStrength());//start strength and save reference
        SetStartInHive();
        captureImunity = ParamManager.Instance.CaptureImunityTime;
    }
    void Update()
    {
        UpdateActiveLinks();
        UpdateStrengthDisplay();
    }
    private void SetStartInHive()//add to hive and set color
    {
        if (isQueen&&HiveRef)
                HiveRef.Queen = this;
        ChangeHive(hiveType);
    }

    private void UpdateStrengthDisplay()// update text and pin to planet
    {
        _strengthDisplay.text = strength + "";
        _strengthDisplay.transform.position = new Vector3(transform.position.x, transform.position.y, -5);
    }
    IEnumerator GenerateStrength()//increase strength
    {
        while (true)
        {
            if (GameManager.Instance.IsPaused)//pause game
                yield return new WaitUntil(() => !GameManager.Instance.IsPaused);
            yield return new WaitForSeconds(ParamManager.Instance.StrengthUpdateRate);//delay between strength ticks
            strength += CalculateDeltaStrength();
            foreach (Capture capture in activeLinks.Where(l=>l.GetType()==typeof(Capture)))//for each capture interaction planet count contribution of capture
            {
                capture.strengthCaptured += strength > 0 ? ParamManager.Instance.CaptureStrengthOutcome
                    : ParamManager.Instance.ZeroStrengthReducedOutcome;//count less contribution if other has 0 strength
            }
            if (strength <= 0)//if reach zero or below get captured
            {
                strength = 0;
                GetCaptured();
            }
        }
    }

    private float CalculateDeltaStrength()// determines strengrh change per strengt tick
    {
        float deltaStrengt = CalculateStrengthIncome() - CalculateStrengthOutcome();
        return deltaStrengt;
    }
    private float CalculateStrengthIncome()//planet's passive income
    {
        if (HiveRef)//if this is in hive it has income if neutral no income
        {
            float income = strengthIncome;//add this planet base income
            income += reinforceLinks.Count(r => r.isActive&&r.Target == this)*ParamManager.Instance.ReinforceBonus;//add bonus from active reinforcements to this planet
            income -= reinforceLinks.Count(r => r.isActive&&r.Origin == this)*ParamManager.Instance.ReinforceCost;//dedact cost of this planet active reinfrociements
            return income; 
            // Mathf.Max(income,0); if we want to make impossible to be negative
        }
        return 0;
    }
    private float CalculateStrengthOutcome()//capture cost for each capture interaction
    {
        float outcome = 0;
        List<Capture> activeConnections = captureLinks.Where(capture => capture.isActive).ToList();//get active captures this planet is part of
        int zeroPlanets = activeConnections.Where(capture => capture.Target==this&&capture.Origin.strength <= 0).Count();//count attacking planets with 0 strength
        outcome += zeroPlanets * ParamManager.Instance.ZeroStrengthReducedOutcome;//planets with 0 strength contribute less to outcome
        int captureLinkCount = 0;//count 2 sided connections wher this is both attacking and attacked by the same planet and both links are active
        foreach(Capture capture in activeConnections)
        {
            foreach (Capture other in activeConnections)
                if (capture.IsReverse(other))
                    captureLinkCount++;
        }
        //outcome is active links - half the 2way connections(they will count twice and we need to count once) - zero planets (they already counted)
        outcome += ParamManager.Instance.CaptureStrengthOutcome * (activeConnections.Count()- captureLinkCount/2 - zeroPlanets);//planets with strength contribute to outcopme
        return outcome;
    }
   
    public void AttemptCapture(Planet captured)//start capture of other planet
    {
        //check if this planet in hive and if other planet is not in this hive
        if (hiveType != HiveController.Hive.Neutral && hiveType != captured.hiveType)
        {
            Capture newLink = new Capture(this, captured);
            if (!captureLinks.Any(capture => capture.CompareExactTo(newLink))) //if not already in capture link list
            {
                captureLinks.Add(newLink);// add capture link to list
                captured.DiscoverLink(newLink);//tell other planet they are under capture
            }
        }
    }
    private void DiscoverLink(Link newLink)//become under capture by another planet
    {
        if (newLink.GetType() == typeof(Capture))//if a capture link
        {
            if (!captureLinks.Any(c => c.CompareExactTo(newLink))) //if not already in under capture interaction with other
            {
                captureLinks.Add((Capture)newLink);// add capture link to list
            }
        }
        else if (newLink.GetType() == typeof(Reinforcement))//if a reinforce link
        {
            if (!reinforceLinks.Any(r => r.CompareExactTo(newLink))) //if not already reinforced by other
            {
                if (!reinforceLinks.Any(r => r.CompareExactTo(newLink))) //if not already reinforced by other
                    reinforceLinks.Add((Reinforcement)newLink);// add reinforce link to list
            }
        }
    }
    private void GetCaptured()//get captured by the hive that captured for the most time.
    {
        if (captureLinks.Any(c => c.GetOther(this).hiveType != HiveController.Hive.Neutral))//if any non neutral capturer in link that this is the target
        {
            //determine hive that captured this planet for the longest among active links
            float playerCaptureDuration = 0;
            float enemyCaptureDuration = 0;
            foreach (Capture link in captureLinks.Where(c => c.isActive))//count active links
            {
                if (link.GetOther(this).hiveType == HiveController.Hive.Player)//count strength taken by player
                {
                    playerCaptureDuration += link.strengthCaptured;
                }
                else if (link.GetOther(this).hiveType == HiveController.Hive.Enemy)//count strength taken by enemy
                {
                    enemyCaptureDuration += link.strengthCaptured;
                }
            }
            if ((playerCaptureDuration <= 0 && enemyCaptureDuration <= 0) || playerCaptureDuration == enemyCaptureDuration)// if no duration or equal duration count all links including non active
            {
                foreach (Capture link in captureLinks.Where(c => !c.isActive))//add to count all not active links
                {
                    if (link.GetOther(this).hiveType == HiveController.Hive.Player)//count strength taken by player
                    {
                        playerCaptureDuration += link.strengthCaptured;
                    }
                    else if (link.GetOther(this).hiveType == HiveController.Hive.Enemy)//count strength taken by enemy
                    {
                        enemyCaptureDuration += link.strengthCaptured;
                    }
                }
            }
            //check for winner if no duration or equal player wins
            if (playerCaptureDuration >= enemyCaptureDuration)//player win if equal
                ChangeHive( HiveController.Hive.Player);
            else
                ChangeHive(HiveController.Hive.Enemy);
            FinishCaptureInteraction();
        }
    }
    private void FinishCaptureInteraction()//remove other from interactions list
    {
        List<Capture> removeCapture = captureLinks.Where(c => c.Origin == this ).ToList();//make a list of links to remove things this is attacking
        List<Reinforcement> removeReinforcement = reinforceLinks;//keep list of reinforcement to remove
        List<Capture> convert = captureLinks.Where(c => c.Target == this && c.Origin.HiveType == this.HiveType).ToList();//make list of links to convert to reinforcement
        //captureLinks.RemoveAll(c => c.origin == this|| (c.target == this && c.origin.HiveType == this.HiveType));//remove captures were this is attacking or attacked by controlling hive
        //reinforceLinks.Clear();//remove all reinforcements
        foreach (Link link in removeCapture) link.DestroyLink();//destroy links
        foreach (Link link in removeReinforcement) link.DestroyLink();//destroy links
        foreach (Capture capture in convert) capture.ConvertToReinforcement();//convert captures by controlling hive to reinforcements

    }
  
    public void AttemptReinforccing(Planet reinforced)//start reinforcing another planet
    {
        //check if this planet in hive and if other planet is in same hive
        if (hiveType != HiveController.Hive.Neutral && hiveType == reinforced.hiveType)
        {
            Reinforcement newLink = new Reinforcement(this, reinforced);
            if (!reinforceLinks.Any(reinforce => reinforce.CompareExactTo(newLink))) //if not already in reinforcement link list
            {
                Reinforcement existing = reinforceLinks.FirstOrDefault(reinforce => newLink.IsReverse(reinforce));//find if areverce reinfrocement exists
                existing.DestroyLink();//if reverse exists destroy it;
                reinforceLinks.Add(newLink);// add reinforce link to list
                reinforced.DiscoverLink(newLink);//tell other planet they are reinforced
            }
        }
    }

    private void UpdateActiveLinks()//check and update for active links in range choose based on time stemps
    {
        List<Link> myLinks = GetMyLinks();
        myLinks.OrderBy(c => c.TimeStemp);//order by timestemp
        activeLinks.Clear();//clear old active links
        activeLinks.AddRange(myLinks.Where(l => IsCapturable(l.Target)).Take(maxActiveLinks));//set new actives equal to max 
        foreach (Link link in myLinks) link.isActive = false;//deactivate old links
        foreach (Link link in activeLinks) link.isActive = true;//activate new links
    }

    private List<Link> GetMyLinks()//get a list of links where this is the origin
    {
        List<Link> myLinks = new List<Link>();
        foreach (Link l in captureLinks.Where(c => c.Origin == this)) myLinks.Add(l);//captures
        foreach (Link l in reinforceLinks.Where(c => c.Origin == this)) myLinks.Add(l);//reinforcements
        return myLinks;
    }
    private void ChangeHive(HiveController.Hive hive)//swap hive to new hive, change color
    {
        if (HiveRef)//remove from current hive
            HiveRef.RemovePlanet(this);
        hiveType = hive;
        if (HiveRef)//check if in hive
        {
            HiveRef.AddPlanet(this);
            _spriteRenderer.color = HiveRef.HiveColor;
        }
        else
        {
            _spriteRenderer.color = ParamManager.Instance.NeutralColor;
        }

    }

    public void HoverObject()
    {
        if (HiveRef)
            _spriteRenderer.color = HiveRef.HiveHighlightColor;
        else
            _spriteRenderer.color = ParamManager.Instance.NeutralHighlightColor;
    }
    public void UnHoverObject()
    {
        if (HiveRef)
            _spriteRenderer.color = HiveRef.HiveColor;
        else
            _spriteRenderer.color = ParamManager.Instance.NeutralColor;
    }
    public void RemoveLink(Link link)//remove link from coresponding list
    {
        if (link.GetType() == typeof(Capture))
            captureLinks.Remove((Capture)link);
        else if (link.GetType() == typeof(Reinforcement))
            reinforceLinks.Remove((Reinforcement)link);
    }
    public void RemoveAllLinks()//destroy all the links originating from this planet
    {
        List<Link> myLinks = GetMyLinks();
        foreach (Link link in myLinks) link.DestroyLink();
    }
    public void RemoveLinkToTarget(Planet target)//destroy capture from this planet to target
    {
        if (HiveType != target.HiveType)//check if target is in another hive
        {
            Capture capture = captureLinks.FirstOrDefault(c => c.Origin == this && c.Target == target);//get the link bertween this and target
            if (capture!=null)//if there is alink destroy it
            {
                capture.DestroyLink();
            }
        }
    }
    public bool IsCapturable(Planet captured)//check if target within capture range, not immune and of another hive
    {
        bool controller = HiveType != captured.HiveType;//check hive
        bool range = (Vector2.Distance(transform.position, captured.transform.position) <= captureRange);//check range
        bool immunity = captured.IsImune;//check immunity
        return controller && range && immunity;
    }
    public enum PlanetSize
    {
        Small=0,
        Medium=1,
        Big=2
    }
}
