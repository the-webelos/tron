using System.Collections;
using UnityEngine;
using Mirror;

namespace Webelos.Tron
{

	[RequireComponent(typeof(CharacterController))]
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
		private ServerGamePrep gamePrep;
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
			base.OnStartLocalPlayer();

			playerInput = GetComponent<PlayerInput>();

			characterController = GetComponent<CharacterController>();
			gamePrep = GameObject.Find("GameManagement").GetComponent<ServerGamePrep>();

			//Save main camera transform so we can reset after we die
			origCamPosition = Camera.main.transform.position;
			origCamRotation = Camera.main.transform.rotation;

			Camera.main.transform.SetParent(transform, false);

			Camera.main.transform.localPosition = new Vector3(0, 1.65f, -5.05f);
			Camera.main.transform.LookAt(transform);
		}

		void FixedUpdate()
		{
			if (!isLocalPlayer || dead || gamePrep == null || !gamePrep.IsComplete() || characterController == null) return;

			transform.Rotate(0f, playerInput.GetLook() * Time.fixedDeltaTime, 0f);

			Vector3 direction = new Vector3(0f, 0f, 1f) * moveSpeed;
			direction = transform.TransformDirection(direction);
			characterController.SimpleMove(direction * Time.fixedDeltaTime);

			if (ticksSinceLastDroppedWall > framesBetweenWallDrops) {
				// Call the server and tell it to spawn a wall
				CmdDropWall();
				ticksSinceLastDroppedWall = 0;
			} else {
				ticksSinceLastDroppedWall++;
			}

		}

		[ServerCallback]
		private void OnTriggerEnter(Collider other)
		{
			//if (other.CompareTag("Wall")) {
			//     StartCoroutine(PlayerDeath());
			// }
			if (!dead) CmdPlayerExplode();
		}

		[Command]
		void CmdPlayerExplode()
		{
			// spawn an explosion
			if (explosionSystem) {
				GameObject explosion = Instantiate(explosionSystem, transform.position, Quaternion.identity);
				NetworkServer.Spawn(explosion);
			}

			// call all the clients and set this player as dead
			RpcDead();
		}

		[ClientRpc]
		void RpcDead()
		{
			// stop the player's movement and disable the renderer so the player object is hidden
			dead = true;
			GetComponentInChildren<MeshRenderer>().enabled = false;

			// if this object belongs to the local player we want to reset the camera after a few seconds
			if (isLocalPlayer) {
				StartCoroutine(WaitAndReset());
			}
		}

		private IEnumerator WaitAndReset()
		{
			// after secondsBeforeSpectate, enable the main camera for spectating and destroy this game object
			yield return new WaitForSeconds(secondsBeforeSpectate);

			//reset camera
			Camera.main.transform.SetPositionAndRotation(origCamPosition, origCamRotation);
			Camera.main.transform.SetParent(null);
		}

		[Command]
		void CmdDropWall()
		{
			Vector3 p = new Vector3(transform.position.x, transform.position.y, transform.position.z) - (transform.forward.normalized * 2f);

			GameObject wall = Instantiate(trailWallPrefab, p, transform.rotation);
			wall.GetComponent<Renderer>().material.color = playerColor;

			NetworkServer.Spawn(wall);

			StartCoroutine(DestroyWall(wall, 30.0f));
		}

		private IEnumerator DestroyWall(GameObject wall, float timer)
		{
			yield return new WaitForSeconds(timer);
			NetworkServer.UnSpawn(wall);
		}
	}
}