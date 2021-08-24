using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Globalization;
using UnityEngine.SceneManagement;
using TMPro;

public class OtherPlayer
{
	public string name = "";
	public Vector3 position;
	public Quaternion rotation;
	public GameObject gameObject;
	public int playerID;
	public GameObject nameObject;

	public bool hasBeenInitialized = false;
}

public class TagClient : MonoBehaviour
{
	public static List<OtherPlayer> otherPlayers;

	private TcpClient socketConnection = null;
	private Thread clientReceiveThread;
	private List<String> actionsToBeSynced;

	void Start()
	{
		otherPlayers = new List<OtherPlayer>();
		actionsToBeSynced = new List<string>();
		connectToServer();
	}

	void FixedUpdate()
	{
		sendMessageToServer("POS|" + Globals.singleton.player.transform.position.ToString("F4"));
		sendMessageToServer("ROT|"+ Globals.singleton.player.transform.rotation.eulerAngles.ToString("F4"));
		updateOtherPlayers();
		syncActions();

		if (Input.GetButton("Start"))
		{
			disconnectFromServer();
			SceneManager.LoadScene(Globals.singleton.mainMenuScene);
		}
	}

	/*   NAME|<playerID>|<playerName> Received Name information from other client. Must rename it in game. <PlayerName> is 'YOURSELF' if it declares this very player.
		 * POS|<playerID>|<Vec3Pos> Received position information from other client. Must update other player's positions
		 * ROT|<playerID>|<QuaternionRot> Received rotation information from other client. Must update other player's rotations
		 * COLL|<Vec3CollisionPoint> Received collision information from other client. Must display collision FX
		 * TAG|<newTagPlayerID> Received Tag information from server. Must update tag marker anchor
		 * DASH|<Vector3DashPosition>|<Vector3DashDirection> Received Dash information from other client. Must display dash FX
		 * JUMPER|<vec3position>|<vec3direction> Received information that a player has hit a jumper. Must display FX
		 */
	public void parseServerMessage(string message)
    {
		string[] args = message.Split('|');

		switch(args[0])
        {
			//NAME atcs as a declaration of other player
			case "NAME":
                {
					if (args.Length < 3)
						break;

					if (args[2] == "YOURSELF")
					{
						Globals.playerOnlineID = int.Parse(args[1]);
						return;
					}

					//in case other player doesn't exist
					OtherPlayer newPlayer = new OtherPlayer();
					newPlayer.name = args[2];
					newPlayer.playerID = int.Parse(args[1]);
					newPlayer.hasBeenInitialized = false;
					otherPlayers.Add(newPlayer);

					break;
                }
			case "POS":
                {
					if (args.Length < 3)
						break;
					for (int i = 0; i < otherPlayers.Count; i++)
					{
						if (otherPlayers[i].playerID == int.Parse(args[1]))
						{
							otherPlayers[i].position = StringToVector3(args[2], otherPlayers[i].position);
							return;
						}
					}
					break;
                }
			case "ROT":
                {
					if (args.Length < 3)
						break;
					for (int i = 0; i < otherPlayers.Count; i++)
					{
						if (otherPlayers[i].playerID == int.Parse(args[1]))
						{
							otherPlayers[i].rotation = Quaternion.Euler(StringToVector3(args[2], otherPlayers[i].rotation.eulerAngles));
							return;
						}
					}
					break;
				}
			case "COLL":
                {if (args.Length < 2)
							break;
					if (args.Length < 2)
						break;
					actionsToBeSynced.Add(message);
					break;
                }
			case "TAG":
                {
					if (args.Length < 2)
						break;
					actionsToBeSynced.Add(message);
					break;
                }
			case "DASH":
				{
					if (args.Length < 3)
						break;
					actionsToBeSynced.Add(message);
					break;
				}
			case "JUMPER":
				{
					if (args.Length < 2)
						break;
					actionsToBeSynced.Add(message);
					break;
				}
			case "DISC":
                {
					if (args.Length < 2)
						break;
					actionsToBeSynced.Add(message);
					break;
				}
			case "":
                {
					break;
                }
			default:
                {
					Debug.LogWarning("Warning : message not parsed - " + message);
					break;
                }
        }
    }

	private void OnApplicationQuit()
	{
		disconnectFromServer();
	}

	public void notifyCollision(Collision collision)
    {
		for(int i = 0; i < otherPlayers.Count; i++)
        {
			if(otherPlayers[i].gameObject == collision.gameObject && otherPlayers[i].playerID < Globals.playerOnlineID)
            {
				sendMessageToServer("COLL|" + otherPlayers[i].playerID + "|" + ((Globals.singleton.player.transform.position + collision.transform.position) /2).ToString());
            }
        }
    }

	private void updateOtherPlayers()
    {
		for (int i = 0; i < otherPlayers.Count; i++)
		{
			if(!otherPlayers[i].hasBeenInitialized)
            {
				GameObject go = GameObject.Instantiate(Globals.singleton.otherClientsPrefab);
				GameObject name = GameObject.Instantiate(Globals.singleton.othersNamePrefab);
				name.GetComponent<TextMeshPro>().text = "";
				otherPlayers[i].gameObject = go;
				otherPlayers[i].nameObject = name;
				otherPlayers[i].hasBeenInitialized = true;
            }
			if (otherPlayers[i].hasBeenInitialized)
			{
				otherPlayers[i].gameObject.transform.position = otherPlayers[i].position;
				otherPlayers[i].gameObject.transform.rotation = otherPlayers[i].rotation;
				otherPlayers[i].nameObject.GetComponent<TextMeshPro>().text = otherPlayers[i].name;
				otherPlayers[i].nameObject.transform.position = otherPlayers[i].gameObject.transform.position + new Vector3(0, Globals.singleton.nameHeightAbovePlayer, 0);
				otherPlayers[i].nameObject.transform.LookAt(Globals.singleton.mainCamera.transform.position);
			}
		}
    }

	private void syncActions()
    {
		for(int i = 0; i < actionsToBeSynced.Count; i++)
        {
			string[] args = actionsToBeSynced[i].Split('|');

			switch (args[0])
			{
				case "COLL":
					{
						GameObject particleFX = GameObject.Instantiate(Globals.singleton.collisionFXPrefab);
						particleFX.transform.position = StringToVector3(args[1], Vector3.zero);
						Globals.singleton.player.GetComponent<Rigidbody>().AddForce((this.transform.position - particleFX.transform.position).normalized * 1000.0f + new Vector3(0,500,0));
						break;
					}
				case "TAG":
                    {
						int newTagPlayerID = int.Parse(args[1]);
						Globals.singleton.controlStateCurrentMovementSpeed = Globals.singleton.controlStateDefaultMovementSpeed;

						if (Globals.playerOnlineID == newTagPlayerID)
						{
							Globals.singleton.currentTag = Globals.singleton.player;
							Globals.singleton.controlStateCurrentMovementSpeed = Globals.singleton.controlStateTagMovementSpeed;
							break;
						}

						for (int j = 0; j < otherPlayers.Count; j++)
						{
							if (newTagPlayerID == otherPlayers[j].playerID)
							{
								Globals.singleton.currentTag = otherPlayers[j].gameObject;
								break;
							}
						}

						break;
					}
				case "DASH":
					{
						GameObject particleFX = GameObject.Instantiate(Globals.singleton.othersDashFXPrefab);
						particleFX.transform.position = StringToVector3(args[1], Vector3.zero);
						particleFX.transform.forward = StringToVector3(args[2], Vector3.zero);
						break;
					}
				case "JUMPER":
					{
						GameObject particleFX = GameObject.Instantiate(Globals.singleton.othersJumperBurstFXPrefab);
						particleFX.transform.position = StringToVector3(args[1], Vector3.zero);
						particleFX.transform.up = StringToVector3(args[2], Vector3.zero);
						break;
					}
				case "DISC":
                    {
						int playerID = int.Parse(args[1]);
						int otherPlayerIndex = -1;
						for (int j = 0; j < otherPlayers.Count; j++)
                        {
							if (otherPlayers[j].playerID == playerID)
								otherPlayerIndex = j;
                        }
						GameObject.Destroy(otherPlayers[otherPlayerIndex].gameObject);
						GameObject.Destroy(otherPlayers[otherPlayerIndex].nameObject);
						otherPlayers.RemoveAt(otherPlayerIndex);
						break;
                    }
				default:
					{
						break;
					}
			}
		}

		actionsToBeSynced.Clear();
	}

	private void connectToServer()
    {
		socketConnection = new TcpClient();
		try
		{
			socketConnection.Connect(Globals.serverIP, 8052);
		}
		catch
        {
			SceneManager.LoadScene(Globals.singleton.mainMenuScene);
        }

		clientReceiveThread = new Thread(new ThreadStart(listenForMessages));
		clientReceiveThread.IsBackground = true;
		clientReceiveThread.Start();

		sendMessageToServer("NAME|" + Globals.clientName);
	}

	private void disconnectFromServer()
    {
		sendMessageToServer("DISC");
		socketConnection.Close();
		clientReceiveThread.Abort();
	}

	private void listenForMessages()
	{
		try
		{
			while (true)
			{
				Byte[] bytes = new Byte[1024];
				using (NetworkStream stream = socketConnection.GetStream())
				{
					int length;
					// Read incomming stream into byte arrary. 						
					while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
					{
						Byte[] incomingData = new byte[length];
						Array.Copy(bytes, 0, incomingData, 0, length);
						// Convert byte array to string message. 							
						string clientMessage = Encoding.ASCII.GetString(incomingData);

						string[] separatedMessages = clientMessage.Split('@');
						for (int i = 0; i < separatedMessages.Length; i++)
						{
							parseServerMessage(separatedMessages[i]);
						}
						stream.Flush();
					}
				}
			}
		}
		catch (SocketException socketException)
		{
			Debug.Log("Socket exception: " + socketException);
		}
	}

	public static Vector3 StringToVector3(string sVector, Vector3 defaultVector)
	{
		// Remove the parentheses
		if (sVector.StartsWith("(") && sVector.EndsWith(")"))
		{
			sVector = sVector.Substring(1, sVector.Length - 2);
		}

		// split the items
		string[] sArray = sVector.Split(',');

		try
		{

			// store as a Vector3
			Vector3 result = new Vector3(
				float.Parse(sArray[0], CultureInfo.InvariantCulture),
				float.Parse(sArray[1], CultureInfo.InvariantCulture),
				float.Parse(sArray[2], CultureInfo.InvariantCulture));

			return result;
		}
		catch
        {
			return defaultVector;
        }
	}

	public void sendMessageToServer(string message)
	{
		message = '@' + message;
		if (socketConnection == null || !socketConnection.Connected)
		{
			return;
		}
		try
		{
			// Get a stream object for writing. 			
			NetworkStream stream = socketConnection.GetStream();
			if (stream.CanWrite)
			{
				// Convert string message to byte array.                 
				byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(message);
				// Write byte array to socketConnection stream.                 
				stream.Flush();
				stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
			}
		}
		catch (SocketException socketException)
		{
			Debug.Log("Socket exception: " + socketException);
		}
	}
}