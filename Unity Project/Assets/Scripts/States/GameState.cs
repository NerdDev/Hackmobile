using System.Collections;
using UnityEngine;

//THIS CLASS IS ONLY HERE SO CUSTOM GAME STATES CAN INHERIT FROM IT!!!!!  DO NOT DIRECTLY IMPLEMENT!!!!

public abstract class GameState : MonoBehaviour
{
    public abstract void OnActivate();
    public abstract void OnDeactivate();
    public abstract void OnUpdate();
}