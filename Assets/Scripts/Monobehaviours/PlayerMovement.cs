using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    //Strictly Internal player movement logic
    [SerializeField]
    private Vector2 position;
    [SerializeField]
    private bool canJump;
    [SerializeField]
    private bool jumping;
    [SerializeField]
    private bool falling;
    private int currentNumJump;    //which jump you are in for multi-jumps


    //Externally required variables
    //horizontal Movement Variables
    public bool fluidMoveSpeedX = false;    //Use these 3 XOR the 2 below
    public float minFluidMoveSpeedX = 0.0f;
    public float maxFluidMoveSpeedX = 10f;

    public float[] moveSpeedsX;     //use these 2 XOR the 3 above
    public float[] accelorationsX;  //exclusively for end-cap accelorations/decelorations on nonfluid movespeed
                                    //might actually want to get rid of this...

    //Jump Variables
    public bool fluidJumpHeight = false;     //Use these 3 XOR the 2 below
    public float minFluidJumpHeight = 0.01f;
    public float maxFluidJumpHeight = 10f;

    public float[] jumpHeihghts;     //use these 2 XOR the 3 above
    public float[] jumpAccelorations;

    //Falling/Gravity Variables
    public bool useDefaultGravity = true;   //Use this XOR the next 2

    public float[] fallSpeeds;              //Use these XOR useDefaultGravity
    public float[] fallAccelorations;

    public bool useMaxFallHeight;
    public float maxFallHeight = 100f;

    //Misc. Jump/manuver Variables
    public int numJumps = 1;
    public bool jumpForgiveness = true;
    public bool canWallJump = false;
    CapturePlayerMovement.Manuver[] otherManuvers;    //For things like attack/enemy-based movements, air-dashes, ground-pounds, grapple-hooks, etc.
    
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetButtonDown("Jump")) {
            Jump();
        }
        if(Input.GetButtonDown("Horizontal")) {
            MoveHorizontal();
        }

	}

    void Jump() {
        
        //insert logic to check/set canJump here

        if(canJump) {
            
            if(fluidJumpHeight) {   //use minFluidJumpHeight & maxFluidJumpHeight here



            }
            else {  //use JumpHeights & JumpAcceloratoins here
                


            }


        }
    }

    void MoveHorizontal() {

    }


    void Perform(CapturePlayerMovement.Manuver manuver) {

    } 
}
