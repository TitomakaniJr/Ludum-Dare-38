using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovingTrap : MonoBehaviour {

	public Vector3[] localWaypoints;
	public Planet myPlanet;
	Vector3[] globalWaypoints;

	public float speed;
	public bool cyclic;
	public bool flipY;
	public bool left;
	public bool horizontal;
	public float waitTime;
	[Range(0,2)]
	public float easeAmount;

	int fromWaypointIndex;
	int faceDir;
	float percentBetweenWaypoints;
	float nextMoveTime;
	AngleController angCont;

	void Start () {
		angCont = FindObjectOfType<AngleController> ();
		if (left) {
			faceDir = -1;
		} else {
			faceDir = 1;
		}
		if (horizontal) {
			transform.localEulerAngles = angCont.GetAngle (transform.position, myPlanet);
		} else {
			Vector3 newRot = new Vector3(0, 0, angCont.GetAngle (transform.position, myPlanet).z - 90);
			transform.localEulerAngles = newRot;
		}
		if (flipY) {
			transform.localScale = new Vector3 (transform.localScale.x * faceDir, transform.localScale.y * -1, transform.localScale.z);
		} else {
			transform.localScale = new Vector3 (transform.localScale.x * faceDir, transform.localScale.y, transform.localScale.z);
		}

		globalWaypoints = new Vector3[localWaypoints.Length];
		for (int i = 0; i < localWaypoints.Length; i++) {
			globalWaypoints [i] = localWaypoints [i] + transform.position;
		}
	}
		
	void Update () {
		Vector3 velocity = CalculateTrapMovement();
		if (horizontal) {
			transform.localEulerAngles = angCont.GetAngle (transform.position, myPlanet);
		} else {
			Vector3 newRot = new Vector3(0, 0, angCont.GetAngle (transform.position, myPlanet).z - 90);
			transform.localEulerAngles = newRot;
		}
		if ((velocity.x > 0 && faceDir == -1) || (velocity.x < 0 && faceDir == 1)) {
			faceDir = -faceDir;
			transform.localScale = new Vector3 (transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
		} 
		transform.Translate (velocity, Space.World);
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