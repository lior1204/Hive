using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Capture : Link
{
    public int strengthCaptured;

    public Capture(Planet attacker, Planet captured) : base(attacker, captured)
    {
        strengthCaptured = 0;
    }
    public void ConvertToReinforcement()//remove this link from members and create new reinforcement link instead
    {
        Origin.AttemptReinforccing(Target);
        DestroyLink();
    }

}
