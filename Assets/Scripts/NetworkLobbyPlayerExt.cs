using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

using UnityEngine.UI;

namespace Webelos.Tron
{
    public class NetworkLobbyPlayerExt : NetworkLobbyPlayer
    {
        public string Name;
        public override void OnStartClient()
        {
            if (LogFilter.Debug) Debug.LogFormat("OnStartClient {0}", SceneManager.GetActiveScene().name);

            base.OnStartClient();
            NetworkLobbyManager lobby = NetworkManager.singleton as NetworkLobbyManager;

			/*
                This demonstrates how to set the parent of the LobbyPlayerPrefab to an arbitrary scene object
                A similar technique would be used if a full canvas layout UI existed and we wanted to show
                something more visual for each player in that layout, such as a name, avatar, etc.

                Note: LobbyPlayer prefab will be marked DontDestroyOnLoad and carried forward to the game scene.
                      Because of this, NetworkLobbyManager must automatically set the parent to null
                      in ServerChangeScene and OnClientChangeScene.
            */

			if (lobby != null && SceneManager.GetActiveScene().name == lobby.LobbyScene) {
    			gameObject.transform.SetParent(GameObject.Find("PlayerListContent").transform);
			}
		}

		void OnReadyClick()
		{
			Debug.LogWarningFormat("OnReadyClick {0} {1} {2}", SceneManager.GetActiveScene().name, isLocalPlayer, netId);

			Text buttonText = GameObject.Find("ReadyButton").GetComponentInChildren<Text>() as Text;
			if (ReadyToBegin) {
				buttonText.text = "ReadyBar";
				CmdChangeReadyState(false);
			} else {
				buttonText.text = "CancelFoo";
				CmdChangeReadyState(true);
			}
		}

		public override void OnClientEnterLobby()
        {
			if (LogFilter.Debug) Debug.LogFormat("OnClientEnterLobby index:{0} netId:{1} {2}", Index, netId, SceneManager.GetActiveScene().name);

			if (NetworkClient.active && isLocalPlayer) {
				Debug.LogWarningFormat("Set OnReadyClick {0} {1}", isLocalPlayer, netId);
				Button button = GameObject.Find("ReadyButton").GetComponent<Button>() as Button;
				button.onClick.AddListener(OnReadyClick);
			}
		}

		public override void OnClientExitLobby()
        {
            if (LogFilter.Debug) Debug.LogFormat("OnClientExitLobby {0}", SceneManager.GetActiveScene().name);
			gameObject.transform.SetParent(null);
		}
	}
}
