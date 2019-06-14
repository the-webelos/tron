﻿using UnityEngine;
using UnityEngine.UI;
using Mirror;


namespace Webelos.Tron
{
	public class CountdownTimer : NetworkBehaviour
	{
		public float floatTimeleft = 3.0f;
		public Text timerText;
		public Animator countdownFadeAnimator;

		[SyncVar]
		public int timeLeft;

		public void Tick(float delta)
		{
			floatTimeleft -= delta;

			int newTimeLeft = Mathf.CeilToInt(floatTimeleft);

			if (newTimeLeft != timeLeft) {
				timeLeft = newTimeLeft;
				RpcTimeTick(newTimeLeft);
			}
		}

		[ClientRpc]
		void RpcTimeTick(int remaining)
		{
			if (remaining > 0) {
				timerText.text = remaining.ToString();
			} else {
				timerText.text = "PLAY";
				countdownFadeAnimator.SetTrigger("StartFade");
			}
		}
	}
}