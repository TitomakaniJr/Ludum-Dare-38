using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationaryTrap : MonoBehaviour {

	public bool horizontal;
	public bool flipY;
	public bool left;
	public Planet myPlanet;

	int faceDir;

	AngleController angCont;
	// Use this for initialization
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
	}
}
