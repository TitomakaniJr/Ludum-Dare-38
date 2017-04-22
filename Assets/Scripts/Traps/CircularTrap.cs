using UnityEngine;
using System.Collections;

public class CircularTrap : MonoBehaviour {

	float angle = 0;
	public Vector3 center;
	public float speed;
	public float radius;
	public int faceDir;

	// Use this for initialization
	void Start () {
		speed = (2 * Mathf.PI) / speed;
		center += transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		angle += speed * Time.deltaTime * faceDir;
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
