using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;

namespace Webelos.Tron
{
	public class NetworkManagerExt : NetworkLobbyManager
	{
		public InputField hostIPInputField;

		/// <summary>
		/// Called just after GamePlayer object is instantiated and just before it replaces LobbyPlayer object.
		/// This is the ideal point to pass any data like player name, credentials, tokens, colors, etc.
		/// into the GamePlayer object as it is about to enter the Online scene.
		/// </summary>
		/// <param name="lobbyPlayer"></param>
		/// <param name="gamePlayer"></param>
		/// <returns>true unless some code in here decides it needs to abort the replacement</returns>
		public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
		{
			// PlayerController player = gamePlayer.GetComponent<PlayerController>();
			// player.Index = lobbyPlayer.GetComponent<NetworkLobbyPlayer>().Index;
			// player.playerColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
			return true;
		}

		/*
            This code below is to demonstrate how to do a Start button that only appears for the Host player
            showStartButton is a local bool that's needed because OnLobbyServerPlayersReady is only fired when
            all players are ready, but if a player cancels their ready state there's no callback to set it back to false
            Therefore, allPlayersReady is used in combination with showStartButton to show/hide the Start button correctly.
            Setting showStartButton false when the button is pressed hides it in the game scene since NetworkLobbyManager
            is set as DontDestroyOnLoad = true.
        */

		bool showStartButton = true;

		public override void OnLobbyServerPlayersReady()
		{
			// calling the base method calls ServerChangeScene as soon as all players are in Ready state.
			if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null && startOnHeadless)
				base.OnLobbyServerPlayersReady();
			else {
				Text buttonText = GameObject.Find("StartButton").GetComponentInChildren<Text>() as Text;
				buttonText.text = "Start";
			}
		}

		public override void OnGUI()
		{
			base.OnGUI();

			if (allPlayersReady && showStartButton && GUI.Button(new Rect(150, 300, 120, 20), "START GAME")) {
				// set to false to hide it in the game scene
				showStartButton = false;

				ServerChangeScene(GameplayScene);
			}
		}

		public void OnJoinButton()
		{
			networkAddress = hostIPInputField.text.ToString();
			StartClient();
		}

		public void OnHostButton()
		{
			networkAddress = hostIPInputField.text.ToString();
			StartHost();
		}

		void OnStartClick()
		{
			ServerChangeScene(GameplayScene);
		}

		public override void OnLobbyClientSceneChanged(NetworkConnection conn)
		{
			if (SceneManager.GetActiveScene().name == LobbyScene) {
				Button startButton = GameObject.Find("StartButton").GetComponent<Button>() as Button;

				if (ClientScene.localPlayer.isServer) {
					startButton.GetComponentInChildren<Text>().text = "Force Start";
					startButton.onClick.AddListener(OnStartClick);
				} else {
					startButton.gameObject.SetActive(false);
				}
			}
		}
	}
}