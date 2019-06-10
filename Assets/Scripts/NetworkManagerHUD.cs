using MLAPI;
using System;
using UnityEngine;
using MLAPI.Transports.UNET;

public class NetworkManagerHUD : MonoBehaviour {
    private string address = "";
    private int port = -1;

    public void SetAddressAndPort(string addressAndPort) {
        Debug.Log("Setting address to " + addressAndPort);

        String[] split = addressAndPort.Split(':');

        if (split.Length > 1) {
            address = split[0];
            if (!Int32.TryParse(split[1], out port)) {
                port = -1;
            }
        }
    }

    public void StartHost() => NetworkingManager.Singleton.StartHost();

    public void StartClient() {
        if (address.Length > 0 && port != -1) {
            NetworkingManager.Singleton.GetComponent<UnetTransport>().ConnectAddress = address;
            NetworkingManager.Singleton.GetComponent<UnetTransport>().ConnectPort = port;
            NetworkingManager.Singleton.StartClient();
        }
    }
}

