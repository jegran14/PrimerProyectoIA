using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State_Example : IState
{
    //aqui podemos segui poninedo avriables privadas
    //e inicializarlas en el constructor de abajo

    public State_Example(/*GameObject ownerGameObject, tagToLookFor*/)
    {
        //constructor
        //poner aqui cosas como buscar una layer
        //this.ownerGameObject = ownerGameObject
        //así lo haces reutilizable para todo tipo de objetos
    }
    public void Enter()
    {
        
    }

    public void Execute()
    {
      //lo que el estado hace cuando lo ejecutas
      //en el caso de patrulla, por ejemplo, aquí estaría 
      //todo el calculo de ruta y mandarlo hacia allí 
    }

    public void Exit()
    {
       
    }
}
