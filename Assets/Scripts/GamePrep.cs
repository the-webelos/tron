using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class GamePrep : NetworkBehaviour
{
	public StartCountdown startCountdown;
	public Text countdownText;
    public MusicManager musicManager;
    public Animator countdownFadeAnimator;

	[SyncVar(hook = nameof(OnComplete))]
	public bool complete;

	void Start() {
		complete = false;
    }

	void OnComplete(bool comp) {
        if (musicManager && comp == true) {
            musicManager.PlayClips();
            countdownFadeAnimator.SetTrigger("StartFade");
        }
	}

	// Update is called once per frame
	void Update() {
		if (!complete) {
            if (startCountdown.timeleft <= 0) {
                countdownText.text = "PLAY";
                complete = true;
            } else {
                countdownText.text = Mathf.CeilToInt(startCountdown.timeleft).ToString();
            }
        }
    }
}
