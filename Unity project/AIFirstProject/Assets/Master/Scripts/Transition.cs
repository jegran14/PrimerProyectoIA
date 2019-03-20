using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Clase encargada de almacenar la información de las trasiciones entre estados.
[System.Serializable]
public class Transition
{
    public Decision decision;
    public State trueState;
    public State falseState;
}
