using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;
using System.Collections;

namespace Webelos.Tron
{
	public class NetworkManagerExt : NetworkLobbyManager
	{
		public InputField hostIPInputField;

        public Toggle hostToggle;
        public Toggle joinToggle;
        private int deadCount = 0;

		public int numPlayersLoadedGameScene = 0;

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
			Debug.LogWarningFormat("OnLobbyServerSceneLoadedForPlayer {0} {1} {2}", SceneManager.GetActiveScene().name, numPlayers, numPlayersLoadedGameScene);

			PlayerController player = gamePlayer.GetComponent<PlayerController>();
			player.playerColor = lobbyPlayer.GetComponent<NetworkLobbyPlayerExt>().playerColor;
			player.Name = "Player " + lobbyPlayer.GetComponent<NetworkLobbyPlayer>().Index;
			//player.Index = lobbyPlayer.GetComponent<NetworkLobbyPlayer>().Index;

			numPlayersLoadedGameScene += 1;

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

        public override void OnLobbyServerPlayersReady() {
            // calling the base method calls ServerChangeScene as soon as all players are in Ready state.
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null && startOnHeadless)
                base.OnLobbyServerPlayersReady();
            else {
                Text buttonText = GameObject.Find("StartButton").GetComponentInChildren<Text>() as Text;
                buttonText.text = "Start";
            }
        }

        public void OnGo()
		{
			networkAddress = hostIPInputField.text;
			if (string.IsNullOrEmpty(networkAddress)) {
				networkAddress = "127.0.0.1";
			}

            if(joinToggle.isOn)
			    StartClient();
            else if (hostToggle.isOn)
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

				if (ClientScene.localPlayer && ClientScene.localPlayer.isServer) {
					startButton.GetComponentInChildren<Text>().text = "Force Start";
					startButton.onClick.AddListener(OnStartClick);
				} else {
					startButton.gameObject.SetActive(false);
				}
			}
		}

		public override void OnLobbyServerSceneChanged(string sceneName)
		{
			base.OnLobbyServerSceneChanged(sceneName);
			if (sceneName == GameplayScene) {
				numPlayersLoadedGameScene = 0;
			}
		}

        public void incrementDead()
        {
            deadCount += 1;

            if(deadCount == numPlayers - 1)
            {
                CmdInitiateEndGame();
            }
        }

        [Command]
        void CmdInitiateEndGame()
        {
            Debug.Log("Ending game...");
            StartCoroutine(WaitAndGoToLobby());
            // TODO Announce winner
        }

        private IEnumerator WaitAndGoToLobby()
        {
            yield return new WaitForSeconds(5.0f);
            SceneManager.LoadScene("Lobby");
        }
    }
}