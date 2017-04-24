using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RaycastController : MonoBehaviour {

	public const float skinWidth = .03f;
	public int horizontalRayCount = 4;
	public int verticalRayCount = 4;

	public LayerMask collisionMask;
	public LayerMask trapMask;

	[HideInInspector]
	public float horizontalRaySpacing;
	[HideInInspector]
	public float verticalRaySpacing;

	public BoxCollider2D collider;
	public RaycastOrigins raycastOrigins;
	public GameObject[] corners;

	public virtual void Awake() {
		collider = GetComponent<BoxCollider2D> ();
		UpdateColliderOrigins ();
	}

	public virtual void Start(){
		CalculateRaySpacing ();
	}

	public void UpdateRaycastOrigins() {
		raycastOrigins.bottomLeft = new Vector2 (corners[0].transform.position.x , corners[0].transform.position.y );
		raycastOrigins.bottomRight = new Vector2 (corners[1].transform.position.x , corners[1].transform.position.y);
		raycastOrigins.topLeft = new Vector2 (corners[2].transform.position.x , corners[2].transform.position.y);
		raycastOrigins.topRight = new Vector2 (corners[3].transform.position.x, corners[3].transform.position.y );
		raycastOrigins.bottomLeftInner = new Vector2 (corners[4].transform.position.x , corners[4].transform.position.y );
		raycastOrigins.bottomRightInner = new Vector2 (corners[5].transform.position.x , corners[5].transform.position.y);
		raycastOrigins.bottomUpper = new Vector2 (corners[6].transform.position.x , corners[6].transform.position.y);
		raycastOrigins.middleRight = new Vector2 (corners[7].transform.position.x , corners[7].transform.position.y);
	}

	public void UpdateColliderOrigins(){
		float colX = collider.bounds.extents.x;
		float colY = collider.bounds.extents.y;
		corners[0].transform.position = new Vector2 (collider.transform.position.x - colX, collider.transform.position.y - colY);
		corners[1].transform.position = new Vector2 (collider.transform.position.x + colX, collider.transform.position.y - colY);
		corners[2].transform.position = new Vector2 (collider.transform.position.x - colX, collider.transform.position.y + colY);
		corners[3].transform.position = new Vector2 (collider.transform.position.x + colX, collider.transform.position.y + colY);
		corners[4].transform.position = new Vector2 (collider.transform.position.x - colX + .2f, collider.transform.position.y - colY);
		corners[5].transform.position = new Vector2 (collider.transform.position.x + colX - .2f, collider.transform.position.y - colY);
		corners[6].transform.position = new Vector2 (collider.transform.position.x - colX, collider.transform.position.y - colY + .1f);
		corners[7].transform.position = new Vector2 (collider.transform.position.x + colX, collider.transform.position.y);
	}

	public void CalculateRaySpacing() {
		Bounds bounds = collider.bounds;
		bounds.Expand (skinWidth * -2);

		horizontalRayCount = Mathf.Clamp (horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp (verticalRayCount, 2, int.MaxValue);

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}

	public struct RaycastOrigins {
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
		public Vector2 bottomLeftInner, bottomRightInner;
		public Vector2 bottomUpper, middleRight;
	}
}
