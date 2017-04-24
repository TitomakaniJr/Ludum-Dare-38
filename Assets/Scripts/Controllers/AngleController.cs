using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleController : MonoBehaviour {

	public Vector3 GetAngle(Vector3 pos, Planet thisPlanet){
		float a = pos.x - thisPlanet.transform.position.x;
		float b = pos.y - thisPlanet.transform.position.y;
		float distanceToPlanet = Mathf.Sqrt (a * a + b * b);
		float angle = Mathf.Rad2Deg * Mathf.Acos (((distanceToPlanet * distanceToPlanet) + (a * a) - (b * b)) / (2 * distanceToPlanet * a));
		if (transform.position.y > thisPlanet.transform.position.y) {
			angle -= 90;
		} else {
			angle = angle - 90;
		}
		return new Vector3 (0, 0, angle);
	}
}
