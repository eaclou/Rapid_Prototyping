using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float playerSpeed = 1f;
    public float physicsSpeedMult = 5f;

    public MoveType moveType;
    public enum MoveType {
        Physics2D,
        Basic,
        UnityInput,
        MouseAim
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        

    }

    private void FixedUpdate() {
        //Movement();
    }
    
    public void Movement() {
        
        switch(moveType) {
            case MoveType.Basic:
                SetForPhysicsMove(false);
                MovementBasic();
                break;
            case MoveType.UnityInput:
                SetForPhysicsMove(false);
                MovementUnityInput();
                break;
            case MoveType.Physics2D:
                SetForPhysicsMove(true);
                MovementPhysics2D();
                break;
            case MoveType.MouseAim:
                SetForPhysicsMove(false);
                MovementMouseAim();
                break;
            default:

                break;
        }
    }

    private void SetForPhysicsMove(bool physicsOn) {
        if(physicsOn) {
            this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }
        else {
            this.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        }
    }

    private void MovementBasic() {
        if (Input.GetKey("up") || Input.GetKey("w")) {
            transform.Translate(0, playerSpeed * Time.deltaTime, 0);
        }
        if (Input.GetKey("down") || Input.GetKey("s"))
        {
            transform.Translate(0, -playerSpeed * Time.deltaTime, 0);
        }
        if (Input.GetKey("left") || Input.GetKey("a"))
        {
            transform.Translate(-playerSpeed * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey("right") || Input.GetKey("d"))
        {
            transform.Translate(playerSpeed * Time.deltaTime, 0, 0);
        }
    }
    private void MovementPhysics2D() {
        if (Input.GetKey("up") || Input.GetKey("w")) {
            this.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, playerSpeed * physicsSpeedMult * Time.deltaTime), ForceMode2D.Impulse);
        }
        if (Input.GetKey("down") || Input.GetKey("s")) {
            this.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, -playerSpeed * physicsSpeedMult * Time.deltaTime), ForceMode2D.Impulse);
        }
        if (Input.GetKey("left") || Input.GetKey("a")) {
            this.GetComponent<Rigidbody2D>().AddForce(new Vector2(-playerSpeed * physicsSpeedMult * Time.deltaTime, 0f), ForceMode2D.Impulse);
        }
        if (Input.GetKey("right") || Input.GetKey("d")) {
            this.GetComponent<Rigidbody2D>().AddForce(new Vector2(playerSpeed * physicsSpeedMult * Time.deltaTime, 0f), ForceMode2D.Impulse);
        }
    }
    private void MovementUnityInput() {
        transform.Translate(playerSpeed * Input.GetAxis("Horizontal") * Time.deltaTime, 0f, 0f);
        transform.Translate(0f, playerSpeed * Input.GetAxis("Vertical") * Time.deltaTime, 0f);
    }
    private void MovementMouseAim() {

    }
}
