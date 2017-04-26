using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Female : MonoBehaviour {

	public Text text;
	public Image endImage;
	public SpriteRenderer blackSprite;
	public float fadeSpeed = .5f;
	public Color c1;
	public Color cDefault;

	bool fading;
	int fadeDir;
	float alpha;
	// Update is called once per frame
	void Update(){
		if (fading) {
			Color temp = blackSprite.color;
			temp.a += fadeDir * fadeSpeed * Time.deltaTime;
			temp.a = Mathf.Clamp01 (temp.a);
			blackSprite.color = temp;
			if (temp.a == 1 && fadeDir == 1) {
				fading = false;
				text.enabled = true;
				endImage.enabled = true;
			} 
		}

	}

	public void SetFade(int dir){
		fading = true;
		fadeDir = dir;
	}


	void OnTriggerEnter2D(Collider2D col){
		if(col.tag == "Player") {
			SetFade (1);
		}
	}
}
