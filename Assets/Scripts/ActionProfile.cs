using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ActionProfile//information on planet and the time of interaction with it
{
    public Planet origin { get; private set; }
    public Planet target { get; private set; }
    private float score = 0;
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
            return score/threshold;
        }
        private set { score = value; }
    }
    public ActionType Action { get; private set; }
    public bool IsPriority { get; private set; } = false;

    public float timeTogather = 0;
    public float timeApart = 0;
    public ActionProfile(Planet origin, Planet target)
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
        if (timeApart + timeTogather < EnemyController.Instance.MinimumProfileTime)
        {
            score = 0;
            return;
        }
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
        //if (origin.GetInstanceID() == -1196 && Action == ActionType.Capture)
        //    Debug.Log("Target:" + target.GetInstanceID() +" Origin: "+origin.GetInstanceID()+ " Action: " + Action);
        float score = 0;
        float val = 0;
        float sizeScore = Normalize01(((float)Planet.PlanetSize.Small), ((float)Planet.PlanetSize.Big), ((int)TargetSize));
        val= sizeScore * EnemyController.Instance.PlanetSizeScore;//score for size
        //if (origin.GetInstanceID() == -1196 && Action == ActionType.Capture)
        //Debug.Log("Size Score: " + val);
        score += val;
        float strengthScore = NormalizeNegativeSigmoid(-2 * ParamManager.Instance.StrengthCap, 2 * ParamManager.Instance.StrengthCap, StrengthDifference);//, EnemyController.Instance.StrengthCaptureSkewing);
        val= strengthScore * EnemyController.Instance.StrengthCaptureScore;//score for strength difference
        //if (origin.GetInstanceID() == -1196 && Action == ActionType.Capture)
        //Debug.Log("Strength Score: " + val);
        score += val;
        float incomeScore = NormalizeNegativeSigmoid(-EnemyController.Instance.IncomeDifferenceMax, EnemyController.Instance.IncomeDifferenceMax, IncomeDifference);//, EnemyController.Instance.IncomeCaptureSkewing);
        incomeScore = Mathf.Sign(incomeScore) * Mathf.Pow(Mathf.Abs(incomeScore), Mathf.Abs(strengthScore) * EnemyController.Instance.IncomeRelevenceBasedStrength);
        val= incomeScore * EnemyController.Instance.IncomeCaptureScore;//score for income difference
        //if (origin.GetInstanceID() == -1196 && Action == ActionType.Capture) 
        //Debug.Log("Income Score: " + val);
        score += val;
        val = 0;
        if (TargetHive == HiveController.Hive.Neutral) val= EnemyController.Instance.NeutralScore;//score for neutral target
        else if (TargetHive == HiveController.Hive.Player) val= EnemyController.Instance.PlayerCaptureScore;//score for player target
        //if (origin.GetInstanceID() == -1196 && Action == ActionType.Capture) 
        //Debug.Log("Target Hive Score: " + val);
        score += val;
        val= Random.Range(-EnemyController.Instance.RandomCaptureScore, EnemyController.Instance.RandomCaptureScore);//add random noise
        //if (origin.GetInstanceID() == -1196 && Action == ActionType.Capture) 
        //Debug.Log("Random Score: " + val);
        score += val;
        //modifier based on relativity ratio 
        val= Mathf.Lerp(EnemyController.Instance.RelativityMinModifier,
            EnemyController.Instance.RelativityMaxModifier, Mathf.Pow(RelativityRatio, EnemyController.Instance.RelativitySkewing));
        //if (origin.GetInstanceID() == -1196 && Action == ActionType.Capture)
        //Debug.Log("Relativity Ratio: " + RelativityRatio + " Skew: " + EnemyController.Instance.RelativitySkewing);
        //Debug.Log("Lerp between:" + EnemyController.Instance.RelativityMinModifier + ", " + EnemyController.Instance.RelativityMaxModifier
        //    + " Value:" + Mathf.Pow(RelativityRatio, EnemyController.Instance.RelativitySkewing));
        //Debug.Log("Relativity Modifier: " + val);
        score *= val;
        this.Score = score;
        IsPriority = false;
        //if (origin.GetInstanceID() == -1196 && Action == ActionType.Capture) 
        //Debug.Log(" Score: " + this.score + " score/threshhold: " + Score);
    }
    private void CalculateCaptureDisconnectScore()
    {
        //Debug.Log("Target:" + target.GetInstanceID() + " Origin: " + origin.GetInstanceID() + " Action: " + Action);
        float score = 0;
        float val = 0;
        float strengthScore = NormalizeNegativeSigmoid(-2 * ParamManager.Instance.StrengthCap, 2 * ParamManager.Instance.StrengthCap, StrengthDifference);//, EnemyController.Instance.StrengthDisconnectSkewing);
        val = -strengthScore * EnemyController.Instance.StrengthDisconnectScore;//score for strength difference
        //Debug.Log("Normalize(Min: " + -2 * ParamManager.Instance.StrengthCap + ",Max: " + 2 * ParamManager.Instance.StrengthCap + ",Val: " + StrengthDifference+")");
        //Debug.Log("Normal: "+ NormalizeNegative(-2 * ParamManager.Instance.StrengthCap, 2 * ParamManager.Instance.StrengthCap, StrengthDifference) + " Value: " + -strengthScore + ", Multiplier: " + EnemyController.Instance.StrengthDisconnectScore);
        //Debug.Log("Strength Score: " + val);
        score += val;
        float incomeScore = NormalizeNegativeSigmoid(-EnemyController.Instance.IncomeDisconnectMax, EnemyController.Instance.IncomeDisconnectMax, IncomeDifference);//, EnemyController.Instance.IncomeDisconnectSkewing);
        incomeScore = Mathf.Sign(incomeScore) * Mathf.Pow(Mathf.Abs(incomeScore), Mathf.Abs(strengthScore) * EnemyController.Instance.IncomeRelevenceBasedStrengthDisconnect);
        val= -incomeScore * EnemyController.Instance.IncomeDisconnectScore;//score for income difference
        //Debug.Log("Income Score: " + val);
        score += val;
        val= Random.Range(-EnemyController.Instance.RandomDisconnectScore, EnemyController.Instance.RandomDisconnectScore);//add random noise
        //Debug.Log("Random Score: " + val);
        score += val;
        val= Mathf.Lerp(EnemyController.Instance.RelativityMinModifier,
            EnemyController.Instance.RelativityMaxModifier, 1- Mathf.Pow(RelativityRatio, EnemyController.Instance.RelativitySkewing));//modifier based on relativity ratio
        //Debug.Log("Relativity Modifier: " + val);
        score *= val;
        this.Score = score;
        if (origin.strength <= EnemyController.Instance.LowStrengthPriorityThreshold)
            IsPriority = true;
        else
            IsPriority = false;
        //Debug.Log(" Score: " + this.score + " score/threshhold: " + Score);
    }
    private void CalculateReinforceScore()
    {
        //Debug.Log("Target:" + target.GetInstanceID() + " Origin: " + origin.GetInstanceID() + " Action: " + Action);
        if (origin.strength > EnemyController.Instance.ReinforceMinStrength && target.strength < EnemyController.Instance.GetHelpMaxStrength)//thresholds for offering reinforcement
        {
            float score = 0;
            float val = 0;
            float strengthScore = Normalize01Sigmoid(0, ParamManager.Instance.StrengthCap, origin.strength);
            val= strengthScore * EnemyController.Instance.StrengthReinforceScore;//score for origin strength
            //Debug.Log("Origin Strength Score: " + val);
            score += val;
            float incomeScore = NormalizeNegativeSigmoid(-EnemyController.Instance.IncomeReinforceMax, EnemyController.Instance.IncomeReinforceMax, origin.CalculateDeltaStrength());
            val= incomeScore * EnemyController.Instance.IncomeReinforceScore;//score for origin income
            //Debug.Log("Origin Income Score: " + val);
            score += val;
            float targetStrengthScore = 1 - Normalize01Sigmoid(0, ParamManager.Instance.StrengthCap, target.strength);
            val= targetStrengthScore * EnemyController.Instance.StrengthTargetReinforceScore;//score for target strength
            //Debug.Log("Target Strength Score: " + val);
            score += val;
            float targetIncomeScore = 1 - NormalizeNegativeSigmoid(-EnemyController.Instance.IncomeReinforceMax, EnemyController.Instance.IncomeReinforceMax, target.CalculateDeltaStrength());
            val= targetIncomeScore * EnemyController.Instance.IncomeTargetReinforceScore;//score for target income
            //Debug.Log("Target Income Score: " + val);
            score += val;
            val= Random.Range(-EnemyController.Instance.RandomReinforceScore, EnemyController.Instance.RandomReinforceScore);//add random noise
            //Debug.Log("Random Score: " + val);
            score += val;
            val= Mathf.Lerp(EnemyController.Instance.RelativityMinModifier,
                EnemyController.Instance.RelativityMaxModifier, Mathf.Pow(RelativityRatio, EnemyController.Instance.RelativitySkewing));//modifier based on relativity ratio
            //Debug.Log("Relativity Modifier Score: " + val);
            score *= val;
            this.Score = score;
            if (target.strength <= EnemyController.Instance.LowStrengthPriorityThreshold)
                IsPriority = true;
            else
                IsPriority = false;
        }
        else
        {
            this.score = 0;
            IsPriority = false;
        }
        //Debug.Log(" Score: " + this.score + " score/threshhold: " + Score);
    }
    private void CalculateReinforceDisconnectScore()
    {
        //Debug.Log("Target:" + target.GetInstanceID() + " Origin: " + origin.GetInstanceID() + " Action: " + Action);
        float score = 0;
        float val = 0;
        float strengthScore = 1 - Normalize01Sigmoid(0, ParamManager.Instance.StrengthCap, origin.strength);
        val= strengthScore * EnemyController.Instance.StrengthReinforceScore;//score for origin strength
        //Debug.Log("Origin Strength Score: " + val);
        score += val;
        float incomeScore = Mathf.Pow( NormalizeNegative(-EnemyController.Instance.IncomeReinforceMax, EnemyController.Instance.IncomeReinforceMax, origin.CalculateDeltaStrength()),3);
        val= incomeScore * EnemyController.Instance.IncomeReinforceScore;//score for origin income
        //Debug.Log("Normalize(Min: " + -EnemyController.Instance.IncomeReinforceMax + ",Max: " + EnemyController.Instance.IncomeReinforceMax + ",Val: " + origin.CalculateDeltaStrength() + ")");
        //Debug.Log("Normal: " + NormalizeNegative(-EnemyController.Instance.IncomeReinforceMax, EnemyController.Instance.IncomeReinforceMax, origin.CalculateDeltaStrength()) + " Value: " + incomeScore + ", Multiplier: " + EnemyController.Instance.IncomeReinforceScore);
        //Debug.Log("Origin Income Score: " + val);
        score += val;
        float targetStrengthScore =  Normalize01Sigmoid(0, ParamManager.Instance.StrengthCap, target.strength);
        val= targetStrengthScore * EnemyController.Instance.StrengthReinforceScore;//score for target strength
        //Debug.Log("Target Strength Score: " + val);
        score += val;
        float targetIncomeScore = NormalizeNegativeSigmoid(-EnemyController.Instance.IncomeReinforceMax, EnemyController.Instance.IncomeReinforceMax, target.CalculateDeltaStrength());
        val= targetIncomeScore * EnemyController.Instance.IncomeReinforceScore;//score for target income
        //Debug.Log("Target Income Score: " + val);
        score += val;
        val = Random.Range(-EnemyController.Instance.RandomDisconnectScore, EnemyController.Instance.RandomDisconnectScore);//add random noise
        //Debug.Log("Random Score: " + val);
        score += val;
        val = Mathf.Lerp(EnemyController.Instance.RelativityMinModifier,
            EnemyController.Instance.RelativityMaxModifier, Mathf.Pow(RelativityRatio, EnemyController.Instance.RelativitySkewing));//modifier based on relativity ratio
        //Debug.Log("Relativity Modifier: " + val);
        score *= val;
        this.Score = score;
        if (origin.strength <= EnemyController.Instance.LowStrengthPriorityThreshold)
            IsPriority = true;
        else
            IsPriority = false;
        //Debug.Log(" Score: " + this.score + " score/threshhold: " + Score);
    }


    //help
    private static float NormalizeNegative(float min,float max, float value)// normalize number between -1 to 1
    {
        if (min == max)
        {
            return 0;
        }
        if (value <= min)
        {
            return -1;
        }
        if (value >= max)
        {
            return 1;
        }
            float normal = ((value - min) / (max - min)) * 2 - 1;
        return normal;
    }
    private static float Normalize01(float min,float max, float value)// normalize number between 0 to 1
    {
        if (min == max)
        {
            return 0;
        }
        if (value <= min)
        {
            return 0;
        }
        if (value >= max)
        {
            return 1;
        }
        float normal = ((value - min) / (max - min));
        return normal;
    }
    private static float NormalizeNegativeSigmoid(float min,float max, float value)// normalize number between -1 to 1 in a parabole
    {
        float normal = NormalizeNegative(min, max, value);
        normal = 2 / (1 + Mathf.Pow(40, -normal))-1f;
        return normal;
    }
    private static float Normalize01Sigmoid(float min,float max, float value)// normalize number between -1 to 1 in a parabole
    {
        float normal = Normalize01(min, max, value);
        
            normal = 1 / (1 + Mathf.Pow(4, (-5.5f * normal + 3)));
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