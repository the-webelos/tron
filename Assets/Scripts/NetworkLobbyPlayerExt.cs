using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

using UnityEngine.UI;

namespace Webelos.Tron
{
    public class NetworkLobbyPlayerExt : NetworkLobbyPlayer
    {
		public Text nameText;
		public Text isReadyText;

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
				GameObject playerListContent = GameObject.Find("PlayerListContent");

				gameObject.transform.position = new Vector3(0, 0, 0);

				gameObject.transform.SetParent(playerListContent.transform, false);
				gameObject.transform.SetAsLastSibling();

				LayoutRebuilder.ForceRebuildLayoutImmediate(playerListContent.transform as RectTransform);
			}
		}

		void OnReadyClick()
		{
			CmdChangeReadyState(!ReadyToBegin);
		}

		public override void OnClientEnterLobby()
        {
			if (LogFilter.Debug) Debug.LogFormat("OnClientEnterLobby index:{0} netId:{1} {2}", Index, netId, SceneManager.GetActiveScene().name);

			if (NetworkClient.active && isLocalPlayer) {
				Button button = GameObject.Find("ReadyButton").GetComponent<Button>() as Button;
				button.onClick.AddListener(OnReadyClick);
			}

			nameText.text = $"Player [{Index + 1}]";
			isReadyText.text = "Not Ready";
		}

		public override void OnClientReady(bool readyState) {
			Text buttonText = GameObject.Find("ReadyButton").GetComponentInChildren<Text>() as Text;

			if (readyState) {
				isReadyText.text = "Ready";
				buttonText.text = "Cancel";
			} else {
				isReadyText.text = "Not Ready";
				buttonText.text = "Ready";
			}
		}
	}
}
