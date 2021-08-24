using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagFollow : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        Globals.singleton.currentTag = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (Globals.singleton.currentTag != null)
        { 
            this.transform.position = Globals.singleton.currentTag.transform.position + new Vector3(0, Globals.singleton.flagHeightAboveTag, 0);
        }
    }
}
