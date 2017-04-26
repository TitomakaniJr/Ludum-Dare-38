using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {
	public GameObject playerPrefab;
	public PlayerAbilities playerAbilities;
	[HideInInspector]
	public MeshRenderer contextualText;

	int frameCount;
	bool respawned;

	GameObject player;
	CheckpointController checkpointController;
	CannibalAlien boss;

	// Use this for initialization
	void Start () {
		respawned = false;
		checkpointController = FindObjectOfType<CheckpointController> ();
		player = GameObject.Find("Player");
		contextualText = player.GetComponentInChildren<MeshRenderer> ();
		boss = FindObjectOfType<CannibalAlien> ();
		frameCount = 0;
	}

	void Update(){
		if (player == null) {
			player = GameObject.Find("Player");
		}
		if (respawned == true) {
			if (frameCount == 0) {
				frameCount++;
			} else {
				player.transform.localEulerAngles = checkpointController.currentCheckpoint.transform.localEulerAngles;
				player.GetComponent<Player> ().standPlanet = checkpointController.currentCheckpoint.GetComponent<Checkpoint> ().myPlanet;
				respawned = false;
			}
		}
	}
	public void Die(){
		Destroy (player);
		Respawn ();
	}

	//create new player object at last checkpoint
	void Respawn(){
		frameCount = 0;
		Instantiate (playerPrefab, checkpointController.currentCheckpoint.transform.position + new Vector3(0,-1f), Quaternion.identity);
		respawned = true;
		boss.Killed ();
	}
		

	public struct PlayerAbilities{
		public bool doubleJump;
		public bool wallClimb;
		public bool canDash;
		public bool canDoubleJump;
	}
}
