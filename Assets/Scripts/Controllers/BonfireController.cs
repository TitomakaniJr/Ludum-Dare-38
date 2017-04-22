using UnityEngine;
using System.Collections;

public class BonfireController : MonoBehaviour {

	public CheckpointController checkpointController;

	public bool lit;

	Animator anim;
	MeshRenderer contextualText;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		contextualText = GetComponentInChildren<MeshRenderer> ();
		contextualText.transform.position = new Vector3 (transform.position.x, transform.position.y + 2, transform.position.z);
	}
		
	void Update(){
		if (checkpointController == null){
			checkpointController = FindObjectOfType<CheckpointController> ();
		}
		if (checkpointController.currentCheckpoint == gameObject && !lit) {
			lit = true;
			anim.SetBool ("Lit", lit);
		} else if(lit && checkpointController.currentCheckpoint != gameObject) {
			lit = false;
			anim.SetBool ("Lit", lit);
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
		contextualText.enabled = enable;
	}
}