using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputAutomaton : MonoBehaviour
{
    private State currentState;

    // Start is called before the first frame update
    void Start()
    {
        currentState = new ControlState();
        currentState.OnEnterState();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        currentState.OnUpdate();

        State transitionningTo = currentState.CheckStateTransition();
        if (transitionningTo != null)
        {
            currentState.OnExitState();
            currentState = transitionningTo;
            currentState.OnEnterState();
        }
    }
}
