using UnityEngine;
using System.Collections;

public class LaserTrap : MonoBehaviour {

	public int faceDir;
	public float onTime;
	public float offTime;
	public float warmupTime;
	public bool flipY;
	public bool left;
	public bool horizontal;
	public GameObject projectilePrefab;
	public LayerMask playerMask;
	public Planet myPlanet;

	float spawnTimer;
	float offsetTimer;
	float raySpacing = .17f;
	float onTimer;
	float offTimer;
	float warmupTimer;
	bool spawn;
	bool off;
	bool on;
	bool warmup;


	PlayerManager playerManager;
	LineRenderer laserRender;
	Vector3 laserHit;
	BoxCollider2D col;
	Vector2 rayOrigin;
	Vector2 rayOrigin0;
	Vector2 rayOrigin1;
	Material[] mats;

	Animator anim;
	AngleController angCont;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		angCont = FindObjectOfType<AngleController> ();
		col = GetComponent<BoxCollider2D> ();
		laserRender = GetComponent<LineRenderer> ();
		laserRender.enabled = false;
		laserRender.useWorldSpace = true;

		playerManager = FindObjectOfType<PlayerManager> ();

		mats = laserRender.materials;

		offTimer = offTime;
		off = true;

		if (horizontal) {
			transform.localEulerAngles = angCont.GetAngle (transform.position, myPlanet);
			rayOrigin = new Vector2 (transform.position.x + (col.bounds.extents.y + .01f) * faceDir, transform.position.y);
			rayOrigin0 = new Vector2 (transform.position.x + (col.bounds.extents.y + .01f) * faceDir, transform.position.y + raySpacing);
			rayOrigin1 = new Vector2 (transform.position.x + (col.bounds.extents.y + .01f) * faceDir, transform.position.y - raySpacing);
		} else {
			Vector3 newRot = new Vector3(0, 0, angCont.GetAngle (transform.position, myPlanet).z - 90);
			transform.localEulerAngles = newRot;
			rayOrigin = new Vector2 (transform.position.x , transform.position.y + (col.bounds.extents.y + .01f) * faceDir);
			rayOrigin0 = new Vector2 (transform.position.x + raySpacing, transform.position.y + (col.bounds.extents.y + .01f) * faceDir);
			rayOrigin1 = new Vector2 (transform.position.x - raySpacing, transform.position.y + (col.bounds.extents.y + .01f) * faceDir);
		}
		transform.localScale = new Vector3 (transform.localScale.x, transform.localScale.y * faceDir, transform.localScale.z);
	}
	
	// Update is called once per frame
	void Update () {
		
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
