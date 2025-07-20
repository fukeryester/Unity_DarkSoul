using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyButton
{
    public bool IsPressing = false;
    public bool OnPressed = false;
    public bool OnReleased = false;

    private bool currentState = false;
    private bool lastState = false;

    public void Tick(bool input)
    {
        currentState = input;

        IsPressing = currentState;

        OnPressed = false;
        OnReleased = false;
        if(currentState != lastState)
        {
            if(currentState == true)
            {
                OnPressed = true;
            }
            else
            {
                OnReleased = true;
            }
        }
        
        lastState = currentState;
    }
}
