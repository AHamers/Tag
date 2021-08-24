using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;
using System.Threading;
using UnityEngine.SceneManagement;

public class Globals : MonoBehaviour
{
    [Header("Camera")] 
    public GameObject cameraPositionGoal;
    public GameObject cameralookDirection;
    public GameObject mainCamera;

    public float cameraDistanceToPlayer;
    public float cameraPositionGoalVerticalOffset;
    public float cameraSpeedToReachGoal;
    public float cameraVerticalSpeed;

    [Header("Control State")]
    public  float controlStateDefaultMovementSpeed;
    public  float controlStateTagMovementSpeed;
    public  float controlStateCurrentMovementSpeed;
    public  float controlStateDashForce;
    public  float controlStateDrag;
    public  int controlStateDashCooldown;

    [Header("Freeroll State")]
    public  float freerollStateDashForce;
    public  float freerollStateDrag;
    public  int freerollStateDashCooldown;

    [Header("Player")]
    public GameObject player;
    public GameObject forwardMarker;
    public float forwardMarkerDistanceMultiplier;
    public float rotationSpeed;
    public float controllerDefaultHapticsForce;
    public Vector3 forward { get { return (Globals.singleton.forwardMarker.transform.position - Globals.singleton.player.transform.position).normalized; } }
    public Vector3 rightward { get { return PerpendicularClockwise((Globals.singleton.forwardMarker.transform.position - Globals.singleton.player.transform.position).normalized); } }
    public float currentForwardAngle;

    [Header("Multiplayer")]
    public TagClient multiplayerHandler;
    public GameObject currentTag;
    public float flagHeightAboveTag;
    public static string clientName;
    public static string serverIP;
    public static int playerOnlineID;
    public GameObject tagFlag;
    public float nameHeightAbovePlayer;

    [Header("Prefabs")]
    public GameObject dashFXPrefab;
    public GameObject BurstFXPrefab;
    public GameObject collisionFXPrefab;
    public GameObject otherClientsPrefab;
    public GameObject othersDashFXPrefab;
    public GameObject othersJumperBurstFXPrefab;
    public GameObject othersNamePrefab;

    [Header("Menu")]
    public string mainMenuScene;

    public static Globals singleton;

    // Start is called before the first frame update
    void Awake()
    {
        Globals.singleton = this;
    }

    public static Vector3 PerpendicularClockwise(Vector3 vector)
    {
        return new Vector3(vector.z, vector.y, -vector.x);
    }

    public static void VibrateController(int timeMs, float force)
    {
        GamePad.SetVibration(PlayerIndex.One, Globals.singleton.controllerDefaultHapticsForce, force);
        Thread.Sleep(timeMs);
        GamePad.SetVibration(PlayerIndex.One, 0, 0);
        return;
    }
}
