using UnityEngine;
using System.Collections;

public class ProjectileTrapController : MonoBehaviour {

	public float projectileSpawnRate;
	public float projectileSpeed;
	public float offset;
	public bool horizontal;
	public bool flipY;
	public bool left;
	public GameObject projectilePrefab;
	public Planet myPlanet;

	int faceDir;
	float spawnTimer;
	float offsetTimer;
	bool spawn;

	Vector3 projectSpawn;
	Animator anim;
	AngleController angCont;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		angCont = FindObjectOfType<AngleController> ();
		offsetTimer = offset;
		spawn = false;
		spawnTimer = projectileSpawnRate;
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
		projectSpawn = transform.FindChild ("ProjectSpawn").position;
	}


	void SpawnProjectile(){
		anim.SetTrigger ("Shoot");
		GameObject projectile;
		if (flipY) {
			projectile = Instantiate (projectilePrefab, new Vector3 (projectSpawn.x, projectSpawn.y, projectSpawn.z), gameObject.transform.rotation) as GameObject;
		} else {
			projectile = Instantiate (projectilePrefab, new Vector3 (projectSpawn.x, projectSpawn.y, projectSpawn.z), gameObject.transform.rotation) as GameObject;
		}
		ProjectileController projectileController = projectile.GetComponent<ProjectileController> ();
		projectileController.SetValues (projectileSpeed, faceDir, horizontal, myPlanet);
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