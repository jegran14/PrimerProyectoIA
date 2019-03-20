using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Controlador IA del guardia
[RequireComponent(typeof(StateMachine))]
public class CopStateManager : AIController
{
    /// <summary>
    /// Mover el personaje a un punto concreto
    /// </summary>
    /// <param name="point">Punto al que se debe mover el personaje</param>
    public override void MoveTo(Vector3 point)
    {
        //Move character
    }
}
