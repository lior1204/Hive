using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RelativeProfile//information on planet and the time of interaction with it
{
    private Planet origin;
    public Planet target { get; private set; }
    public float score { get; private set; } = 0;
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
        score += val * EnemyController.Instance.PlanetSizeScore;//score for size
        float strengthScore = NormalizeNegativeParabole(-2 * ParamManager.Instance.StrengthCap, 2 * ParamManager.Instance.StrengthCap, StrengthDifference, EnemyController.Instance.StrengthSkewing);
        score += StrengthDifference * EnemyController.Instance.StrengthScore;//score for strength difference
        val = NormalizeNegativeParabole(-EnemyController.Instance.IncomeDifferencMax, EnemyController.Instance.IncomeDifferencMax, IncomeDifference, EnemyController.Instance.IncomeSkewing);
        val = Mathf.Sign(val)* Mathf.Pow(Mathf.Abs(val), Mathf.Abs(strengthScore) * EnemyController.Instance.IncomeRelevenceBasedStrength);
        score += val * EnemyController.Instance.IncomeScore;//score for income difference
        if (TargetHive == HiveController.Hive.Neutral) score += EnemyController.Instance.NeutralScore;//score for neutral target
        else if (TargetHive == HiveController.Hive.Player) score += EnemyController.Instance.PlayerScore;//score for player target
        score += Random.Range(-EnemyController.Instance.RandomScore, EnemyController.Instance.RandomScore);//add random noise
        score *= Mathf.Lerp(EnemyController.Instance.RelativityScoreModifier.x,
            EnemyController.Instance.RelativityScoreModifier.y, Mathf.Pow(RelativityRatio, 1.5f));//modifier based on relativity ratio
        this.score = score;
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