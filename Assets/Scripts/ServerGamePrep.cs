using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

namespace Webelos.Tron
{
	public class ServerGamePrep : NetworkBehaviour
	{
		private CountdownTimer startCountdown;
		public MusicManager musicManager;

		[SyncVar]
		public int state = 0;  // 0 - waiting for players to load scene
							   // 1 - all players loaded
							   // 2 - prep complete

		private NetworkManagerExt networkManager;

		void Start()
		{
			networkManager = NetworkManager.singleton as NetworkManagerExt;
			startCountdown = GetComponent<CountdownTimer>();
		}

		public bool IsComplete()
		{
			return state == 2;
		}

		// Update is called once per frame
		[ServerCallback]
		void Update()
		{
			if (state == 0) {
				if (networkManager.numPlayers == networkManager.numPlayersLoadedGameScene) {
					state = 1;
					RpcWaitingForPlayers();
				}
			} else if (state == 1) {
				startCountdown.Tick(Time.deltaTime);
				if (startCountdown.timeLeft <= 0) {
					state = 2;
					RpcPrepComplete();
				}
			}
		}

		[ClientRpc]
		void RpcWaitingForPlayers()
		{
			Debug.LogWarningFormat("Waiting for players");
		}

		[ClientRpc]
		void RpcAllPlayersLoaded()
		{
			Debug.LogWarningFormat("All players loaded");
		}

		[ClientRpc]
		void RpcPrepComplete()
		{
			Debug.LogWarningFormat("Prep complete");

			if (musicManager) {
				//musicManager.PlayClips();
			}
		}
	}
}