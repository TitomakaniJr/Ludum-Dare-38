using UnityEngine;
using System.Collections;

public class ProjectileTrapController : MonoBehaviour {

	public float projectileSpawnRate;
	public float projectileSpeed;
	public float offset;
	public int faceDir;
	public bool horizontal;
	public GameObject projectilePrefab;

	float spawnTimer;
	float offsetTimer;

	bool spawn;
	// Use this for initialization
	void Start () {
		offsetTimer = offset;
		spawn = false;
		spawnTimer = projectileSpawnRate;
		if (horizontal) {
			transform.localEulerAngles = new Vector3 (transform.rotation.x, transform.rotation.y, transform.rotation.z - 90);
		}
		transform.localScale = new Vector3 (transform.localScale.x, transform.localScale.y * faceDir, transform.localScale.z);
	}


	void SpawnProjectile(){
		GameObject projectile = Instantiate (projectilePrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z + .1f), gameObject.transform.rotation) as GameObject;
		ProjectileController projectileController = projectile.GetComponent<ProjectileController> ();
		projectileController.SetDirection (faceDir, horizontal);
		projectileController.SetSpeed (projectileSpeed);
	}

	// Update is called once per frame
	void Update () {
		if (offsetTimer > 0) {
			offsetTimer -= Time.deltaTime;
		} else {
			if (!spawn) {
				SpawnProjectile ();
			}
			spawn = true;
		}
		if(spawn){
			if (spawnTimer > 0) {
				spawnTimer -= Time.deltaTime;
			} else {
				spawnTimer = projectileSpawnRate;
				SpawnProjectile ();
			}
		}

	}
}