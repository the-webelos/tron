using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DebugHUD : MonoBehaviour
{
	NetworkManager manager;
	public bool showGUI = true;
	public int offsetX;
	public int offsetY;

	void Awake()
	{
		manager = GetComponent<NetworkManager>();
	}

	void OnGUI()
	{
		if (!showGUI)
			return;

		GUILayout.BeginArea(new Rect(10 + offsetX, 40 + offsetY, 415, 9999));

		if (NetworkServer.active) {
			GUILayout.Label("Server: active. Transport: " + Transport.activeTransport);
			foreach (NetworkConnection connection in NetworkServer.connections.Values) {
				GUILayout.Label("Server: connection: " + connection + " controller: " + connection.playerController);
			}
		}

		if (NetworkClient.isConnected) {
			GUILayout.Label("Client: address=" + manager.networkAddress);
   			GUILayout.Label("Client: connId=" + NetworkClient.connection.connectionId);
			GUILayout.Label("Client: netId=" + ClientScene.localPlayer?.netId);
//			GUILayout.Label("Client: name=" + ClientScene.localPlayer?.name);
		}

		GUILayout.EndArea();
	}
}
