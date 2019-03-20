using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Clase principal de las acciones que pueden realizar los personajes, Todas las acciones heredan de esta clase
public abstract class Action : ScriptableObject
{
    public abstract void Act(AIController controller);
}
