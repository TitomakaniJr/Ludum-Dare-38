using UnityEngine;
using System.Collections;

public class PixelCameraZoom : MonoBehaviour {

	Camera cam;
	// Use this for initialization
	void Start () {
		cam = GetComponent<Camera> ();
		cam.orthographicSize = (Screen.height / 22.5f) / 2f;
	}
}
