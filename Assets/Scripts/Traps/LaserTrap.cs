using UnityEngine;
using System.Collections;

public class LaserTrap : MonoBehaviour {

	public int faceDir;
	public bool horizontal;
	public float onTime;
	public float offTime;
	public float warmupTime;

	float raySpacing = .17f;
	float onTimer;
	float offTimer;
	float warmupTimer;

	public LayerMask playerMask;

	PlayerManager playerManager;
	LineRenderer laserRender;
	Vector3 laserHit;
	BoxCollider2D col;
	Vector2 rayOrigin;
	Vector2 rayOrigin0;
	Vector2 rayOrigin1;
	Material[] mats;
	bool off;
	bool on;
	bool warmup;

	// Use this for initialization
	void Start () {
		col = GetComponent<BoxCollider2D> ();
		laserRender = GetComponent<LineRenderer> ();
		laserRender.enabled = false;
		laserRender.useWorldSpace = true;

		playerManager = FindObjectOfType<PlayerManager> ();

		mats = laserRender.materials;

		offTimer = offTime;
		off = true;

		if (horizontal) {
			transform.localEulerAngles = new Vector3 (transform.rotation.x, transform.rotation.y, transform.rotation.z - 90);
			rayOrigin = new Vector2 (transform.position.x + (col.bounds.extents.y + .01f) * faceDir, transform.position.y);
			rayOrigin0 = new Vector2 (transform.position.x + (col.bounds.extents.y + .01f) * faceDir, transform.position.y + raySpacing);
			rayOrigin1 = new Vector2 (transform.position.x + (col.bounds.extents.y + .01f) * faceDir, transform.position.y - raySpacing);
		} else {
			rayOrigin = new Vector2 (transform.position.x , transform.position.y + (col.bounds.extents.y + .01f) * faceDir);
			rayOrigin0 = new Vector2 (transform.position.x + raySpacing, transform.position.y + (col.bounds.extents.y + .01f) * faceDir);
			rayOrigin1 = new Vector2 (transform.position.x - raySpacing, transform.position.y + (col.bounds.extents.y + .01f) * faceDir);
		}
		transform.localScale = new Vector3 (transform.localScale.x, transform.localScale.y * faceDir, transform.localScale.z);
	}
	
	// Update is called once per frame
	void Update () {
		if (horizontal) {
			transform.localEulerAngles = new Vector3 (transform.rotation.x, transform.rotation.y, transform.rotation.z - 90);
			rayOrigin = new Vector2 (transform.position.x + (col.bounds.extents.y + .01f) * faceDir, transform.position.y);
			rayOrigin0 = new Vector2 (transform.position.x + (col.bounds.extents.y + .01f) * faceDir, transform.position.y + raySpacing);
			rayOrigin1 = new Vector2 (transform.position.x + (col.bounds.extents.y + .01f) * faceDir, transform.position.y - raySpacing);
		} else {
			rayOrigin = new Vector2 (transform.position.x , transform.position.y + (col.bounds.extents.y + .01f) * faceDir);
			rayOrigin0 = new Vector2 (transform.position.x + raySpacing, transform.position.y + (col.bounds.extents.y + .01f) * faceDir);
			rayOrigin1 = new Vector2 (transform.position.x - raySpacing, transform.position.y + (col.bounds.extents.y + .01f) * faceDir);
		}
		transform.localScale = new Vector3 (transform.localScale.x, transform.localScale.y * faceDir, transform.localScale.z);
		//Check if laser is on
		if (onTimer > 0) {
			onTimer -= Time.deltaTime;
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, transform.up * faceDir);
			Debug.DrawLine (rayOrigin, hit.point);
			if (hit.point != new Vector2 (0, 0)) {
				laserHit = new Vector3 (hit.point.x, hit.point.y, transform.position.z);
			} else {
				laserHit = new Vector3 (transform.position.x, transform.position.y + 30 * faceDir);
			}
			laserRender.SetPosition (1, laserHit);
			laserRender.SetPosition (0, transform.position);

			//Check if player is touching the laser
			RaycastHit2D playerHit0 = Physics2D.Raycast (rayOrigin0, transform.up * faceDir, Vector3.Distance (transform.position, laserHit), playerMask);
			Debug.DrawRay (rayOrigin0, transform.up * faceDir, Color.green);

			RaycastHit2D playerHit1 = Physics2D.Raycast (rayOrigin1, transform.up * faceDir, Vector3.Distance (transform.position, laserHit), playerMask);
			Debug.DrawRay (rayOrigin1, transform.up * faceDir, Color.green);
			if (playerHit0 || playerHit1) {
				playerManager.Die ();
			}
		} 
		//Turn laser off
		else if (on) {
			off = true;
			on = false;
			offTimer = offTime;
			laserRender.enabled = false;
		} 

		//Check if laser is warming up
		else if (warmupTimer > 0) {
			warmupTimer -= Time.deltaTime;
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, transform.up * faceDir);
			Debug.DrawLine (rayOrigin, hit.point);
			if (hit.point != new Vector2 (0, 0)) {
				laserHit = new Vector3 (hit.point.x, hit.point.y, transform.position.z);
			} else {
				laserHit = new Vector3 (transform.position.x, transform.position.y + 30 * faceDir);
			}
			laserRender.SetPosition (1, laserHit);
			laserRender.SetPosition (0, transform.position);
		}
		//Turn the laser on
		else if (warmup) {
			onTimer = onTime;
			on = true;
			warmup = false;
			laserRender.SetWidth (.34f, .34f);
			laserRender.material = mats [0];
		} 

		//Check if laser is off
		else if(offTimer > 0) {
			offTimer -= Time.deltaTime;
		} 

		//Start the warmup
		else if (off) {
			warmup = true;
			off = false;
			laserRender.enabled = true;
			laserRender.SetWidth (.1f, .1f);
			laserRender.material = mats [1];
			warmupTimer = warmupTime;
		} 
	}
}
