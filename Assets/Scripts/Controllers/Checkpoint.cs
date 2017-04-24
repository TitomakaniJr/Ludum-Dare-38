using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {

	public bool active;

	public Sprite shipActive;
	public Sprite shipInactive;
	public Planet myPlanet;

	SpriteRenderer textRend;
	CheckpointController checkpointController;
	SpriteRenderer rend;
	AngleController angCont;
	// Use this for initialization
	void Start () {
		angCont = FindObjectOfType<AngleController> ();
		checkpointController = FindObjectOfType<CheckpointController> ();
		rend = GetComponent<SpriteRenderer> ();
		textRend = transform.Find ("Text").GetComponent<SpriteRenderer> ();
		transform.localEulerAngles = angCont.GetAngle (transform.position, myPlanet);
	}
		
	void Update(){
		if (checkpointController == null){
			checkpointController = FindObjectOfType<CheckpointController> ();
		}
		if (checkpointController.currentCheckpoint == gameObject && !active) {
			rend.sprite = shipActive;
			active = true;
		} else if(active && checkpointController.currentCheckpoint != gameObject) {
			rend.sprite = shipInactive;
			active = false;
		}
	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.tag == "Player" && checkpointController.currentCheckpoint != gameObject) {
			SetTextEnable(true);
			if (Input.GetKeyDown (KeyCode.F) || Input.GetKey(KeyCode.F)) {
				checkpointController.currentCheckpoint = gameObject;
				SetTextEnable (false);
			}
		}
	}

	void OnTriggerStay2D(Collider2D col){
		if (col.tag == "Player" && (Input.GetKeyDown(KeyCode.F) || Input.GetKey(KeyCode.F)) && checkpointController.currentCheckpoint != gameObject) {
			checkpointController.currentCheckpoint = gameObject;
			SetTextEnable (false);
		}
	}

	void OnTriggerExit2D(Collider2D col){
		SetTextEnable(false);
	}

	void SetTextEnable(bool enable){
		textRend.enabled = enable;
	}
}