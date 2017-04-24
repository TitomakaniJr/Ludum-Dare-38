using UnityEngine;
using System.Collections;

public class CircularTrap : MonoBehaviour {

	public float speed;
	public float radius;
	public bool flipY;
	public bool left;
	public bool horizontal;
	public Vector3 center;
	public Planet myPlanet;

	int faceDir;
	float angle = 0;
 	AngleController angCont;
	SpriteRenderer spriteRend;

	// Use this for initialization
	void Start () {
		angCont = FindObjectOfType<AngleController> ();
		spriteRend = GetComponent<SpriteRenderer> ();
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
			transform.localScale = new Vector3 (transform.localScale.x, transform.localScale.y * -1, transform.localScale.z);
		} else {
			transform.localScale = new Vector3 (transform.localScale.x, transform.localScale.y, transform.localScale.z);
		}
		center += transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 diff = transform.position - center;
		if (horizontal) {
			if (!left) {
				if (diff.x < 0 && diff.y > 0 && spriteRend.flipX) {
					spriteRend.flipX = false;
				} else if (diff.x > 0 && diff.y < 0 && !spriteRend.flipX) {
					spriteRend.flipX = true;
				}
			} else {
				if (diff.x < 0 && diff.y < 0 && !spriteRend.flipX) {
					spriteRend.flipX = true;
				} else if (diff.x > 0 && diff.y > 0 && spriteRend.flipX) {
					spriteRend.flipX = false;
				}
			}
			transform.localEulerAngles = angCont.GetAngle (transform.position, myPlanet);
		} else {
			if (left) {
				if (diff.x < 0 && diff.y > 0 && !spriteRend.flipX) {
					spriteRend.flipX = true;
				} else if (diff.x > 0 && diff.y < 0 && spriteRend.flipX) {
					spriteRend.flipX = false;
				}
			} else {
				if (diff.x < 0 && diff.y < 0 && !spriteRend.flipX) {
					spriteRend.flipX = true;
				} else if (diff.x > 0 && diff.y > 0 && spriteRend.flipX) {
					spriteRend.flipX = false;
				}
			}
			Vector3 newRot = new Vector3(0, 0, angCont.GetAngle (transform.position, myPlanet).z - 90);
			transform.localEulerAngles = newRot;
		}
		angle += speed * Time.deltaTime * faceDir;
		if (angle > 10000) {
			angle = 0;
		}
		transform.position = new Vector3 (Mathf.Cos (angle) * radius + center.x,  Mathf.Sin (angle) * radius + center.y , 1);
	}

	void OnDrawGizmos(){
		Gizmos.color = Color.red;
		float size = .3f;
		Vector3 radiusVector = new Vector3 (0, radius, 0);
		Gizmos.DrawLine (center + transform.position - Vector3.up * size, center + transform.position + Vector3.up * size);
		Gizmos.DrawLine (center + transform.position - Vector3.left * size, center + transform.position + Vector3.left * size);

		Gizmos.DrawLine (center + transform.position + radiusVector - Vector3.up * size, center + transform.position + radiusVector + Vector3.up * size);
		Gizmos.DrawLine (center + transform.position + radiusVector - Vector3.left * size, center + transform.position + radiusVector + Vector3.left * size);
	}
}
