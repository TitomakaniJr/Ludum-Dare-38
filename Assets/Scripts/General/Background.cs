using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour {
	public int width;
	public int height;
	public GameObject background;

	// Use this for initialization
	void Start () {
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				GameObject newBG = Instantiate (background, new Vector3 (transform.position.x + (30 * i), transform.position.y + (30 * j), 0), transform.rotation); 
				newBG.transform.SetParent(gameObject.transform);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
