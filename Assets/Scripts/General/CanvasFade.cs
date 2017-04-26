using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	
	public Image fadeImage;
	public float fadeSpeed = .5f;
	public Color c0;
	public Color c1;
	public Color cDefault;


	bool fading;
	int fadeDir;
	float alpha;

	// Update is called once per frame
	void Update(){
		
		if (fading) {
			Color temp = fadeImage.color;
			temp.a += fadeDir * fadeSpeed * Time.deltaTime;
			temp.a = Mathf.Clamp01 (temp.a);
			fadeImage.color = temp;
			if (temp.a == 1 && fadeDir == 1) {
				fading = false;

			} else if (temp.a == 0 && fadeDir == -1) {
				fading = false;
			}
		}

	}

	public void SetFade(int dir){
		fading = true;
		fadeDir = dir;
	}


}
