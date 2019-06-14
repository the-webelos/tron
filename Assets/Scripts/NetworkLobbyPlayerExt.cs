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

        [SyncVar(hook = nameof(UpdatePlayerColor))]
        public Color playerColor;

		[SyncVar]
		public string Name;

        private GameObject playerPreview;
        private GameObject playerIcon;
        private Text playerPreviewName;

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
				Name = "Player " + Index;

                //LayoutRebuilder.ForceRebuildLayoutImmediate(playerListContent.transform as RectTransform);
            }
		}

		void OnReadyClick()
		{
			CmdChangeReadyState(!ReadyToBegin);
		}

        public void NextColor() {
            playerColor = PlayerColors.NextColor(PlayerColors.NextColor(playerColor));
        }

        public void PreviousColor() {
            playerColor = PlayerColors.PreviousColor(PlayerColors.PreviousColor(playerColor));
        }

        public override void OnClientEnterLobby()
		{
			if (LogFilter.Debug) Debug.LogFormat("OnClientEnterLobby index:{0} netId:{1} {2}", Index, netId, SceneManager.GetActiveScene().name);

            if (NetworkClient.active && isLocalPlayer) {
                Button button = GameObject.Find("ReadyButton").GetComponent<Button>() as Button;
                button.onClick.AddListener(OnReadyClick);

                Button button1 = GameObject.Find("ToggleRight").GetComponent<Button>() as Button;
                button1.onClick.AddListener(NextColor);

                Button button2 = GameObject.Find("ToggleLeft").GetComponent<Button>() as Button;
                button2.onClick.AddListener(PreviousColor);

                playerPreview = GameObject.Find("PlayerPreview");
                playerIcon = gameObject.transform.Find("Player").Find("PlayerBodyCapsule").gameObject;

                playerPreviewName = GameObject.Find("PlayerName").GetComponent<Text>();
                playerPreviewName.text = Name;
            }

            nameText.text = Name;
            isReadyText.text = "Not Ready";
            playerColor = PlayerColors.RandomColor();
        }

		public override void OnClientReady(bool readyState)
		{
			if (readyState) {
				isReadyText.text = "Ready";
			} else {
				isReadyText.text = "Not Ready";
			}

			if (NetworkClient.active && isLocalPlayer) {
				GameObject readyButton = GameObject.Find("ReadyButton");
				if (readyButton == null) return;

				Text buttonText = readyButton.GetComponentInChildren<Text>() as Text;

				if (readyState) {
					buttonText.text = "Cancel";
				} else {
					buttonText.text = "Ready";
				}
			}
		}

        private void UpdatePlayerColor(Color color) {
            playerPreview.GetComponent<MeshRenderer>().material.color = color;
            playerIcon.GetComponent<MeshRenderer>().material.color = color;
        }
	}
}