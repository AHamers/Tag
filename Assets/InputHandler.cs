using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputHandler : MonoBehaviour
{
    private string serverAddress = "";
    private string nickname = "";
    public string gameScene;
    
    public void onConnectButtonClicked()
    {
        Globals.clientName = nickname;
        Globals.serverIP = serverAddress;
        SceneManager.LoadScene(gameScene);
    }
    public void onQuitButtonClicked()
    {
        Application.Quit();
    }

    public void onServerAddressChanged(string serverAddress)
    {
        this.serverAddress = serverAddress;
    }
    public void onNuckNameChanged(string nickname)
    {
        this.nickname = nickname;
    }
}
