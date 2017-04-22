using UnityEngine;
using System.Collections;

public class ProjectileController : MonoBehaviour {

	float projectileSpeed;
	int faceDir;
	bool horizontal;
	// Use this for initialization
	void Start () {
		Invoke ("DestroyProjectile", 4);
	}

	public void SetSpeed(float speed){
		projectileSpeed = speed;
	}

	public void SetDirection(int dir, bool hor){
		faceDir = dir;
		horizontal = hor;
	}

	// Update is called once per frame
	void Update () {
		if (Mathf.Abs(projectileSpeed) > 0 ) {
			if (horizontal) {
				transform.position = new Vector3 (transform.position.x + (projectileSpeed * faceDir), transform.position.y, transform.position.z);
			} else {
				transform.position = new Vector3 (transform.position.x, transform.position.y + (projectileSpeed * faceDir), transform.position.z);
			}
		}
	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.tag == "Obstacle") {
			DestroyProjectile ();
		}
		else if(col.name == "Player") {
			print ("hit");
			Invoke ("DestroyProjectile", .05f);
		}
	}

	void DestroyProjectile(){
		GameObject.Destroy (gameObject);
	}
}