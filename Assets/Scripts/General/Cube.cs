using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour {

	public Planet myPlanet;

	AngleController angCont;
	// Use this for initialization
	void Start () {
		angCont = FindObjectOfType<AngleController> ();
		transform.localEulerAngles = angCont.GetAngle (transform.position, myPlanet);
	}

}
