using System.Collections;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(CharacterController))]
//[RequireComponent(typeof(Camera))]
public class PlayerController : NetworkBehaviour
{
	public float moveSpeed = 300f;
	public float turnSpeed = 100f;
	public GameObject explosionSystem;
	public float secondsBeforeSpectate = 2f;
	public int framesBetweenWallDrops = 5;

	[SyncVar]
	public string Name;

	private CharacterController characterController;
	private int ticksSinceLastDroppedWall = 0;
	private GamePrep gamePrep;
	private WallSpawnManager wallSpawnManager;
	private PlayerInput playerInput;
	private Vector3 origCamPosition;
	private Quaternion origCamRotation;

	public GameObject trailWallPrefab;

	[SyncVar(hook = nameof(SetColor))]
	public Color playerColor = Color.white;

	public bool dead = false;

	Material materialClone;

	void SetColor(Color color)
	{
		if (materialClone == null) materialClone = GetComponentInChildren<Renderer>().material;
		materialClone.color = color;
	}

	private void OnDestroy()
	{
		Destroy(materialClone);
	}

	public override void OnStartLocalPlayer()
	{
		Debug.LogWarningFormat("OnStartLocalPlayer {0}", name);
		base.OnStartLocalPlayer();

		playerInput = GetComponent<PlayerInput>();

        characterController = GetComponent<CharacterController>();
		gamePrep = GameObject.Find("GameManagement").GetComponent<GamePrep>();
		wallSpawnManager = GameObject.Find("SpawnManager").GetComponent<WallSpawnManager>();

		//Save main camera transform so we can reset after we die
		origCamPosition = Camera.main.transform.position;
		origCamRotation = Camera.main.transform.rotation;

		Camera.main.transform.SetParent(transform, false);

		Camera.main.transform.localPosition = new Vector3(0, 1.65f, -5.05f);
		Camera.main.transform.LookAt(transform);
	}

	void FixedUpdate()
	{
		if (!isLocalPlayer || dead || gamePrep == null || !gamePrep.complete || characterController == null) return;

		transform.Rotate(0f, playerInput.GetLook() * Time.fixedDeltaTime, 0f);

		Vector3 direction = new Vector3(0f, 0f, 1f) * moveSpeed;
		direction = transform.TransformDirection(direction);
		characterController.SimpleMove(direction * Time.fixedDeltaTime);

		if (ticksSinceLastDroppedWall > framesBetweenWallDrops) {
			DropWall();
			ticksSinceLastDroppedWall = 0;
		} else {
			ticksSinceLastDroppedWall++;
		}

	}

	private void OnTriggerEnter(Collider other)
	{
		//if (other.CompareTag("Wall")) {
		//     StartCoroutine(PlayerDeath());
		// }
		if (!dead) StartCoroutine(PlayerDeath());
	}

	private IEnumerator PlayerDeath()
	{
		// stop the player's movement and disable the renderer so the player object is hidden
		dead = true;
		GetComponentInChildren<MeshRenderer>().enabled = false;

		// spawn an explosion
		if (explosionSystem) {
//			GameObject explosion = Instantiate(explosionSystem, transform.position, Quaternion.identity);
			GameObject explosion = Instantiate(explosionSystem, transform, false);
		}

		// after secondsBeforeSpectate, enable the main camera for spectating and destroy this game object
		yield return new WaitForSeconds(secondsBeforeSpectate);

		//reset camera
		Camera.main.transform.SetPositionAndRotation(origCamPosition, origCamRotation);
		Camera.main.transform.SetParent(null);

		//Destroy(gameObject);
	}

	private void DropWall()
	{
		Vector3 p = new Vector3(transform.position.x, transform.position.y, transform.position.z) - (transform.forward.normalized * 2f);

		GameObject wall = wallSpawnManager.GetFromPool(p);
		wall.transform.rotation = transform.rotation;
        wall.GetComponent<Renderer>().material.color = playerColor;

        NetworkServer.Spawn(wall, wallSpawnManager.assetId);

		StartCoroutine(DestroyWall(wall, 30.0f));
	}

	private IEnumerator DestroyWall(GameObject wall, float timer)
	{
		yield return new WaitForSeconds(timer);
		wallSpawnManager.UnSpawnObject(wall);
		NetworkServer.UnSpawn(wall);
	}
}
