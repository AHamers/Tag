using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionNotifier : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        Globals.singleton.multiplayerHandler.notifyCollision(collision);
    }
}
