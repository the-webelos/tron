using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class GamePrep : NetworkBehaviour
{
	public StartCountdown startCountdown;
	public Text countdownText;

	[SyncVar(hook = nameof(OnComplete))]
	public bool complete;

	void Start() {
		complete = false;
    }

	void OnComplete(bool comp) {
	}

	// Update is called once per fram
	void Update()
    {
		if (startCountdown.timeleft <= 0) {
			countdownText.text = "PLAY";
			complete = true;
        }
        else {
		    countdownText.text = Mathf.CeilToInt(startCountdown.timeleft).ToString();
        }
    }
}
