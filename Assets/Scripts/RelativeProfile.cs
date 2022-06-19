using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RelativeProfile//information on planet and the time of interaction with it
{
    private Planet origin;
    public Planet target { get; private set; }
    public float Score
    {
        get
        {
            float threshold=1;
            switch (Action)
            {
                case ActionType.Capture:
                    threshold = EnemyController.Instance.CaptureThreshold;
                    break;
                case ActionType.Reinforce:
                    threshold = EnemyController.Instance.ReinforceThreshold;
                    break;
                case ActionType.DisconnectCapture:
                    threshold = EnemyController.Instance.DisconnectCaptureThreshold;
                    break;
                case ActionType.DisconnectReinforcement:
                    threshold = EnemyController.Instance.DisconnectReinforceThreshold;
                    break;
            }
            return Score/threshold;
        }
        private set { Score = value; }
    }
    public ActionType Action { get; private set; }
    public bool IsPriority { get; private set; } = false;

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
    public void CalculateScore()//calculate score for relative
    {
        if (origin.HiveType != target.HiveType) {//hostile planet
            if (origin.IsCapturingTarget(target))//already capturing
            {
                Action = ActionType.DisconnectCapture;
                CalculateCaptureDisconnectScore();
            }
            else
            {
                Action = ActionType.Capture;
                CalculateCaptureScore();
            }
        }
        else if (origin.IsReinforcingTarget(target))//already reinforcing
        {
            Action = ActionType.DisconnectReinforcement;
            CalculateReinforceDisconnectScore();
        }
        else
        {
            Action = ActionType.Reinforce;
            CalculateReinforceScore();
        }
    }

    private void CalculateCaptureScore()
    {
        float score = 0;
        float sizeScore = Normalize01(((float)Planet.PlanetSize.Small), ((float)Planet.PlanetSize.Big), ((int)TargetSize));
        score += sizeScore * EnemyController.Instance.PlanetSizeScore;//score for size
        float strengthScore = NormalizeNegativeParabole(-2 * ParamManager.Instance.StrengthCap, 2 * ParamManager.Instance.StrengthCap, StrengthDifference, EnemyController.Instance.StrengthCaptureSkewing);
        score += strengthScore * EnemyController.Instance.StrengthCaptureScore;//score for strength difference
        float incomeScore = NormalizeNegativeParabole(-EnemyController.Instance.IncomeDifferenceMax, EnemyController.Instance.IncomeDifferenceMax, IncomeDifference, EnemyController.Instance.IncomeCaptureSkewing);
        incomeScore = Mathf.Sign(incomeScore) * Mathf.Pow(Mathf.Abs(incomeScore), Mathf.Abs(strengthScore) * EnemyController.Instance.IncomeRelevenceBasedStrength);
        score += incomeScore * EnemyController.Instance.IncomeCaptureScore;//score for income difference
        if (TargetHive == HiveController.Hive.Neutral) score += EnemyController.Instance.NeutralScore;//score for neutral target
        else if (TargetHive == HiveController.Hive.Player) score += EnemyController.Instance.PlayerCaptureScore;//score for player target
        score += Random.Range(-EnemyController.Instance.RandomCaptureScore, EnemyController.Instance.RandomCaptureScore);//add random noise
        //modifier based on relativity ratio 
        score *= Mathf.Lerp(EnemyController.Instance.RelativityMinModifier,
            EnemyController.Instance.RelativityMaxModifier, Mathf.Pow(RelativityRatio, EnemyController.Instance.RelativitySkewing));
        this.Score = score;
        IsPriority = false;
    }
    private void CalculateCaptureDisconnectScore()
    {
        float score = 0;
        float strengthScore = NormalizeNegativeParabole(-2 * ParamManager.Instance.StrengthCap, 2 * ParamManager.Instance.StrengthCap, StrengthDifference, EnemyController.Instance.StrengthCaptureSkewing);
        score -= strengthScore * EnemyController.Instance.StrengthCaptureScore;//score for strength difference
        float incomeScore = NormalizeNegativeParabole(-EnemyController.Instance.IncomeDifferenceMax, EnemyController.Instance.IncomeDifferenceMax, IncomeDifference, EnemyController.Instance.IncomeCaptureSkewing);
        incomeScore = Mathf.Sign(incomeScore) * Mathf.Pow(Mathf.Abs(incomeScore), Mathf.Abs(strengthScore) * EnemyController.Instance.IncomeRelevenceBasedStrength);
        score -= incomeScore * EnemyController.Instance.IncomeCaptureScore;//score for income difference
        this.Score = score;
        score += Random.Range(-EnemyController.Instance.RandomDisconnectScore, EnemyController.Instance.RandomDisconnectScore);//add random noise
        score *= Mathf.Lerp(EnemyController.Instance.RelativityMinModifier,
            EnemyController.Instance.RelativityMaxModifier, 1- Mathf.Pow(RelativityRatio, EnemyController.Instance.RelativitySkewing));//modifier based on relativity ratio
        this.Score = score;
        if (origin.strength <= EnemyController.Instance.LowStrengthPriorityThreshold)
            IsPriority = true;
        else
            IsPriority = false;
    }
    private void CalculateReinforceScore()
    {
        float score = 0;
        float strengthScore = Normalize01Parabole(0, ParamManager.Instance.StrengthCap, origin.strength, EnemyController.Instance.StrengthReinforceSkewing);
        score += strengthScore * EnemyController.Instance.StrengthReinforceScore;//score for origin strength
        float incomeScore = Normalize01Parabole(0, EnemyController.Instance.IncomeReinforceMax, origin.CalculateDeltaStrength(), EnemyController.Instance.IncomeReinforceSkewing);
        score += incomeScore * EnemyController.Instance.IncomeReinforceScore;//score for origin income

        float targetStrengthScore =1- Normalize01Parabole(0, ParamManager.Instance.StrengthCap, target.strength, EnemyController.Instance.StrengthReinforceSkewing);
        score += targetStrengthScore * EnemyController.Instance.StrengthReinforceScore;//score for target strength
        float targetIncomeScore =1- Normalize01Parabole(0, EnemyController.Instance.IncomeReinforceMax, target.CalculateDeltaStrength(), EnemyController.Instance.IncomeReinforceSkewing);
        score += targetIncomeScore * EnemyController.Instance.IncomeReinforceScore;//score for target income
        this.Score = score;
        score += Random.Range(-EnemyController.Instance.RandomReinforceScore, EnemyController.Instance.RandomReinforceScore);//add random noise
        score *= Mathf.Lerp(EnemyController.Instance.RelativityMinModifier,
            EnemyController.Instance.RelativityMaxModifier, Mathf.Pow(RelativityRatio, EnemyController.Instance.RelativitySkewing));//modifier based on relativity ratio
        this.Score = score;
        if (target.strength <= EnemyController.Instance.LowStrengthPriorityThreshold)
            IsPriority = true;
        else
            IsPriority = false;
    }
    private void CalculateReinforceDisconnectScore()
    {
        float score = 0;
        float strengthScore = 1 - Normalize01Parabole(0, ParamManager.Instance.StrengthCap, origin.strength, EnemyController.Instance.StrengthReinforceSkewing);
        score += strengthScore * EnemyController.Instance.StrengthReinforceScore;//score for origin strength
        float incomeScore = 1 - Normalize01Parabole(0, EnemyController.Instance.IncomeReinforceMax, origin.CalculateDeltaStrength(), EnemyController.Instance.IncomeReinforceSkewing);
        score += incomeScore * EnemyController.Instance.IncomeReinforceScore;//score for origin income

        float targetStrengthScore =  Normalize01Parabole(0, ParamManager.Instance.StrengthCap, target.strength, EnemyController.Instance.StrengthReinforceSkewing);
        score += targetStrengthScore * EnemyController.Instance.StrengthReinforceScore;//score for target strength
        float targetIncomeScore = Normalize01Parabole(0, EnemyController.Instance.IncomeReinforceMax, target.CalculateDeltaStrength(), EnemyController.Instance.IncomeReinforceSkewing);
        score += targetIncomeScore * EnemyController.Instance.IncomeReinforceScore;//score for target income
        this.Score = score;
        score += Random.Range(-EnemyController.Instance.RandomDisconnectScore, EnemyController.Instance.RandomDisconnectScore);//add random noise
        score *= Mathf.Lerp(EnemyController.Instance.RelativityMinModifier,
            EnemyController.Instance.RelativityMaxModifier, Mathf.Pow(RelativityRatio, EnemyController.Instance.RelativitySkewing));//modifier based on relativity ratio
        this.Score = score;
        if (origin.strength <= EnemyController.Instance.LowStrengthPriorityThreshold)
            IsPriority = true;
        else
            IsPriority = false;
    }
    

    //help
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

   public enum ActionType
    {
        Capture=0,
        Reinforce=1,
        DisconnectCapture=2,
        DisconnectReinforcement=3
    }
}