using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeDecision : Decision
{
    public override bool Decide(AIController controller)
    {
        bool isInTime = CompareTime(controller);
        return isInTime;
    }

    private bool CompareTime(AIController controller)
    {
        bool isInTime = false;
        return isInTime;
    }
}
