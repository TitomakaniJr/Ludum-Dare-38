using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannibalAlien : MonoBehaviour {

	public float moveSpeed;
	public float strafeSpeed;
	public float swipeSpeed;
	public float easeAmount;
	public float shootTime;
	public GameObject projectile;
	public CapsuleCollider2D roof;
	public Vector3[] localWaypoints;
	public Vector3[] swipeWaypoints;
	public Vector3[] strafeWaypoints;
	public Planet myPlanet;
	public CapsuleCollider2D[] myCols;
	public GameObject femalePlatform;
	public GameObject female;

	int fromWaypointIndex;
	int faceDir;
	int bossFunc;
	int hitCounter;
	int moveCounter;
	int gunCounter;
	float percentBetweenWaypoints;
	float nextMoveTime;
	float shootTimer;
	float shootSpeed;
	float shootSpread;
	float funcTime;
	bool inFunc;
	bool movingToStart;
	bool setup;
	bool dead;
	bool inFight;
	Vector3 barrel_0;
	Vector3 barrel_1;
	Vector3 barrel_2;
	Vector3 defaultPos;
	Vector3[] globalWaypoints;
	AngleController angCont;
	Animator anim;

	void Start () {
		angCont = FindObjectOfType<AngleController> ();
		anim = GetComponent<Animator> ();
		shootTimer = shootTime;
		transform.localEulerAngles = angCont.GetAngle (transform.position, myPlanet);
		defaultPos = transform.position;
		inFunc = false;
		movingToStart = false;
		setup = true;
		dead = false;
		roof.enabled = false;
		gunCounter = 0;

		globalWaypoints = new Vector3[localWaypoints.Length];
		for (int i = 0; i < localWaypoints.Length; i++) {
			globalWaypoints [i] = localWaypoints [i] + transform.position;
		}
		for (int i = 0; i < swipeWaypoints.Length; i++) {
			swipeWaypoints [i] = swipeWaypoints [i] + transform.position;
		}
		for (int i = 0; i < strafeWaypoints.Length; i++) {
			strafeWaypoints [i] = strafeWaypoints [i] + transform.position;
		}
	}

	void Update () {
		if (!dead && inFight) {
			if (!inFunc) {
				globalWaypoints = new Vector3[2];
				globalWaypoints [0] = transform.position;
				globalWaypoints [1] = defaultPos;
				Vector3 velocity = CalculateTrapMovement (.5f);
				transform.localEulerAngles = angCont.GetAngle (transform.position, myPlanet);
				transform.Translate (velocity, Space.World);
				moveCounter = Random.Range (3, 6);
			}
			if (transform.position == globalWaypoints [1] || inFunc) {
				if (!inFunc) {
					if (gunCounter == 3) {
						bossFunc = 2;
					} else {
						bossFunc = Random.Range (0, 3);
					}
					if (bossFunc == 0 || bossFunc == 1) {
						gunCounter++;
						if (!anim.GetBool ("Gun")) {
							anim.ResetTrigger ("Plunge");
							anim.SetTrigger ("Gun");
						}
					} else {
						gunCounter = 0;
						if (!anim.GetBool ("Plunge")) {
							anim.ResetTrigger ("Gun");
							anim.SetTrigger ("Plunge");
						}
					}
					myCols [0].enabled = true;
					myCols [1].enabled = true;
				}
				if (bossFunc == 0) {
					if (!inFunc) {
						funcTime = 5;
						inFunc = true;
						barrel_0 = transform.Find ("Barrel_0").transform.position;
						barrel_1 = transform.Find ("Barrel_1").transform.position;
						barrel_2 = transform.Find ("Barrel_2").transform.position;
					}
					if (funcTime > 0) {
						funcTime -= Time.deltaTime;
						Quaternion rotation = Quaternion.Euler (0, 0, Mathf.PingPong (Time.time * 30, 180));
						if (shootTimer > 0) {
							shootTimer -= Time.deltaTime;
						} else {
							shootTimer = shootTime;
							int barrel = Random.Range (0, 3);
							GameObject project;
							ProjectileController projectileController;
							if (barrel == 0) {
								project = Instantiate (projectile, barrel_0, rotation);
								projectileController = project.GetComponent<ProjectileController> ();
								projectileController.SetValues (.1f + (.01f * hitCounter), 1, false, myPlanet);
							} else if (barrel == 1) {
								project = Instantiate (projectile, barrel_1, rotation);
								projectileController = project.GetComponent<ProjectileController> ();
								projectileController.SetValues (.1f + (.01f * hitCounter), 1, false, myPlanet);
							} else if (barrel == 2) {
								project = Instantiate (projectile, barrel_2, rotation);
								projectileController = project.GetComponent<ProjectileController> ();
								projectileController.SetValues (.1f + (.01f * hitCounter), 1, false, myPlanet);
							}
						}
					} else {
						inFunc = false;
						moveCounter--;
					}
				} else {
					int left = Random.Range (0, 2);
					if (!inFunc) {
						if (left == 1) {
							System.Array.Reverse (bossFunc == 1 ? strafeWaypoints : swipeWaypoints);
						}
						inFunc = true;
						movingToStart = true;
						globalWaypoints = new Vector3[2];
						globalWaypoints [0] = transform.position;
						globalWaypoints [1] = bossFunc == 1 ? strafeWaypoints [0] : swipeWaypoints [0];
					}
					if (movingToStart) {
						Vector3 velocity = CalculateTrapMovement (moveSpeed);
						transform.localEulerAngles = angCont.GetAngle (transform.position, myPlanet);
						transform.Translate (velocity, Space.World);
					}
					if (transform.position == globalWaypoints [1] && movingToStart) {
						movingToStart = false;
					}
					if (bossFunc == 1 && !movingToStart) { //STRAFE SHOOT
						if (setup) {
							globalWaypoints = new Vector3[strafeWaypoints.Length];
							for (int i = 0; i < strafeWaypoints.Length; i++) {
								globalWaypoints [i] = strafeWaypoints [i];
							}
							setup = false;
						}
						Vector3 velocity = CalculateTrapMovement (strafeSpeed);
						transform.localEulerAngles = angCont.GetAngle (transform.position, myPlanet);
						transform.Translate (velocity, Space.World);
						barrel_0 = transform.Find ("Barrel_0").transform.position;
						barrel_1 = transform.Find ("Barrel_1").transform.position;
						barrel_2 = transform.Find ("Barrel_2").transform.position;
						if (transform.position == globalWaypoints [strafeWaypoints.Length - 1]) {
							inFunc = false;
							setup = true;
							moveCounter--;
							if (left == 1) {
								System.Array.Reverse (strafeWaypoints);
							}
						} else {
							if (shootTimer > 0) {
								shootTimer -= Time.deltaTime;
							} else {
								shootTimer = shootTime;
								int barrel = Random.Range (0, 3);
								GameObject project;
								ProjectileController projectileController;
								if (barrel == 0) {
									project = Instantiate (projectile, barrel_0, Quaternion.identity);
									project.transform.localEulerAngles = new Vector3 (0, 0, transform.localEulerAngles.z + 90);
									projectileController = project.GetComponent<ProjectileController> ();
									projectileController.SetValues (.1f, 1, false, myPlanet);
								} else if (barrel == 1) {
									project = Instantiate (projectile, barrel_1, Quaternion.identity);
									project.transform.localEulerAngles = new Vector3 (0, 0, transform.localEulerAngles.z + 90);
									projectileController = project.GetComponent<ProjectileController> ();
									projectileController.SetValues (.1f, 1, false, myPlanet);
								} else if (barrel == 2) {
									project = Instantiate (projectile, barrel_2, Quaternion.identity);
									project.transform.localEulerAngles = new Vector3 (0, 0, transform.localEulerAngles.z + 90);
									projectileController = project.GetComponent<ProjectileController> ();
									projectileController.SetValues (.1f, 1, false, myPlanet);
								}
							}
						}
							
					} else if (!movingToStart) { //SWIPE
						if (setup) {
							globalWaypoints = new Vector3[swipeWaypoints.Length];
							for (int i = 0; i < swipeWaypoints.Length; i++) {
								globalWaypoints [i] = swipeWaypoints [i];
							}
							setup = false;
							roof.enabled = true;
						}
						Vector3 velocity = CalculateTrapMovement (swipeSpeed);
						transform.localEulerAngles = angCont.GetAngle (transform.position, myPlanet);
						transform.Translate (velocity, Space.World);
						if (transform.position == globalWaypoints [swipeWaypoints.Length - 1]) {
							inFunc = false;
							moveCounter--;
							setup = true;
							roof.enabled = false;
							if (left == 1) {
								System.Array.Reverse (swipeWaypoints);
							}
						} else {

						}
					}
				} 
			}
		}
	}


		
	public void Hit(){
 		roof.enabled = false;
		myCols [0].enabled = false;
		myCols [1].enabled = false;
		hitCounter++;
		moveCounter = 0;
		setup = true;
		inFunc = false;
		shootTime -= .07f;
		anim.SetTrigger ("Hit");
		if (hitCounter == 3) {
			dead = true;
			femalePlatform.SetActive (true);
			female.SetActive (true);
		}
	}

	public void Killed(){
		if (!dead) {
			anim.SetTrigger ("Kill");
			hitCounter = 0;
			moveCounter = 0;
			setup = true;
			inFunc = false;
			shootTime = .5f;
			inFight = false;
			transform.position = defaultPos;
		}
	}

	public void StartFight(){
		inFight = true;
	}

	float Ease(float x){
		float a = easeAmount + 1;
		return Mathf.Pow (x, a) / (Mathf.Pow (x, a) + Mathf.Pow (1 - x, a));
	}

	//Return new velocity based on distance between two waypoints
	Vector3 CalculateTrapMovement(float speed){

		if (Time.time < nextMoveTime) {
			return Vector3.zero;
		}

		fromWaypointIndex %= globalWaypoints.Length;
		int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
		//distance between from waypoint and to waypoint
		float distanceBetweenWaypoints = Vector3.Distance (globalWaypoints [fromWaypointIndex], globalWaypoints [toWaypointIndex]);
		//get the percent the object is between those two waypoints using the speed and deltatime
		percentBetweenWaypoints += Time.deltaTime * speed / distanceBetweenWaypoints;
		percentBetweenWaypoints = Mathf.Clamp01 (percentBetweenWaypoints);
		float easedPercentBetweenWaypoints = Ease (percentBetweenWaypoints);

		//get a new position for the object for the next frame
		Vector3 newPos = Vector3.Lerp (globalWaypoints [fromWaypointIndex], globalWaypoints [toWaypointIndex], easedPercentBetweenWaypoints);
		if (percentBetweenWaypoints >= 1) {
			percentBetweenWaypoints = 0;
			fromWaypointIndex++;
			if (fromWaypointIndex >= globalWaypoints.Length - 1) {
				fromWaypointIndex = 0;
			}
			nextMoveTime = Time.time;
		}
		//return the velocity to get to the new position
		return newPos - transform.position;
	}

	//draw in the debug window so that we can see where the waypoints are in the game world
	void OnDrawGizmos(){
		if (localWaypoints != null) {
			Gizmos.color = Color.red;
			float size = .3f;

			for (int i = 0; i < localWaypoints.Length; i++) {
				Vector3 globalWaypointPos = (Application.isPlaying)?globalWaypoints[i] : localWaypoints [i] + transform.position;
				Gizmos.DrawLine (globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
				Gizmos.DrawLine (globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
			}
		}
	}
}
