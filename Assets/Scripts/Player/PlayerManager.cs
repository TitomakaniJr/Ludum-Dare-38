using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {
	public GameObject playerPrefab;
	public PlayerAbilities playerAbilities;
	[HideInInspector]
	public MeshRenderer contextualText;

	bool respawned;

	GameObject player;
	CheckpointController checkpointController;

	// Use this for initialization
	void Start () {
		respawned = false;
		checkpointController = FindObjectOfType<CheckpointController> ();
		player = GameObject.Find("Player");
		contextualText = player.GetComponentInChildren<MeshRenderer> ();
	}

	void Update(){
		if (player == null) {
			player = GameObject.Find("Player");
		}
		if (respawned == true) {
			player.transform.localEulerAngles = checkpointController.currentCheckpoint.transform.localEulerAngles;
			respawned = false;
		}
	}
	public void Die(){
		Destroy (player);
		Respawn ();
	}

	//create new player object at last checkpoint
	void Respawn(){
		Instantiate (playerPrefab, checkpointController.currentCheckpoint.transform.position + new Vector3(0,-1f), Quaternion.identity);
		respawned = true;
	}
		

	public struct PlayerAbilities{
		public bool doubleJump;
		public bool wallClimb;
		public bool canDash;
		public bool canDoubleJump;
	}
}
