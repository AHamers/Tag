using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Jumper : MonoBehaviour
{
    public GameObject directionVector;
    public float jumpForce;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == Globals.singleton.player)
        {
            other.gameObject.GetComponent<Rigidbody>().velocity += jumpForce * directionVector.transform.up;
            GameObject FX = GameObject.Instantiate(Globals.singleton.BurstFXPrefab);
            FX.transform.position = this.transform.position;
            Globals.singleton.multiplayerHandler.sendMessageToServer("JUMPER|" + this.transform.position.ToString() + "|" + this.directionVector.transform.up);
        }
    }
}
