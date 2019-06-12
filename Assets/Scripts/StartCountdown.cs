using UnityEngine;
using Mirror;


public class StartCountdown : NetworkBehaviour
{
	[SyncVar]
	public float timeleft = 3.0f;

    // Update is called once per frame
    void Update()
    {
		if (!isServer || timeleft < 0) return;

		timeleft -= Time.deltaTime;
	}
}
