using UnityEngine;
using System.Collections;

public class AirResetController : MonoBehaviour {

	PlayerManager playerManager;
	// Use this for initialization
	void Start () {
		playerManager = FindObjectOfType<PlayerManager> ();

	}
	
	void OnTriggerEnter2D(Collider2D col){
		if (col.name == "Player") {
			playerManager.playerAbilities.canDash = true;
			playerManager.playerAbilities.canDoubleJump = true;
			Destroy (gameObject);
		}
	}
}
