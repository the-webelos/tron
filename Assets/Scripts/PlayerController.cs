using System.Collections;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Camera))]
public class PlayerController:MonoBehaviour {
    public float moveSpeed = 300f;
    public float turnSpeed = 100f;
    public GameObject explosionSystem;
    public float secondsBeforeSpectate = 2f;
    public int framesBetweenWallDrops = 5;
	public PlayerInput playerInput;

	private CharacterController characterController;
    private Animator animator;
    private Camera mainCam;
    private bool dead = false;
    private int ticksSinceLastDroppedWall = 0;
	private GamePrep gamePrep;
	private WallSpawnManager wallSpawnManager;
	private Camera playerCam;

	public GameObject trailWallPrefab;

    void Start() {
        characterController = GetComponent<CharacterController>();
		gamePrep = GameObject.Find("GameManagement").GetComponent<GamePrep>();
		wallSpawnManager = GameObject.Find("SpawnManager").GetComponent<WallSpawnManager>();
		playerCam = GetComponentInChildren<Camera>();

        // enable the follow camera and disable the main camera
        playerCam.enabled = true;
        mainCam = Camera.main;
        if (mainCam != null) {
            mainCam.enabled = false;
        }
    }

    void FixedUpdate() {
		if (!gamePrep.complete) return;

        if (ticksSinceLastDroppedWall > framesBetweenWallDrops) {
            DropWall();
            ticksSinceLastDroppedWall = 0;
        } else {
            ticksSinceLastDroppedWall++;
        }
    }

    void LateUpdate() {
        if (characterController == null || dead || !gamePrep.complete) return;

        transform.Rotate(0f, playerInput.GetLook() * Time.fixedDeltaTime, 0f);

        Vector3 direction = new Vector3(0f, 0f, 1f) * moveSpeed;
        direction = transform.TransformDirection(direction);
        characterController.SimpleMove(direction * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter(Collider other) {
        //if (other.CompareTag("Wall")) {
       //     StartCoroutine(PlayerDeath());
       // }
	   if (!dead) StartCoroutine(PlayerDeath());
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
		return;

		Vector3 p = new Vector3(transform.position.x, transform.position.y, transform.position.z) - (transform.forward.normalized * 2f);
//        Quaternion q = transform.rotation;

		GameObject wall = wallSpawnManager.GetFromPool(p);
		wall.transform.rotation = transform.rotation;

		NetworkServer.Spawn(wall, wallSpawnManager.assetId);

		StartCoroutine(DestroyWall(wall, 30.0f));

		// Drop wall with same rotation is that of the player
		//GameObject wall = Instantiate(trailWallPrefab, p, q);
	}

	private IEnumerator DestroyWall(GameObject wall, float timer)
	{
		yield return new WaitForSeconds(timer);
		wallSpawnManager.UnSpawnObject(wall);
		NetworkServer.UnSpawn(wall);
	}

}
