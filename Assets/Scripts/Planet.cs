using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;

[ExecuteInEditMode]
public class Planet : MouseInteractable, IOrbitable
{
    private static int IDCount = 0;

    //parameters
    [SerializeField] private HiveController.Hive hiveType = HiveController.Hive.Neutral;
    public HiveController.Hive HiveType { get { return hiveType; } }
    [SerializeField] private bool isQueen = false;
    [SerializeField] private int startingStrength = 6;
    [SerializeField] private PlanetSize planetSize = PlanetSize.Small;
    public PlanetSize Size { get { return planetSize; } }
    private float strengthIncome = 2;
    private int maxActiveLinks = 2;
    private float orbitCycleTime = 5f;
    public float captureRange { get; private set; } = 7;
    private float visibilityRange = 150;

    public int PlanetID { get; private set; } = 0;

    //state variables
    public float strength { get; private set; } = 0;
    private List<Capture> captureLinks = new List<Capture>();
    private List<Reinforcement> reinforceLinks = new List<Reinforcement>();
    private List<Link> activeLinks = new List<Link>();


    private float captureImunity = 0;
    public bool IsImune { get { return captureImunity <= 0; } }


    //references
    private TextMeshPro _strengthDisplay = null;
    private SpriteRenderer _spriteRenderer = null;
    private SpriteRenderer _highlight = null;
    private Animator _highlightAnimator = null;
    private SpriteMask _fogMask=null;
    private Coroutine strenghtGrowthCoroutine;
    public HiveController HiveRef //dynamic HiveControllerReference based on controllingHive
    {
        get
        {
            if (hiveType == HiveController.Hive.Player) return HiveController.Player;
            else if (hiveType == HiveController.Hive.Enemy) return HiveController.Enemy;
            else return null;
        }
        private set { }
    }

    void Start()
    {
        if (Application.isPlaying)
        {
            
            PlanetID = IDCount;//set id
            IDCount++;//increase id count
            _strengthDisplay = Instantiate(ParamManager.Instance.StrengthDisplayPrefab);//create strength display
            _spriteRenderer = GetComponent<SpriteRenderer>();//sprite renderer reference
            _highlight = GetComponentsInChildren<SpriteRenderer>().FirstOrDefault(kid=>kid.CompareTag(ParamManager.Instance.HIGHLIGHTTAG));//highlight reference
            _highlightAnimator= _highlight.GetComponent<Animator>();
            _highlight.enabled = false;
            _fogMask = GetComponentInChildren<SpriteMask>();
            strength = startingStrength;//st starting strength
            strenghtGrowthCoroutine = StartCoroutine(GenerateStrength());//start strength and save reference
            SetStartInHive();
            SetPlanetParametersBySize();
            SetMask();
            RandomizeAnimation();
            if (hiveType == HiveController.Hive.Enemy && FindObjectOfType<TutorialManager>())
                this.enabled = false;
        }
    }

    void Update()
    {
        if (Application.isPlaying)
        {
            UpdateStrengthDisplay();
            if (captureImunity >= 0) captureImunity--;//reduce immunity timer 
        }
        else
        {
            UpdateVisualInEditor();
        }
    }

    private void RandomizeAnimation()
    {
        int version = UnityEngine.Random.Range(0, ParamManager.Instance.planetAnimations.Count-1);
        Animator animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = ParamManager.Instance.planetAnimations.ElementAt(version);//randomize animation version
        animator.speed = UnityEngine.Random.Range(ParamManager.Instance.AnimationMinSpeed, ParamManager.Instance.AnimationMaxSpeed);//randomize speed 
        animator.SetBool(ParamManager.Instance.PlanetReversseAnimationBool, UnityEngine.Random.value>0.5);//randomize direction
    }
    private void SetStartInHive()//add to hive and set color
    {
        if (isQueen && HiveRef)
            HiveRef.Queen = this;
        ChangeHive(hiveType);
    }
    private void SetPlanetParametersBySize()
    {
        ParamManager.PlanetSizeParameters sizeParams = null;
        sizeParams = ParamManager.Instance.planetSizeSet.FirstOrDefault(s => s.size == planetSize);
        strengthIncome = sizeParams.strengthIncome;
        maxActiveLinks = sizeParams.maxActiveLinks;
        this.orbitCycleTime = sizeParams.orbitCycleTime;
        captureRange = sizeParams.captureRange;
        visibilityRange = sizeParams.visibilityRange;
        ReSize(sizeParams);
    }

    private void ReSize(ParamManager.PlanetSizeParameters sizeParams)
    {
        if (transform.parent && transform.parent.CompareTag(ParamManager.Instance.ORBITTAG))
        {
            transform.parent.GetComponent<OrbitalMovement>().SetCycleTime();
        }
        //Transform child = GetComponentsInChildren<Transform>().FirstOrDefault(kid => kid != this.transform);
        if (transform.parent && transform.parent.parent)
        {
            transform.parent.localScale = Vector2.one / transform.parent.parent.localScale.x;
        }
        transform.localScale = Vector2.one * sizeParams.planetScale;
    }

    private void SetMask()//set mask visibility and size
    {
        if (_fogMask)
        {
            if (HiveType == HiveController.Hive.Player || ParamManager.Instance.nonPlayerMaskActive)
                _fogMask.enabled = true;
            else
                _fogMask.enabled = false;
            _fogMask.transform.localScale = new Vector3(visibilityRange, visibilityRange, 1)/transform.localScale.x;
        }
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
            UpdateActiveLinks();
            strength += CalculateDeltaStrength();
            foreach (Capture capture in activeLinks.Where(l => l is Capture))//for each capture interaction planet count contribution of capture
            {
                capture.strengthCaptured += strength > 0 ? ParamManager.Instance.CaptureStrengthOutcome
                    : ParamManager.Instance.ZeroStrengthReducedOutcome;//count less contribution if other has 0 strength
            }
            if (strength >= ParamManager.Instance.StrengthCap)
                strength = ParamManager.Instance.StrengthCap;
            else if (strength <= 0)//if reach zero or below get captured
            {
                strength = 0;
                GetCaptured();
            }
        }
    }
    public float CalculateDeltaStrength()// determines strengrh change per strengt tick
    {
        float deltaStrengt = CalculateStrengthIncome() - CalculateStrengthOutcome();
        return deltaStrengt;
    }
    private float CalculateStrengthIncome()//planet's passive income
    {
        if (HiveRef)//if this is in hive it has income if neutral no income
        {
            float income = strengthIncome;//add this planet base income
            income += reinforceLinks.Count(r => r.IsActive && r.Target == this) * ParamManager.Instance.ReinforceBonus;//add bonus from active reinforcements to this planet
            income -= reinforceLinks.Count(r => r.IsActive && r.Origin == this) * ParamManager.Instance.ReinforceCost;//dedact cost of this planet active reinfrociements
            return income;
            // Mathf.Max(income,0); if we want to make impossible to be negative
        }
        return 0;
    }
    private float CalculateStrengthOutcome()//capture cost for each capture interaction
    {
        float outcome = 0;
        List<Capture> activeConnections = captureLinks.Where(capture => capture.IsActive).ToList();//get active captures this planet is part of
        int zeroPlanets = activeConnections.Where(capture => capture.Target == this && capture.Origin.strength <= 0).Count();//count attacking planets with 0 strength
        outcome += zeroPlanets * ParamManager.Instance.ZeroStrengthReducedOutcome;//planets with 0 strength contribute less to outcome
        int captureLinkCount = 0;//count 2 sided connections wher this is both attacking and attacked by the same planet and both links are active
        foreach (Capture capture in activeConnections)
        {
            foreach (Capture other in activeConnections)
                if (capture.IsReverse(other))
                    captureLinkCount++;
        }
        //outcome is active links - half the 2way connections(they will count twice and we need to count once) - zero planets (they already counted)
        outcome += ParamManager.Instance.CaptureStrengthOutcome * (activeConnections.Count() - captureLinkCount / 2 - zeroPlanets);//planets with strength contribute to outcopme
        return outcome;
    }

    public void AttemptCapture(Planet captured)//start capture of other planet
    {
        //check if this planet in hive and if other planet is not in this hive
        if (HiveType != HiveController.Hive.Neutral && HiveType != captured.HiveType)
        {
            Capture newLink = Capture.NewLink(this, captured);
            if (!captureLinks.Any(capture => capture.CompareExactTo(newLink))) //if not already in capture link list
            {
                captureLinks.Add(newLink);// add capture link to list
                captured.DiscoverLink(newLink);//tell other planet they are under capture
            }
            else
            {
                newLink.DestroyLink();
            }
        }
    }
    private void DiscoverLink(Link newLink)//become under capture by another planet
    {
        if (newLink is Capture)//if a capture link
        {
            if (!captureLinks.Any(c => c.CompareExactTo(newLink))) //if not already in under capture interaction with other
            {
                captureLinks.Add((Capture)newLink);// add capture link to list
            }
        }
        else if (newLink is Reinforcement)//if a reinforce link
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
        if (captureLinks.Where(c => c.IsActive).Any(c => c.GetOther(this).HiveType != HiveController.Hive.Neutral))//if any ACTIVE non neutral capturer in link that this is the target
        {
            //determine hive that captured this planet for the longest among active links
            float playerCaptureDuration = 0;
            float enemyCaptureDuration = 0;
            List<Capture> activeCaptures = captureLinks.Where(c => c.IsActive).ToList();
            foreach (Capture link in activeCaptures)//count active links
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
                foreach (Capture link in activeCaptures)//add to count all not active links
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
                ChangeHive(HiveController.Hive.Player);
            else
                ChangeHive(HiveController.Hive.Enemy);
            FinishCaptureInteraction();
        }
    }
    private void FinishCaptureInteraction()//remove other from interactions list
    {
        Capture[] removeCapture = captureLinks.Where(c => c.Origin == this).ToArray();//make a list of links to remove things this is attacking
        Reinforcement[] removeReinforcement = reinforceLinks.ToArray();//keep list of reinforcement to remove
        Capture[] convert = captureLinks.Where(c => c.Target == this && c.Origin.HiveType == this.HiveType).ToArray ();//make list of links to convert to reinforcement
        foreach (Capture link in removeCapture) { link.DestroyLink(); }//destroy links
        foreach (Reinforcement link in removeReinforcement) { if(link)link.DestroyLink(); }//destroy links
        foreach (Capture capture in convert) { capture.ConvertToReinforcement(); }//convert captures by controlling hive to reinforcements
    }
    public Reinforcement AttemptReinforccing(Planet reinforced)//start reinforcing another planet
    {
        //check if this planet in hive and if other planet is in same hive
        if (hiveType != HiveController.Hive.Neutral && hiveType == reinforced.hiveType)
        {
            Reinforcement newLink = Reinforcement.NewLink(this, reinforced);
            if (!reinforceLinks.Any(reinforce => reinforce.CompareExactTo(newLink))) //if not already in reinforcement link list
            {
                Reinforcement existing = reinforceLinks.FirstOrDefault(reinforce => newLink.IsReverse(reinforce));//find if areverce reinfrocement exists
                if (existing) existing.DestroyLink();//if reverse exists destroy it;
                reinforceLinks.Add(newLink);// add reinforce link to list
                reinforced.DiscoverLink(newLink);//tell other planet they are reinforced
            }
            return newLink;
        }
        return null;
    }
    private void UpdateActiveLinks()//check and update for active links in range choose based on time stemps
    {
        List<Link> myLinks = GetMyLinks();
        myLinks = myLinks.OrderBy(c => c.TimeStemp).ToList();//order by timestemp
        activeLinks.Clear();//clear old active links
        //set new actives equal to max active if capturable or reinforcable
        activeLinks.AddRange(myLinks.Where(l => l is Capture ?
            IsCapturable(l.Target) : IsReinforcable(l.Target)).Take(maxActiveLinks));
        foreach (Link link in myLinks) link.IsActive = false;//deactivate old links
        foreach (Link link in activeLinks) link.IsActive = true;//activate new links
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
            //play capture sound effect
            if (HiveType == HiveController.Hive.Player)
                AudioManager.Instance.OnPlayerCapture();
            else if (HiveType == HiveController.Hive.Enemy)
                AudioManager.Instance.OnEnemyCapture();

        }
        captureImunity = ParamManager.Instance.CaptureImunityTime;
        UpdateColorAndHighlight();
        SetMask();
    }
    public override void UpdateColorAndHighlight()//update color based on hive and highlight
    {
        if (isHovered || isClicked)
        {
            if (HiveRef)
            {
                _spriteRenderer.color = HiveRef.HiveHighlightColor;
                if (HiveType == HiveController.Hive.Player)
                {
                    _highlight.enabled = true;
                    _highlightAnimator.SetBool(ParamManager.Instance.HighlightEnableBool, true);
                }
            }
            else
                _spriteRenderer.color = ParamManager.Instance.NeutralHighlightColor;
        }
        else
        {
            if (HiveRef)
            {
                _spriteRenderer.color = HiveRef.HiveColor;
                if (HiveType == HiveController.Hive.Player)
                {
                    _highlightAnimator.SetBool(ParamManager.Instance.HighlightEnableBool, false);
                }
            }
            else
                _spriteRenderer.color = ParamManager.Instance.NeutralColor;
        }
    }

    public void RemoveLink(Link link)//remove link from coresponding list
    {
        if (link is Capture)
            captureLinks.Remove((Capture)link);
        else if (link is Reinforcement)
            reinforceLinks.Remove((Reinforcement)link);
    }
    public void RemoveAllLinks()//destroy all the links originating from this planet
    {
        List<Link> myLinks = GetMyLinks();
        foreach (Link link in myLinks) link.DestroyLink();
    }
    public void RemoveLinkToTarget(Planet target)//destroy capture from this planet to target
    {
        Debug.Log("Remove Planet");
        if (HiveType != target.HiveType)//check if target is in another hive
        {
            Capture capture = captureLinks.FirstOrDefault(c => c.Origin == this && c.Target == target);//get the link bertween this and target
            if (capture != null)//if there is alink destroy it
            {
                Debug.Log("Destroy");
                capture.DestroyLink();
            }
        }
    }

    private void OnDisable()
    {
        if(_strengthDisplay)
        _strengthDisplay.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        if(_strengthDisplay)
        _strengthDisplay.gameObject.SetActive(true);
    }

    //checks
    public bool IsCapturable(Planet captured)//check if target within capture range, not immune and of another hive
    {
        bool controller = HiveType != captured.HiveType;//check hive
        bool range = (Vector2.Distance(transform.position, captured.transform.position) <= captureRange);//check range
        bool immunity = captured.IsImune;//check immunity
        return controller && range && immunity;
    }
    public bool IsReinforcable(Planet reinforced)//check if target within capture range, and of hive
    {
        bool controller = HiveType == reinforced.HiveType;//check hive
        bool range = (Vector2.Distance(transform.position, reinforced.transform.position) <= captureRange);//check range
        return controller && range;
    }
    public float GetOrbitCycleTime()
    {
        return orbitCycleTime;
    }
    public bool IsCapturingTarget(Planet target)
    {
        
        return captureLinks.Any(link => link.Target == target);
    }
    public bool IsReinforcingTarget(Planet target)
    {
        return reinforceLinks.Any(link => link.Target == target);
    }
    public Link GetLink(Planet target)
    {
        return GetMyLinks().FirstOrDefault(link => link.Target == target);
    }
    public enum PlanetSize
    {
        Small = 0,
        Medium = 1,
        Big = 2
    }
    private void UpdateVisualInEditor()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();//sprite renderer reference
        _fogMask = GetComponentInChildren<SpriteMask>();
        SetPlanetParametersBySize();
        SetMask();
        if (HiveType == HiveController.Hive.Player)
            _spriteRenderer.color = ParamManager.Instance.PlayerColor;
        else if (HiveType == HiveController.Hive.Enemy)
            _spriteRenderer.color = ParamManager.Instance.EnemyColor;
        else if (HiveType == HiveController.Hive.Neutral)
            _spriteRenderer.color = ParamManager.Instance.NeutralColor;
        
    }

}

