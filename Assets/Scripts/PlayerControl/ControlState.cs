using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class ControlState : State
{
    public bool canDash = true;
    public override State CheckStateTransition()
    {
        if(Input.GetButton("LeftJoystickButton"))
        {
            return new FreeRollState();
        }

        return null;
    }

    public override void OnEnterState()
    {
        Globals.singleton.player.gameObject.GetComponent<Rigidbody>().drag = Globals.singleton.controlStateDrag;
    }

    public override void OnExitState()
    {
    }

    public override void OnUpdate()
    {
        Rigidbody rb = Globals.singleton.player.GetComponent<Rigidbody>();

        //hangle movement
        rb.AddForce(Globals.singleton.controlStateCurrentMovementSpeed * Globals.singleton.forward * Input.GetAxis("LeftStickVertical"));
        rb.AddForce(Globals.singleton.controlStateCurrentMovementSpeed * Globals.singleton.rightward * Input.GetAxis("LeftStickHorizontal"));

        Globals.singleton.currentForwardAngle += Input.GetAxis("RightStickHorizontal") * Globals.singleton.rotationSpeed;

        if (Input.GetButton("A"))
        {
            if (canDash)
            {
                Globals.singleton.multiplayerHandler.sendMessageToServer("DASH|" + rb.transform.position + '|' + -1* Globals.singleton.forward);
                GameObject FX = GameObject.Instantiate(Globals.singleton.dashFXPrefab);
                FX.transform.position = Globals.singleton.player.transform.position;
                FX.transform.forward = -1 * Globals.singleton.forward;
                rb.velocity = Globals.singleton.controlStateDashForce * Globals.singleton.forward;
                Thread dashCooldown = new Thread(startDashCooldown);
                dashCooldown.Start();
            }
        }
    }

    void startDashCooldown()
    {
        canDash = false;
        Thread.Sleep(Globals.singleton.controlStateDashCooldown);
        canDash = true;
        Thread hapticsThread = new Thread(() => Globals.VibrateController(200, Globals.singleton.controllerDefaultHapticsForce));
        hapticsThread.Start();
    }

}
