using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour {

	public float maxJumpHeight = 3.75f;
	public float minJumpHeight = 1;
	public float minSlideSpeed = 2;
	public float upwardsWallSlow = .85f;
	public float timeToJumpApex = .5f;
	public float ceilingSlideSlowDown = .5f;
	public float timeBeforeslideSlowDown = .5f;
	public float timeBeforeCeilingSlowDown = .275f;
	public float wallSlideSpeedMax = .5f;
	public float wallStickTime = .2f;
	public float ceilingSlideTimer = .15f;
	public float minGrav = -5f;

	public float accelerationTimeAirborne = .22f;
	float accelerationTimeGrounded = .1f;
	float planetSwapTime = 1f;
	float planetSwapTimer = -1;
	public float accelerationTimeWall = .32f;
	float moveSpeed = 7;
	float sprintSpeed = 11;
	float gravity;
	float timeToWallUnstick;
	float jumpTime;
	//Time after ceiling collisions play can still ceiling Slide
	float timeForCeilingSlide;
	//Time before player speed starts to slow when attached to ceiling
	float timeForCeilingSlow;
	float timeForSlideSlow;
	float maxJumpVelocity;
	float minJumpVelocity;
	float velocityXSmoothing;
	float velocityYSmoothing;
	//Time player has in air to jump
	float airJumpTime = .25f;
	float planetBasedRotation;
	float distanceToPlanet;

	bool canJump;
	bool ceilingSlide;
	bool canCeilingSlide;
	bool slide;
	bool sprinting;
	bool wallUnstuck;

	public Vector2 wallJumpClimb;
	public Vector2 wallJumpOff;
	public Vector2 wallLeap;

	Vector3 velocity;
	Controller2D controller;
	Animator anim;
	PlayerManager playerManager;
	Planet[] planets;
	Planet standPlanet;
	SpriteRenderer spriteRend;

	void Start(){
		planets = FindObjectsOfType<Planet> ();
		controller = GetComponent<Controller2D> ();
		anim = GetComponent<Animator> ();
		spriteRend = GetComponent<SpriteRenderer> ();
		playerManager = FindObjectOfType<PlayerManager> ();
		distanceToPlanet = 1;
		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity * timeToJumpApex);
		minJumpVelocity = Mathf.Sqrt(2*Mathf.Abs(gravity) * minJumpHeight);
	}

	void LateUpdate(){
		if (gameObject.name != "Player") {
			gameObject.name = "Player";
		}
	}

	void Update(){
		FindStrongestPlanet ();
		gravity = GetGravity ();
		if(standPlanet != null){
			float a = transform.position.x - standPlanet.transform.position.x;
			float b = transform.position.y - standPlanet.transform.position.y;
			controller.collisions.a = a;
			controller.collisions.b = b;
			distanceToPlanet = Mathf.Sqrt (a * a + b * b);
			float angle = Mathf.Rad2Deg * Mathf.Acos (((distanceToPlanet * distanceToPlanet) + (a * a) - (b * b)) / (2 * distanceToPlanet * a));
			if (transform.position.y > standPlanet.transform.position.y) {
				angle -= 90;
				if (planetSwapTimer > 0) {
					angle = Mathf.LerpAngle (transform.localEulerAngles.z, angle, Time.deltaTime * 4);
					if (Mathf.Abs(transform.localEulerAngles.z - angle) < .1f) {
						planetSwapTimer = -1;
					}
				}
			} else {
				angle = -angle - 90;
				if (planetSwapTimer > 0) {
					angle = Mathf.LerpAngle (transform.localEulerAngles.z, angle, Time.deltaTime * 4);
					if (Mathf.Abs(transform.localEulerAngles.z - angle) < .1f) {
						planetSwapTimer = -1;
					}
				}
			}
			Vector3 newRotation = new Vector3 (0, 0, angle);
			transform.eulerAngles = newRotation;
			controller.collisions.angle = angle;
		}
			
		//Check For Sprint
		if (Input.GetKeyDown (KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftShift)) {
			sprinting = true;
		} else {
			sprinting = false;
		}

		//Get User Input
		Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));
		int wallDirX = (controller.collisions.left) ? -1 : 1;

		if (!ceilingSlide && !slide) {
			float targetVelocityX = input.x * ((sprinting) ? sprintSpeed : moveSpeed);
			//Smooth horizontal movement
			velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
		}

		bool wallSliding = false; 
		controller.collisions.wallSlide = false;

		WallSlideCheck (ref wallSliding, input, wallDirX);

		if (!wallSliding) {
			if (spriteRend.flipX) {
				spriteRend.flipX = false;
			}
		}

		if (!ceilingSlide && !slide) {
			if (!wallSliding) {
				if (input.x != 0) {
					if (!anim.GetBool ("Jumping") && !anim.GetBool ("DoubleJump")) {
						anim.SetBool ("Moving", true);
					}
				} else {
					anim.SetBool ("Moving", false);
				}
			}
		}

		//Check if we are on ground
		if (!controller.collisions.below) {
			slide = false;
			//Allow a small window where we can still jump after leaving ground
			if (jumpTime > 0) {
				jumpTime -= Time.deltaTime;
			} 
			else if(!wallSliding) {
				canJump = false;
			}
		
			if (controller.collisions.above && !ceilingSlide) {
				canCeilingSlide = true;
				timeForCeilingSlide= ceilingSlideTimer;
				if (timeForCeilingSlow <= 0) {
					timeForCeilingSlow = timeBeforeCeilingSlowDown;
				}
			}

			if (timeForCeilingSlide > 0 && canCeilingSlide) {
				timeForCeilingSlide -= Time.deltaTime;
			} else {
				canCeilingSlide = false;
			}
		} else {	//We are on the ground
			if (anim.GetBool ("Falling")) {
				if (jumpTime < 0) {
					anim.SetTrigger ("Land");
				}
				anim.SetBool ("Falling", false);
			}
			canJump = true;
			playerManager.playerAbilities.canDoubleJump = true;
			jumpTime = airJumpTime;
			if (input.y == -1 && (Mathf.Abs(velocity.x) > minSlideSpeed || slide)) {	//Check for slide input
				if (Mathf.Abs (velocity.x) > 5f) {
					slide = true;
					if (timeForSlideSlow > 0) {
						timeForSlideSlow -= Time.deltaTime;
					} else {
						if (Mathf.Abs (velocity.x) < .75f) {
							velocity.x = Mathf.SmoothDamp (velocity.x, 0, ref velocityXSmoothing, accelerationTimeGrounded);
						} else {
							velocity.x = Mathf.SmoothDamp (velocity.x, velocity.x * ceilingSlideSlowDown, ref velocityXSmoothing, accelerationTimeGrounded);
						}
					}
				} else if(slide) {
					velocity.x = 0;
				}
					 
			} else {
				slide = false;
				timeForSlideSlow = timeBeforeslideSlowDown;
			}
		}

		//Jumping
		if (Input.GetKeyDown (KeyCode.Space)) {
			slide = false;
			if (wallSliding) {
				//STOP WALL CLING ANIM HERE
				controller.collisions.wallTimer = .03f;
				controller.collisions.wallJump = true;
				if (wallDirX == input.x) {
					velocity.x = -wallDirX * wallJumpClimb.x;
				} else if (input.x == 0) {
					velocity.x = -wallDirX * wallJumpOff.x;
				} else {
					velocity.x = -wallDirX * wallLeap.x;
				}
				wallSliding = false;
				anim.SetBool ("Wallslide", false);
				if (spriteRend.flipX) {
					spriteRend.flipX = false;
				}
			}
			if (controller.collisions.below || canJump || playerManager.playerAbilities.canDoubleJump) {
				if (!canJump) {
					anim.SetBool ("DoubleJump", true);
					anim.SetBool ("Jumping", false);
					playerManager.playerAbilities.canDoubleJump = false;
				} else {
					anim.SetBool ("Jumping", true);
				}
				anim.SetBool ("Moving", false);
				anim.SetBool ("Falling", false);
				velocity.y = maxJumpVelocity;
				canJump = false;
			}
		}
		if (Input.GetKeyUp (KeyCode.Space)) {
			if (velocity.y > minJumpVelocity) {
				velocity.y = minJumpVelocity;
			}
		}
		 
		//Ceiling Slide
		if (input.y == 1 && (canCeilingSlide || ceilingSlide) && Mathf.Abs(velocity.x) > .75f) {
			if (controller.collisions.above) {
				ceilingSlide = true;
				velocity.y = .1f;
				if (timeForCeilingSlow > 0) {
					timeForCeilingSlow -= Time.deltaTime;
				} else {
					velocity.x = Mathf.SmoothDamp (velocity.x, velocity.x * ceilingSlideSlowDown, ref velocityXSmoothing, accelerationTimeGrounded);
				}
			} else{
				velocity.y = 0;
				ceilingSlide = false;
				timeForCeilingSlow = timeBeforeCeilingSlowDown; 
			}
		} else {
			if (!wallSliding) {
				velocity.y += gravity * Time.deltaTime;
				if (!controller.collisions.below) {
					if (velocity.y < 0) {
						if (jumpTime < 0) {
							anim.SetBool ("Falling", true);
							anim.SetBool ("Jumping", false);
							anim.SetBool ("DoubleJump", false);
						}
					} else if (velocity.y > 0) {
						if(!anim.GetBool("DoubleJump") && !anim.GetBool("Jumping")){
							anim.SetBool("Jumping", true);
						}
					}

				}
			} else {
				velocity.y += gravity * Time.deltaTime * upwardsWallSlow;
			}
			ceilingSlide = false;
		}
			
		//Set our animation for this frame
		//UNCOMMENT WHEN ANIMATIONS ARE IN!!!
		//anim.SetBool ("sliding", slide);
		//anim.SetBool ("CeilingSlide", ceilingSlide);


		//Move our transform
		controller.Move (velocity * Time.deltaTime, input);

		//Player hit a trap, we must die
		if (controller.collisions.trap) {
			playerManager.Die ();
		}

		//Stop velocity on ground
		if (controller.collisions.above || controller.collisions.below) {
			velocity.y = 0;
		}
	}

	void WallSlideCheck(ref bool wallSliding, Vector2 input, int wallDirX){
		if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below || controller.collisions.backwardsWallslide) {
			if (controller.collisions.backwardsWallslide) {
				controller.collisions.faceDir = -controller.collisions.faceDir;
				transform.localScale = new Vector3 (1 * controller.collisions.faceDir, 1, 1);
			} 

			if (!spriteRend.flipX && !wallUnstuck) {
				spriteRend.flipX = true;
			} else if (wallUnstuck) {
				wallUnstuck = false;
			}
			wallSliding = true;
			anim.SetBool ("Wallslide", true);
			anim.SetBool ("Falling", false);
			anim.SetBool ("Jumping", false);
			anim.SetBool ("DoubleJump", false);
			controller.collisions.wallSlide = true;
			canJump = true;
			//Reset DoubleJump on walls
			playerManager.playerAbilities.canDoubleJump = true;

			if (velocity.y < -wallSlideSpeedMax) {
				velocity.y = Mathf.SmoothDamp (velocity.y, -wallSlideSpeedMax, ref velocityYSmoothing, accelerationTimeWall);
			}

			if (timeToWallUnstick > 0) {
				velocityXSmoothing = 0;
				velocity.x = 0;

				if (input.x != wallDirX && input.x != 0) {
					timeToWallUnstick -= Time.deltaTime;
				} else {
					timeToWallUnstick = wallStickTime;
				}
			} else {
				wallSliding = false;
				wallUnstuck = true;
				timeToWallUnstick = wallStickTime;
			}
		} else if(anim.GetBool("Wallslide"))  {
			anim.SetBool ("Wallslide", false);
		}
	}

	void FindStrongestPlanet(){
		Planet tempPlanet = standPlanet;
		float topForce = 0;
		float tempForce;
		if (planetSwapTimer > 0) {
			planetSwapTimer -= Time.deltaTime;
		} else {
			for (int i = 0; i < planets.Length; i++) {
				float distance = Vector2.Distance (new Vector2 (transform.position.x, transform.position.y), 
                		new Vector2 (planets [i].transform.position.x, planets [i].transform.position.y));
				tempForce = planets [i].gravityConst * (planets [i].mass / Mathf.Pow (distance, 2));
				if (i == 0) {
					tempPlanet = planets [i];
				}
				if (distance < 8) {
					tempPlanet = planets [i];
					topForce = tempForce;
					break;
				}
				if (Mathf.Abs (tempForce) > Mathf.Abs (topForce)) {
					tempPlanet = planets [i];
					topForce = tempForce;
				}
			}
			if ((standPlanet != tempPlanet && topForce < minGrav) || standPlanet == null) {
				print(topForce + " " + minGrav);
				standPlanet = tempPlanet;
				gravity = topForce;
				planetSwapTimer = planetSwapTime;
				velocity.y = 0;
			}
		}
	}

	float GetGravity(){
		float distance = Vector2.Distance (new Vector2 (transform.position.x, transform.position.y), 
			new Vector2 (standPlanet.transform.position.x, standPlanet.transform.position.y));
		float tempGrav = standPlanet.gravityConst * (standPlanet.mass / (Mathf.Pow (distance, 2) * 1.2f));
		if (tempGrav > minGrav) {
			return minGrav;
		} else {
			return tempGrav;
		}
	}
}