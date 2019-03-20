using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Clase padre para todas las decisiones.
public abstract class Decision : ScriptableObject
{
    /// <summary>
    /// Test if the decision is being met or not
    /// </summary>
    /// <param name="controller"></param>
    /// <returns></returns>
    public abstract bool Decide(AIController controller);
}
