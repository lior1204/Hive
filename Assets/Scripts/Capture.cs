﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Capture : Link
{
    public int strengthCaptured;

    public Capture(Planet attacker, Planet captured) : base(attacker,captured)
    {
        strengthCaptured = 0;
    }
}
