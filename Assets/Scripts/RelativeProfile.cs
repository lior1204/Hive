using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RelativeProfile//information on planet and the time of interaction with it
{
    private Planet origin;
    public Planet target { get; private set; }
    public float timeTogather { get { return timeTogather; } set { timeTogather += value > 0 ? value : 0; } }
    public float timeApart { get { return timeApart; } set { timeApart += value > 0 ? value : 0; } }
    public RelativeProfile(Planet origin, Planet target)
    {
        this.origin = origin;
        this.target = target;
        timeTogather = 0;
        timeApart = 0;
    }
    public float RelativityRatio { get { return timeTogather / (timeTogather + timeApart); } }//how much to prioritize this planet
    public float StrengthDifference { get { return origin.strength - target.strength; } }//the stength difference between the planets
    public HiveController.Hive TargetHive { get { return target.HiveType; } }//hive of target
    public Planet.PlanetSize TargetSize { get { return target.Size; } }//size of target
    public float IncomeDifference { get { return origin.CalculateDeltaStrength() - target.CalculateDeltaStrength(); } }//the income difference between the planets
    public float CalculateScore()//calculate score for relative
    {
        float score = 0;
        float val;
        val = Normalize01(((float)Planet.PlanetSize.Small), ((float)Planet.PlanetSize.Big),((int)TargetSize));
        score += val * ParamManager.Instance.PlanetSizeScore;//score for size
        score += StrengthDifference * ParamManager.Instance.StrengthScore;//score for strength difference
        score += IncomeDifference * ParamManager.Instance.IncomeScore;//score for income difference
        if (TargetHive == HiveController.Hive.Neutral) score += ParamManager.Instance.NeutralScore;//score for neutral target
        else if (TargetHive == HiveController.Hive.Player) score += ParamManager.Instance.PlayerScore;//score for player target
        score += Random.Range(-ParamManager.Instance.RandomScore, ParamManager.Instance.RandomScore);//add random noise
        score *= Mathf.Lerp(ParamManager.Instance.RelativityScoreModifier.x,
            ParamManager.Instance.RelativityScoreModifier.y, Mathf.Pow(RelativityRatio, 1.5f));//modifier based on relativity ratio
        return score;
    }

    public static float NormalizeNegative(float min,float max, float value)// normalize number between -1 to 1
    {
        if (min == max) return 0;
        if (value <= min) return -1;
        if (value >= min) return 1;
        float normal = ((value - min) / (max - min)) * 2 - 1;
        return normal;
    }
    public static float Normalize01(float min,float max, float value)// normalize number between 0 to 1
    {
        if (min == max) return 0;
        if (value <= min) return -1;
        if (value >= min) return 1;
        float normal = ((value - min) / (max - min));
        return normal;
    }
    public static float NormalizeNegativeParabole(float min,float max, float value,float curve)// normalize number between -1 to 1 in a parabole
    {

        float normal = NormalizeNegative(min, max, value);
        if(curve>0)
            normal = Mathf.Sign(normal) * Mathf.Abs(Mathf.Pow(normal, curve));
        return normal;
    }
    public static float Normalize01Parabole(float min,float max, float value,float curve)// normalize number between -1 to 1 in a parabole
    {

        float normal = Normalize01(min, max, value);
        if(curve>0)
            normal = Mathf.Pow(normal, curve);
        return normal;
    }
}