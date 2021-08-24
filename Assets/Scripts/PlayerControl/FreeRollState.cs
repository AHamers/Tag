using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using XInputDotNetPure;

public class FreeRollState : State
{
    public bool canDash = true;
    public override State CheckStateTransition()
    {
        if (!Input.GetButton("LeftJoystickButton"))
        {
            return new ControlState();
        }

        return null;
    }

    public override void OnEnterState()
    {
        GameObject player = Globals.singleton.player;
        player.GetComponent<Rigidbody>().drag = Globals.singleton.freerollStateDrag;
    }

    public override void OnExitState()
    {
    }

    public override void OnUpdate()
    {
        if (Input.GetButton("A"))
        {
            if (canDash)
            {
                Rigidbody player = Globals.singleton.player.GetComponent<Rigidbody>();
                Globals.singleton.multiplayerHandler.sendMessageToServer("DASH|" + player.transform.position + '|' + -1* Globals.singleton.forward);
                GameObject FX = GameObject.Instantiate(Globals.singleton.dashFXPrefab);
                FX.transform.forward = -1 * Globals.singleton.forward;
                FX.transform.position = Globals.singleton.player.transform.position;
                player.GetComponent<Rigidbody>().velocity = Globals.singleton.freerollStateDashForce * Globals.singleton.forward;
                Thread dashCooldown = new Thread(startDashCooldown);
                dashCooldown.Start();
            }
        }

        Globals.singleton.currentForwardAngle += Input.GetAxis("RightStickHorizontal") * Globals.singleton.rotationSpeed;            
    }

    void startDashCooldown()
    {
        canDash = false;
        Thread.Sleep(Globals.singleton.freerollStateDashCooldown);
        canDash = true; Thread hapticsThread = new Thread(() => Globals.VibrateController(200, Globals.singleton.controllerDefaultHapticsForce));
        hapticsThread.Start();
    }
}
