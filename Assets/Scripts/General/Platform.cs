using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour {

	public bool horizontal;
	public Planet myPlanet;

	AngleController angCont;
	// Use this for initialization
	void Start () {
		angCont = FindObjectOfType<AngleController> ();
		if (horizontal) {
			transform.localEulerAngles = angCont.GetAngle (transform.position, myPlanet);
		} else {
			Vector3 newRot = new Vector3(0, 0, angCont.GetAngle (transform.position, myPlanet).z - 90);
			transform.localEulerAngles = newRot;
		}
	}
}
