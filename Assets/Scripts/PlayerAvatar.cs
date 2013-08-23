using UnityEngine;
using System.Collections;

public class PlayerAvatar : CoreGameObject {	
	private float jumpSpeed = 500;
	private float movementSpeed = 250;
	
	private bool jump = false;
	private bool left = false;
	private bool undoleft = false;
	private bool right = false;
	private bool undoright = false;
	private bool down = false;
	

	// Use this for initialization
	public void Start() {
		coreType = "player";
	}
	// Update is called once per frame
	public void Update () {		
			
		if(Input.GetKeyDown(KeyCode.W)) {
			jump = true;
		}
		if(Input.GetKeyDown(KeyCode.S)) {
			down = true;
		}
		
		if(Input.GetKey(KeyCode.D)) {
			right = true;
		} else if(Input.GetKeyUp(KeyCode.D)) {
			undoright = true;
		}
		
		if(Input.GetKey(KeyCode.A)) {
			left = true;
		}
		if(Input.GetKeyUp(KeyCode.A)) {
			undoleft = true;
		}
	}
	
	// Update is called once per frame
	public void FixedUpdate () {
		Vector3 finalVelocity = new Vector3(0.0f, rigidbody.velocity.y, 0.0f);
		if(jump) {
			finalVelocity += new Vector3(0.0f, jumpSpeed, 0.0f);
			MasterGameController.Instance.PlayerJumped();
			jump = false;
		}
		if(down) {
			finalVelocity += new Vector3(0.0f, -jumpSpeed, 0.0f);	
			MasterGameController.Instance.PlayerDashed();
			down = false;
		}
		if(undoleft) {
			left = false;
			finalVelocity -= new Vector3(-movementSpeed, 0.0f, 0.0f);
			undoleft = false;
		}
		if(left) {
			finalVelocity += new Vector3(-movementSpeed, 0.0f, 0.0f);
		}		
		
		if(undoright) {
			right = false;
			undoright = false;
			finalVelocity -= new Vector3(movementSpeed, 0.0f, 0.0f);
			
		}
		if(right) {
			finalVelocity += new Vector3(movementSpeed, 0.0f, 0.0f);
		}
		rigidbody.velocity = finalVelocity;
		
		
	}
}
