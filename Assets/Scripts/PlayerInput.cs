using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	public float turnSpeed;

	private float look;

	void FixedUpdate() {
		Vector2 mousePosition = Input.mousePosition;
		Vector3 screenPosition = Camera.main.ScreenToViewportPoint(
        		new Vector3(mousePosition.x, mousePosition.y, transform.position.z - Camera.main.transform.position.z));

		// if screenPosition.x is outside of range [0, 1], clamp it
		// can technically happen if the mouse is outside of the viewport
		if (screenPosition.x > 1f) {
			look = 0.5f * turnSpeed;
		} else if (screenPosition.x < 0f) {
			look = -0.5f * turnSpeed;
		} else {
			look = (screenPosition.x - 0.5f) * turnSpeed;
		}
    }

	public float GetLook() {
		return look;
	}
}
