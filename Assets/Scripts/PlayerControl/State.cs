using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    public abstract void OnUpdate();
    public abstract void OnEnterState();
    public abstract void OnExitState();
    public abstract State CheckStateTransition();
}
