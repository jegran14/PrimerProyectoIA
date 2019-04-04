using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Clase encargada de almacenar la información de las trasiciones entre estados.
[System.Serializable]
public class Transition
{
    public Decision decision; //Decisiones a comprobar para llevar a cabo la transicion
    public State trueState; //Estado al que transicionar si la decision es verdadera
    public State falseState; //Estado al que transiocionar si la decision es falsa

    //Acciones que realizar si la transicion es verdadera o falsa, las acciones pueden estar vacias si no se quiere hacer nada
    public Action[] trueActions;
    public Action[] falseActions;
}
