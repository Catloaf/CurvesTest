using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapturePlayerMovement : MonoBehaviour {
    public GameObject player;   //The GameObject representing the player that this all corresponds to
    public float timeStep;      //How much time to move
    List<Manuver> actions;

        //horizontal Movement Variables
    public bool fluidMoveSpeedX = false;    //Use these 3 XOR the 2 below
    public float minFluidMoveSpeedX = 0.0f;
    public float maxFluidMoveSpeedX = 10f;

    public float[] moveSpeedsX;     //use these 2 XOR the 3 above
    public float[] accelorationsX;

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
    Manuver[] otherManuvers;    //For things like attack/enemy-based movements, air-dashes, ground-pounds, grapple-hooks, etc.
    
    void Start () {
        //TODO: GetPlayerGameObject();
        
        if(fluidMoveSpeedX) {   //YOU WILL MESS THINGS UP IF YOU USE BOTH!
            moveSpeedsX = null;
            accelorationsX = null;
        }
        else {
            minFluidMoveSpeedX = -1f;
            maxFluidMoveSpeedX = -1f;
        }
        if(fluidJumpHeight) {
            jumpHeihghts = null;
            jumpAccelorations = null;
        }
        else {
            minFluidJumpHeight = -1f;
            maxFluidJumpHeight = -1f;
        }
        if(!useMaxFallHeight) {
            maxFallHeight = -1;
        }
        
        //TODO: Set initial manuvers

	}
	
	void Update () {    //Don't think I'll be using Update() but not deleting it outright... yet.
		
	}

    public void CalculateMovementRange() { 
        //Think I need to do something about manuvers being inaccessable outside of the class and all that crap...





    }

    public void CalculateMovementRange(float time) {

    }

    public void AddAction(string action) {
        //TODO: HUGE switch case on different kinds of predefined manuvers
        action = action.ToLower();  //not sure if these return void/if the output is stored as the input, thus the possible redundency
        action = action.Trim();
        switch(action) {
            case "idle":
                //XY-Position same going in as exiting
                //Z-position some higher value
                //requires to not be falling, so some sort of rigidbody/collider beneath it

                break;
            case "walk":

                break;
            case "run":

                break;
            case "fall":

                break;
            case "jump":

                break;
            case "walljump":

                break;

            //there WILL be more cases there
            default:
                break;
        }
         //Then add the selected one to 'actions'
    }

        //Gets/Sets     ...Might not REALLY be needed for most of these things as they're all currently public variables, but they might not always be
    public void SetMoveSpeedX(float min, float max) {
        fluidMoveSpeedX = true;
        minFluidMoveSpeedX = min;
        maxFluidMoveSpeedX = max;
    }

    public void SetMoveSpeedX(float[] speeds) {
        fluidMoveSpeedX = false;
        moveSpeedsX = speeds;
    }

    public void SetJumpHeight(float min, float max) {
        fluidJumpHeight = true;
        minFluidJumpHeight = min;
        maxFluidJumpHeight = max;
    }

    public void SetJumpHeight(float[] heights) {
        fluidJumpHeight = false;
        jumpHeihghts = heights;
    }

    public void UseDefaultGravity() {
        useDefaultGravity = true;
    }

    public void SetFallSpeed(float[] speeds) {
        useDefaultGravity = false;
        fallSpeeds = speeds;
    }

    public void SetFallSpeed(float[] speeds, float[] accelorations) {
        useDefaultGravity = false;
        fallSpeeds = speeds;
        fallAccelorations = accelorations;
    }

    public void SetMaxFallHeight(float height) {
        if(height > 0f) {
            useMaxFallHeight = true;
            maxFallHeight = height;
        }
        else {
            useMaxFallHeight = false;
        }
    }

    public void SetNumJumps(int jumps) {
        if(jumps >= 1) {
            numJumps = jumps;
        }
        else {
            numJumps = 1;
        }
    }

    public void SetJumpForgiveness(bool forgive) {
        jumpForgiveness = forgive;
    }

    public void SetWallJump(bool canJump) {
        canWallJump = canJump;
    }

    //For things like attack/enemy-based movements, air-dashes, ground-pounds, grapple-hooks, etc.
    public struct Manuver {
        ManuverCondition mCondtions;    //conditions required to perform manuver
        Movement moveIn;    //movement into
        Movement moveOut;   //movement out of

        //special interactions... Will add this later.... hopefully
        //invincibility frames/time?

        Manuver(ManuverCondition mc, Movement mIn, Movement mOut) {
            mCondtions = mc;
            moveIn = mIn;
            moveOut = mOut;
        }
    }
    public struct Movement {  
        public Vector3 startPosition, endPosition;  //the Z-axis in the vector3s is for time

        public Movement(Vector3 start, Vector2 end, float time) {   //Keeping the z out of the 2nd Vector to make sure it's right
            startPosition = start;
            endPosition = new Vector3(end.x, end.y, startPosition.z + time);
        }
    }
    public struct ManuverCondition {
        public bool canPerform;
        public GameObject[] requiredGameObjectsIn;
        public GameObject[] requiredGameObjectsOut;
        public Vector3[] requiredGameObjectCoordinatesIn;
        public Vector3[] requiredGameObjectCoordinatesOut;

        public ManuverCondition(GameObject[] rGOsIn, GameObject[] rGOsOut, Vector3[] rGOCsIn, Vector3[] rGOCsOut) {
            canPerform = false;     //Will always default to false
            requiredGameObjectsIn = rGOsIn;
            requiredGameObjectsOut = rGOsOut;
            requiredGameObjectCoordinatesIn = rGOCsIn;
            requiredGameObjectCoordinatesOut = rGOCsOut;
        }

        public void SetManuverCondition(bool b) {
            canPerform = b;
        }
    }
}
