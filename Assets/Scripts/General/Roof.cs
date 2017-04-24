using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roof : MonoBehaviour {

	public CannibalAlien can;
	PlayerManager playerMan;

	// Use this for initialization
	void Start () {
		playerMan = FindObjectOfType<PlayerManager> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D col){
		if(col.name == "Player") {
			print ("HIT");
			playerMan.playerAbilities.canDoubleJump = true;
			can.Hit();
		}
	}
}
