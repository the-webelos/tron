using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePrep : MonoBehaviour
{
	public StartCountdown startCountdown;
	public Text countdownText;
	public bool complete;

	void Start() {
		complete = false;
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
