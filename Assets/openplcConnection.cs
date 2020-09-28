using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class openplcConnection : MonoBehaviour
{
    public GameObject openplc_connection;
    public InputField openplc_ip;
    public InputField openplc_port;
    public Button connect_to_plc;
    public Button disconnect_from_plc;

    public static string ip = "";
    public static string port = "";
    public static bool openplc_connect = false;

    void Start()
    {
        // Registering listeners to input fields
        openplc_ip.onEndEdit.AddListener(GetIp);
        openplc_port.onEndEdit.AddListener(GetPort);

    }

    private void GetIp(string arg0)
    {
        ip = openplc_ip.text;
        //print(ip);
    }

    private void GetPort(string arg1) 
    {
        port = openplc_port.text;
        //print(port);
    }

    public void OnConnectClick()
    {
        //when the button connect_to_plc is clicked
        openplc_connect = true;
    }

    public void OnDisconnectClick()
    {
        //when the button disconnect_from_plc is clicked
        openplc_connect = false;
    }

}
