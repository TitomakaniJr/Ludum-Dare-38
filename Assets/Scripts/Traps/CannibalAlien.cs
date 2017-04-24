using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannibalAlien : MonoBehaviour {

	public float speed;
	public bool cyclic;
	public float waitTime;
	[Range(0,2)]
	public float easeAmount;
	public float shootTime;
	public GameObject projectile;
	public Vector3[] localWaypoints;
	public Vector3[] swipeWaypoints;
	public Vector3[] strafeWaypoints;
	public Planet myPlanet;

	int fromWaypointIndex;
	int faceDir;
	float percentBetweenWaypoints;
	float nextMoveTime;
	float shootTimer;
	float shootSpeed;
	float shootSpread;
	float funcTime;
	bool inDefault;
	bool inFunc;
	bool movingToDefault;
	Vector3 barrel_0;
	Vector3 barrel_1;
	Vector3 barrel_2;
	Vector3 defaultPos;
	Vector3[] globalWaypoints;
	Vector3[] swipWayPoints;
	AngleController angCont;

	void Start () {
		angCont = FindObjectOfType<AngleController> ();
		transform.localEulerAngles = angCont.GetAngle (transform.position, myPlanet);
		barrel_0 = transform.Find ("Barrel_0").transform.position;
		barrel_1 = transform.Find ("Barrel_1").transform.position;
		barrel_2 = transform.Find ("Barrel_2").transform.position;
		defaultPos = transform.position;
		inDefault = true;
		inFunc = false;
		globalWaypoints = new Vector3[localWaypoints.Length];
		for (int i = 0; i < localWaypoints.Length; i++) {
			globalWaypoints [i] = localWaypoints [i] + transform.position;
		}
	}
		
	void Update () {
		if (!inFunc && !movingToDefault) {
			StartCoroutine (MoveToDefault ());
			if (inDefault) {
				StartCoroutine (StationaryShoot ());
			}
		}
	}

	IEnumerator MoveToDefault(){
		movingToDefault = true;
		globalWaypoints = new Vector3[2];
		globalWaypoints [0] = transform.position;
		globalWaypoints [1] = defaultPos;
		Vector3 velocity = CalculateTrapMovement();
		transform.localEulerAngles = angCont.GetAngle (transform.position, myPlanet);
		transform.Translate (velocity, Space.World);
		if (transform.position == globalWaypoints [1]) {
			movingToDefault = false;
			inDefault = true;
			yield return null;
		}
	}

	IEnumerator StationaryShoot(){
		inFunc = true;
		print ("INFUNC");
		float spacing = Mathf.Sin(Time.time * (shootSpeed / shootSpread)) * shootSpread;
		Quaternion rotation;
		funcTime = 60 * 5;
		while(funcTime > 0) {
			funcTime -= Time.deltaTime;
			rotation = Quaternion.Euler (0, 0, Mathf.PingPong (Time.time * 10, 180));
			if (shootTimer > 0) {
				shootTimer -= Time.deltaTime;
			} else {
				Instantiate (projectile, barrel_0, rotation);
				Instantiate (projectile, barrel_1, rotation);
				Instantiate (projectile, barrel_2, rotation);
			}
		}
		inFunc = false;
		yield return null;
	}

	IEnumerator StrafeShoot(){
		inFunc = true;
		inFunc = false;
		yield return null;
	}

	IEnumerator Swipe(){
		inFunc = true;
		inFunc = false;
		yield return null;
	}

	IEnumerable GrabCube(){
		inFunc = true;
		inFunc = false;
		yield return null;
	}
		

	float Ease(float x){
		float a = easeAmount + 1;
		return Mathf.Pow (x, a) / (Mathf.Pow (x, a) + Mathf.Pow (1 - x, a));
	}

	//Return new velocity based on distance between two waypoints
	Vector3 CalculateTrapMovement(){

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
			if (!cyclic) {
				if (fromWaypointIndex >= globalWaypoints.Length - 1) {
					fromWaypointIndex = 0;
					System.Array.Reverse (globalWaypoints);
				}
			}
			nextMoveTime = Time.time + waitTime;
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
