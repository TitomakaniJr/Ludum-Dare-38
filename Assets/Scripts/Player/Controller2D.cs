using UnityEngine;
using System.Collections;

[RequireComponent (typeof (BoxCollider2D))]
public class Controller2D : RaycastController {

	public CollisionInfo collisions;
	[HideInInspector]
	public Vector2 playerInput;

	public override void Start() {
		base.Start();
		collisions.faceDir = 1;
	}

	public void Move(Vector3 velocity, bool standingOnPlatform){
		Move (velocity, Vector2.zero, standingOnPlatform);
	}

	public void Move(Vector3 velocity, Vector2 input, bool standingOnPlatform = false){
		UpdateRaycastOrigins ();
		collisions.Reset ();
		collisions.velocityOld = velocity;
		playerInput = input;

		TrapCollisions ();
		if (collisions.trapNext) {
			collisions.trap = true;
			collisions.trapNext = false;
		}
		if (!collisions.trap) {
			if (velocity.x != 0) {
				collisions.faceDir = (int)Mathf.Sign (velocity.x);
				transform.localScale = (new Vector3 (collisions.faceDir * 1, 1, 1));
			}

			HorizontalCollisions (ref velocity);

			if (velocity.y != 0) {
				VerticalCollisions (ref velocity);
			}

			transform.Translate (velocity);

			if (standingOnPlatform) {
				collisions.below = true;
			}
		}
	}

	//Check for collisions with moving traps
	void TrapCollisions (){
		float rayLength = 2 * skinWidth;

		//Check 
		for (int i = 0; i < horizontalRayCount + 1; i++) {
			Vector2 rayOriginLeft = raycastOrigins.bottomLeft;
			Vector2 rayOriginRight =  raycastOrigins.bottomRight;
			Vector2 tempDir = new Vector2 (raycastOrigins.bottomLeft.x - raycastOrigins.bottomRight.x, raycastOrigins.bottomLeft.y - raycastOrigins.bottomRight.y);
			Vector2 spacing = (((raycastOrigins.topRight - raycastOrigins.bottomRight) / horizontalRayCount) * i);
			rayOriginLeft += spacing;
			rayOriginRight += spacing;
			RaycastHit2D trapHitLeft = Physics2D.Raycast (rayOriginLeft, tempDir * -1, rayLength, trapMask);
			RaycastHit2D trapHitRight = Physics2D.Raycast (rayOriginRight, tempDir, rayLength, trapMask);
			Debug.DrawRay (rayOriginRight,tempDir * rayLength, Color.yellow);
			if (trapHitLeft || trapHitRight) {
				collisions.trapNext = true;
			}
		}

		for (int i = 0; i < verticalRayCount + 1; i++) {
			Vector2 rayOriginLeft = raycastOrigins.bottomLeftInner;
			Vector2 rayOriginRight =  raycastOrigins.topLeft;
			Vector2 tempDir = new Vector2 (raycastOrigins.bottomLeft.x - raycastOrigins.topLeft.x, raycastOrigins.bottomLeft.y - raycastOrigins.topLeft.y);
			Vector2 spacing = (((raycastOrigins.bottomRight - raycastOrigins.bottomLeft) / verticalRayCount) * i);
			rayOriginLeft += spacing;
			rayOriginRight += spacing;
			RaycastHit2D trapHitLeft = Physics2D.Raycast (rayOriginLeft, -tempDir, rayLength, trapMask);
			RaycastHit2D trapHitRight = Physics2D.Raycast (rayOriginRight, tempDir, rayLength, trapMask);
			Debug.DrawRay (rayOriginRight, tempDir * rayLength, Color.yellow);
			if (trapHitLeft || trapHitRight) {
				collisions.trapNext = true;
			}
		}
	}

	void HorizontalCollisions(ref Vector3 velocity){
		float directionX = collisions.faceDir;
		float rayLength = Mathf.Abs (velocity.x) + skinWidth;
		Vector2 rayOrigin;

		if (Mathf.Abs (velocity.x) < skinWidth) {
			rayLength = 2 * skinWidth;
		}

		for (int i = 0; i < horizontalRayCount; i++) {
			Vector2 tempDir;
			if (collisions.wallJump) {
				rayOrigin = (((raycastOrigins.topRight - raycastOrigins.bottomRight) / horizontalRayCount) * i) + -raycastOrigins.bottomRight;
			} else {
				rayOrigin = (((raycastOrigins.topRight - raycastOrigins.bottomRight) / horizontalRayCount) * i) + raycastOrigins.bottomRight;
			}
			if (directionX == 1) {
				tempDir = new Vector2 (raycastOrigins.bottomLeft.x - raycastOrigins.bottomRight.x, raycastOrigins.bottomLeft.y - raycastOrigins.bottomRight.y);
			} else {
				tempDir = new Vector2 (raycastOrigins.bottomRight.x - raycastOrigins.bottomLeft.x, raycastOrigins.bottomRight.y - raycastOrigins.bottomLeft.y);
			}
			RaycastHit2D trapHit = Physics2D.Raycast (rayOrigin, tempDir * -directionX, rayLength, trapMask);
			if (trapHit) {
				collisions.trapNext = true;
			}
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, tempDir * -directionX, rayLength, collisionMask);
			Debug.DrawRay (rayOrigin, tempDir * -directionX * rayLength, Color.green);

			RaycastHit2D secondHit = Physics2D.Raycast (raycastOrigins.bottomUpper, tempDir * directionX, 0f, collisionMask);
			Debug.DrawRay (raycastOrigins.bottomUpper, tempDir *directionX * 0f, Color.red);

			RaycastHit2D thirdHit = Physics2D.Raycast (rayOrigin, tempDir * -directionX, 5, collisionMask);
			//Debug.DrawRay (rayOrigin, tempDir * -directionX * 5, Color.red);
			if (secondHit) {
			}

			if (hit || secondHit || thirdHit) {
				if(!hit){
					if (secondHit) {
						hit = secondHit;
						directionX = -directionX;
					}
				}

				if (collisions.wallTimer > 0) {
					collisions.wallTimer -= Time.deltaTime;
					continue;
				} 

				if (hit.distance == 0 && !secondHit && !thirdHit) {
					continue;
				}
					

				if (secondHit) {
					velocity.x = .1f * directionX;
					collisions.backwardsWallslide = true;
				} else if(hit && hit.distance == 0 && thirdHit){
 					velocity.x = .1f * - directionX;
				}else if(hit){
					velocity.x = (hit.distance - skinWidth) * directionX;
				}
				if (hit) {
					rayLength = hit.distance;
					collisions.left = directionX == -1;
					collisions.right = directionX == 1;
				}

			}
		}
		collisions.wallJump = false;
	}

	void VerticalCollisions(ref Vector3 velocity){
		float directionY = Mathf.Sign (velocity.y);
		float rayLength = Mathf.Abs (velocity.y) + skinWidth;
		for (int i = 0; i < verticalRayCount; i++) {
			Vector2 rayOrigin;
			Vector2 tempDir;
			if (directionY != 1) {
				rayOrigin = (i == 0) ? raycastOrigins.bottomLeftInner : raycastOrigins.bottomRightInner;
				tempDir = new Vector2 (raycastOrigins.bottomLeft.x - raycastOrigins.topLeft.x, raycastOrigins.bottomLeft.y - raycastOrigins.topLeft.y);
			} else {
				rayOrigin = (i == 0) ? raycastOrigins.topLeft : raycastOrigins.topRight;
				tempDir = new Vector2 (raycastOrigins.topLeft.x - raycastOrigins.bottomLeft.x, raycastOrigins.topLeft.y - raycastOrigins.bottomLeft.y);
			}
			RaycastHit2D trapHit = Physics2D.Raycast (rayOrigin, tempDir * directionY, rayLength, trapMask);
			if (trapHit) {
				collisions.trapNext = true;
			}
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, tempDir , rayLength, collisionMask);
			Debug.DrawRay (rayOrigin, tempDir * rayLength, Color.red);
			if (hit) {
				if (hit.collider.tag == "Through") {
					if (directionY == 1 || hit.distance == 0) {
						continue;
					}
					if (collisions.fallingThroughPlatform) {
						continue;
					}
					if (playerInput.y == -1) {
						collisions.fallingThroughPlatform = true;
						Invoke ("ResetFallingThroughPlatform", .5f);
						continue;
					}
				} 

				if(!collisions.wallSlide){
					velocity.y = (hit.distance - skinWidth) * directionY;
					rayLength = hit.distance;

					collisions.below = directionY == -1;
					collisions.above = directionY == 1;
				}
			}
		}

	}

	void ResetFallingThroughPlatform(){
		collisions.fallingThroughPlatform = false;
	}

	public struct CollisionInfo {
		public bool above, below;
		public bool left, right;
		public bool trapNext;
		public bool trap;
		public bool wallSlide;
		public bool wallJump;
		public bool backwardsWallslide;
		public float wallTimer;

		public float angle;
		public float a, b;
		public Vector3 velocityOld;
		public int faceDir;
		public bool fallingThroughPlatform;

		public void Reset(){
			above = below = false;
			left = right = false;
			trap = false;
			backwardsWallslide = false;
		}
	}
}