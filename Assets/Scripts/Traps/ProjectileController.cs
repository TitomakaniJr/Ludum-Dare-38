using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour {

	int faceDir;
	float projectileSpeed;
	bool horizontal;
	Planet myPlanet;

	// Use this for initialization
	void Start () {
		Invoke ("DestroyProjectile", 4);
	}
		
	// Update is called once per frame
	void Update () {
		if (Mathf.Abs(projectileSpeed) > 0 ) {
			if (horizontal) {
				float a = transform.position.x - myPlanet.transform.position.x;
				float b = transform.position.y - myPlanet.transform.position.y;
				float distanceToPlanet = Mathf.Sqrt (a * a + b * b);
				float angle = Mathf.Rad2Deg * Mathf.Acos (((distanceToPlanet * distanceToPlanet) + (a * a) - (b * b)) / (2 * distanceToPlanet * a));
				if (transform.position.y > myPlanet.transform.position.y) {
					angle -= 90;
				} else {
					angle = -angle - 90;
				}
				Vector3 newRotation = new Vector3 (0, 0, angle);
				transform.eulerAngles = newRotation;
				transform.Translate(new Vector3 (projectileSpeed * faceDir, 0, 0));
			} else {
				transform.Translate(new Vector3 (projectileSpeed * -faceDir, 0, 0));
			}
		}
	}

	public void SetValues(float speed, int dir, bool hor, Planet newPlanet){
		projectileSpeed = speed;
		faceDir = dir;
		horizontal = hor;
		myPlanet = newPlanet;
	}

	void OnTriggerEnter2D(Collider2D col){
 		if (col.tag == "Obstacle") {
			DestroyProjectile ();
		}
		else if(col.name == "Player") {
			Invoke ("DestroyProjectile", .05f);
		}
	}

	void DestroyProjectile(){
		GameObject.Destroy (gameObject);
	}
}