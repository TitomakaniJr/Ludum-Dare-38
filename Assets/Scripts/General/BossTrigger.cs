using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrigger : MonoBehaviour {

	CannibalAlien boss;
	// Use this for initialization
	void Start () {
		boss = FindObjectOfType<CannibalAlien> ();
	}
	
	void OnTriggerEnter2D(Collider2D col){
		if(col.name == "Player") {
			boss.StartFight ();
		}
	}
}
