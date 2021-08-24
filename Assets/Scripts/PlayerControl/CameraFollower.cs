using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    private float lookDirectionVerticalOffset;

    void Start()
    {
        lookDirectionVerticalOffset = 0;
    }

    void FixedUpdate()
    {
        //horizontal camera placement
        updateForwardMarker();

        //vertical camera placement
        lookDirectionVerticalOffset += Input.GetAxis("RightStickVertical") * Globals.singleton.cameraVerticalSpeed;
        Globals.singleton.cameralookDirection.transform.position = Globals.singleton.forwardMarker.transform.position + new Vector3(0, lookDirectionVerticalOffset, 0);

        //placing camera position goal
        Vector3 idealCameraPositionGoal = Globals.singleton.player.transform.position - (Globals.singleton.cameralookDirection.transform.position - Globals.singleton.player.transform.position).normalized * Globals.singleton.cameraDistanceToPlayer
               + new Vector3(0, Globals.singleton.cameraPositionGoalVerticalOffset, 0);

        RaycastHit hitInfo;
        if (!Physics.Raycast(Globals.singleton.player.transform.position, idealCameraPositionGoal - Globals.singleton.player.transform.position, out hitInfo, (idealCameraPositionGoal - Globals.singleton.player.transform.position).magnitude))
        {
            Globals.singleton.cameraPositionGoal.transform.position = idealCameraPositionGoal;
        }
        else
        {
            Globals.singleton.cameraPositionGoal.transform.position = hitInfo.point;
        }

        //updating camera position & rotation
        this.transform.position = this.transform.position + (Globals.singleton.cameraPositionGoal.transform.position - this.transform.position) * Globals.singleton.cameraSpeedToReachGoal;
        this.transform.LookAt(Globals.singleton.cameralookDirection.transform.position);
    }

    private void updateForwardMarker()
    {
        Globals.singleton.forwardMarker.transform.position =
            Globals.singleton.player.transform.position
            + (new Vector3(Mathf.Sin(Globals.singleton.currentForwardAngle), 0, Mathf.Cos(Globals.singleton.currentForwardAngle)) * Globals.singleton.forwardMarkerDistanceMultiplier);
    }
}
