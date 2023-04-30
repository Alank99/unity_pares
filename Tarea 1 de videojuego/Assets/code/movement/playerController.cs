using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
/*
Describes the movement of player 
Alejandro Fernández 
Alan Hernandez
*/

public class playerController : MonoBehaviour
{
    [Header("Referencias")]
    Rigidbody2D playerRB;

    [Header("Movimiento lateral")]

    public float maxSpeedX;

    public float airtimeControlReduction;
    public Vector2 sensitivity;
    public Vector2 initialPushWhenGrounded;
    public Animator playerAnim;

    [Header("Cosas para el brinco")]

    /// <summary>
    /// Explains the speed that is applied to the player after pressing skip
    /// </summary>
    public float jumpForce;
    /// <summary>
    /// The gravity to be applied when pressing space
    /// </summary>
    public float initialGravity;
    /// <summary>
    /// Gravity the rest of the time
    /// </summary>
    public float finalGravity;
    /// <summary>
    /// What is the maximum time that the player can jump
    /// </summary>
    public float maxJumpTime;

    public AnimationCurve jumpCurve;

    [Header("Estadísticas del sistema")]

    public bool grounded;
    public bool jumping;
    public float elapsed;
    public Vector2 movement;

    private void Start() {
        playerRB = gameObject.GetComponent<Rigidbody2D>();
        grounded = true;
    }

    public void TouchGrass(){
        grounded = true;
        stopJump();
    }
    public void StopTouchGrass(){
        grounded = false;
        //stopJump();
    }

    private void Update() {
        var cacheSens = grounded ? sensitivity : sensitivity * airtimeControlReduction;
        playerRB.AddForce(new Vector2(movement.x * cacheSens.x * Time.deltaTime, 
                                      movement.y * cacheSens.y * Time.deltaTime));

        if (playerRB.velocity.x >  maxSpeedX){
            playerRB.velocity = new Vector2(maxSpeedX, playerRB.velocity.y);
        }

        if (playerRB.velocity.x <  -maxSpeedX){
            playerRB.velocity = new Vector2(-maxSpeedX, playerRB.velocity.y);
        }
        
        // kill the player if it falls from the map:
        if (transform.position.y < -10){
            killPlayer();
        }
    }

    public void killPlayer(){
        SceneManager.LoadScene(0);
    }

    public void win(){
        SceneManager.LoadScene(1);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("End")){
            win();
        }
    }


    /// <summary>
    /// Used by the player controller, returns how much something has moved
    /// </summary>
    /// <param name="value"></param>
    public void OnMove(InputValue value){
        movement = value.Get<Vector2>();

        if (movement.x == 0){
            playerRB.velocity = new Vector2(playerRB.velocity.x/2, playerRB.velocity.y);
            playerAnim.SetBool("isRunning", false);
        }
        else{
            playerAnim.SetBool("isRunning",true);
        }

        if (grounded){
            playerRB.velocity = new Vector2(movement.x * initialPushWhenGrounded.x, playerRB.velocity.y);
        }

    }

    /// <summary>
    /// runs asynchronously. It starts with startCoroutine and ends when it exits or stopcoroutine.
    /// </summary>
    /// <returns></returns>
    IEnumerator jumpController(){
        jumping = true;
        // // the x is kept so that we do not interfere with it
        playerRB.velocity = new Vector2(playerRB.velocity.x, jumpForce);
        playerRB.gravityScale = initialGravity;

        // General time calculations
        var startTime = Time.time;
        elapsed = Time.time - startTime;
        // 3 relays so that we know what percentage we are carrying (goes from 0 to 1)
        var percentageElapsed = elapsed/maxJumpTime;

        var localGravityScale = initialGravity;

        while (elapsed < maxJumpTime){
            elapsed = Time.time - startTime;
            percentageElapsed = elapsed/maxJumpTime;

            // a linear interpolation is made from where we are to where we should be.
            localGravityScale = Mathf.Lerp(initialGravity, finalGravity, jumpCurve.Evaluate(percentageElapsed)); // el jump curve es para conseguir el y
            playerRB.gravityScale = localGravityScale;

            // Here we wait for the following physics calculation
            yield return new WaitForFixedUpdate();
        }

        playerRB.gravityScale = initialGravity;
        jumping = false;
    }


    /// <summary>
    /// Finish the coroutine and lower the character.
    /// </summary>
    private void stopJump(){
        StopCoroutine("jumpController");
        playerRB.gravityScale = finalGravity;
    }

    /// <summary>
    /// Runs when space is pressed and when space is completed.
    /// </summary>
    /// <param name="state"></param>
    public void OnJump(InputValue state){
        if (state.Get<float>() > 0.5f){ // difference between press and stop
            // this is when the leap is initiated
            if (grounded)
                StartCoroutine("jumpController");
        }
        else{
            // This is when the leap ends
            stopJump();
        }
    }
}
