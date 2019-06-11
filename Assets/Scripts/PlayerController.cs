﻿using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Camera))]
public class PlayerController:MonoBehaviour {
    public float moveSpeed = 300f;
    public float turnSpeed = 100f;
    public GameObject explosionSystem;
    public float secondsBeforeSpectate = 2f;

    private CharacterController characterController;
    private Camera cam;
    private Animator animator;
    private Camera mainCam;
    private bool dead = false;
    private float turn = 0f;
    private Transform myTransform;
    private Vector3 lastWallDropPosition;

    public GameObject trailWallPrefab;

    void Start() {
        characterController = GetComponent<CharacterController>();
        cam = GetComponentInChildren<Camera>();

        // enable the follow camera and disable the main camera
        cam.enabled = true;
        mainCam = Camera.main;
        if (mainCam != null) {
            mainCam.enabled = false;
        }
        myTransform = transform;
        DropWall();
    }

    void FixedUpdate() {
        Vector2 mousePosition = Input.mousePosition;
        Vector3 screenPosition = cam.ScreenToViewportPoint(new Vector3(mousePosition.x, mousePosition.y, transform.position.z - cam.transform.position.z));

        // if screenPosition.x is outside of range [0, 1], clamp it
        // can technically happen if the mouse is outside of the viewport
        if (screenPosition.x > 1f) {
            turn = 0.5f * turnSpeed;
        } else if (screenPosition.x < 0f) {
            turn = -0.5f * turnSpeed;
        } else {
            turn = (screenPosition.x - 0.5f) * turnSpeed;
        }

        float playerVelocityMagniture = characterController.velocity.magnitude;
        float playerPositionMagnitude = characterController.transform.position.magnitude;

        Debug.Log("Player velocity = " + playerVelocityMagniture);
        Debug.Log("Player position = " + playerPositionMagnitude);

        float positionDiff = Mathf.Abs(playerPositionMagnitude - lastWallDropPosition.magnitude);

        Debug.Log("Position diff = " + positionDiff);
        if (playerVelocityMagniture > 0 && positionDiff > 3)
        {
            DropWall();
        }

        //if (Input.GetKey(KeyCode.W))
        //{ //Up movement
        //    myTransform.rotation = Quaternion.Euler(0, 0, 0);
        //}

        //if (Input.GetKey(KeyCode.A))
        //{ //Left movement
        //    myTransform.rotation = Quaternion.Euler(0, 270, 0);
        //}

        //if (Input.GetKey(KeyCode.S))
        //{ //Down movement
        //    myTransform.rotation = Quaternion.Euler(0, 180, 0);
        //}

        //if (Input.GetKey(KeyCode.D))
        //{ //Right movement
        //    myTransform.rotation = Quaternion.Euler(0, 90, 0);
        //}
    }

    void LateUpdate() {
        if (characterController == null || dead) return;

        transform.Rotate(0f, turn * Time.fixedDeltaTime, 0f);

        Vector3 direction = new Vector3(0f, 0f, 1f) * moveSpeed;
        direction = transform.TransformDirection(direction);
        characterController.SimpleMove(direction * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Wall")) {
            StartCoroutine(PlayerDeath());
        }
    }

    private IEnumerator PlayerDeath() {
        // stop the player's movement and disable the renderer so the player object is hidden
        dead = true;
        GetComponent<MeshRenderer>().enabled = false;

        // spawn an explosion
        if (explosionSystem) {
            GameObject explosion = Instantiate(explosionSystem, transform.position, Quaternion.identity);
        }

        // after secondsBeforeSpectate, enable the main camera for spectating and destroy this game object
        yield return new WaitForSeconds(secondsBeforeSpectate);
        mainCam.enabled = true;
        Destroy(gameObject);
    }

    private void DropWall() {
        lastWallDropPosition = new Vector3(Mathf.RoundToInt(myTransform.position.x), trailWallPrefab.transform.position.y, Mathf.RoundToInt(myTransform.position.z));
        Instantiate(trailWallPrefab, lastWallDropPosition, characterController.transform.rotation);
    }
}
