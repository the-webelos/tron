using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController:MonoBehaviour {
    public float moveSpeed = 300f;
    public float turnSpeed = 100f;

    private CharacterController characterController;
    private Camera cam;
    private float turn = 0f;

    void Start() {
        characterController = GetComponent<CharacterController>();
        cam = GetComponentInChildren<Camera>();
        cam.enabled = true;
        Camera.main.enabled = false;
    }

    void FixedUpdate() {
        Vector2 mousePosition = Input.mousePosition;
        Vector3 screenPosition = cam.ScreenToViewportPoint(new Vector3(mousePosition.x, mousePosition.y, transform.position.z - cam.transform.position.z));
        turn = (screenPosition.x - 0.5f) * turnSpeed;
    }

    void LateUpdate() {
        if (characterController == null) return;

        transform.Rotate(0f, turn * Time.fixedDeltaTime, 0f);

        Vector3 direction = Vector3.ClampMagnitude(new Vector3(0f, 0f, 1f), 1f) * moveSpeed;
        direction = transform.TransformDirection(direction);
        characterController.SimpleMove(direction * Time.fixedDeltaTime);
    }
}
