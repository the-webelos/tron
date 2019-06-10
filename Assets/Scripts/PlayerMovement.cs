//using MLAPI;
//using MLAPI.LagCompensation;
//using System.Collections;
using UnityEngine;
using System;

//public class PlayerMovement:NetworkedBehaviour {
//    void Update() {
//        if (IsLocalPlayer) {
//            transform.Translate(new Vector3(Input.GetAxis("Horizontal") * 2f * Time.deltaTime, 0, Input.GetAxis("Vertical") * 2f * Time.deltaTime));
//            if (Input.GetKeyDown(KeyCode.Space)) {
//                StartCoroutine(Simulate());
//                return;
//            }
//        }
//    }

//    IEnumerator Simulate() {
//        yield return new WaitForSeconds(1);
//        LagCompensationManager.Simulate(1f, () => {
//            for (int i = 0; i < LagCompensationManager.SimulationObjects.Count; i++) {
//                Debug.Log(LagCompensationManager.SimulationObjects[i].transform.position);
//                GameObject go = new GameObject();
//                go.transform.position = LagCompensationManager.SimulationObjects[i].transform.position;
//            }
//        });
//    }
//}

public class PlayerMovement:MonoBehaviour {
    public float speed = 10f;
    public float turnSpeed = 10f;

    private CharacterController characterController;
    private float previousHorizontalForce = 0f;
    private Rigidbody rb;
    private float yaw = 0f;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        characterController = GetComponent<CharacterController>();
    }

    void Update() {
        yaw += turnSpeed * Input.GetAxis("Mouse X");
    }

    private void FixedUpdate() {
        transform.Rotate(0f, yaw * Time.fixedDeltaTime, 0f);
        Vector3 direction = Vector3.ClampMagnitude(new Vector3(1f, 0, 0), 1f) * speed;
        direction = transform.TransformDirection(direction);
        characterController.SimpleMove(direction * Time.fixedDeltaTime);
    }
}
