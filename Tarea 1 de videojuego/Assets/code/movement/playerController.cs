using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    /// Explica la velocidad que se le aplica al jugador después de presionar brincar
    /// </summary>
    public float jumpForce;
    /// <summary>
    /// La gravedad que se le va a aplicar cuando precione espacio
    /// </summary>
    public float initialGravity;
    /// <summary>
    /// La gravedad el resto del tiempo
    /// </summary>
    public float finalGravity;
    /// <summary>
    /// Cual es el tiempo máximo que el jugador puede brincar 
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
        Debug.Log("se salto");
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
        
    }




    /// <summary>
    /// Utilizado por el player controller, regresa que tanto esta movido algo
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
    /// Esta madre se corre asincrona. Se inicia con startCoroutine y termina cuando se sale o stopcoroutine
    /// </summary>
    /// <returns></returns>
    IEnumerator jumpController(){
        jumping = true;
        // la x se mantiene para que no interferimos con ella
        playerRB.velocity = new Vector2(playerRB.velocity.x, jumpForce);
        playerRB.gravityScale = initialGravity;

        // Cálculos generales de tiempo
        var startTime = Time.time;
        elapsed = Time.time - startTime;
        // relga de 3 para que sepamos que porcentaje llevamos (va de 0 a 1)
        var percentageElapsed = elapsed/maxJumpTime;

        var localGravityScale = initialGravity;

        while (elapsed < maxJumpTime){
            elapsed = Time.time - startTime;
            percentageElapsed = elapsed/maxJumpTime;

            // vamos a hacer una interpolación lineal de donde estamos, a donde debemos de estar
            localGravityScale = Mathf.Lerp(initialGravity, finalGravity, jumpCurve.Evaluate(percentageElapsed)); // el jump curve es para conseguir el y
            playerRB.gravityScale = localGravityScale;

            // Aquí esperamos al siguiente calculo de física
            yield return new WaitForFixedUpdate();
        }

        playerRB.gravityScale = initialGravity;
        jumping = false;
    }


    /// <summary>
    /// Termina la corutina y baja al personaje
    /// </summary>
    private void stopJump(){
        StopCoroutine("jumpController");
        playerRB.gravityScale = finalGravity;
    }

    /// <summary>
    /// Se corre cuando se preciona espacio y cuando se termina de precionar
    /// </summary>
    /// <param name="state"></param>
    public void OnJump(InputValue state){
        if (state.Get<float>() > 0.5f){ // diferencia entre preciona y deja de
            // Nota: este es cuando se inicia el brinco
            if (grounded)
                StartCoroutine("jumpController");
        }
        else{
            // Aquí es cuando se termina el brinco
            stopJump();
        }
    }
}
