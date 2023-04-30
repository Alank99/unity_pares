using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class general : MonoBehaviour
{
    [Header("Referencias")]

    [SerializeField]
    Transform pos1;

    [SerializeField]
    Transform pos2;

    [SerializeField]
    Rigidbody2D rb;

    Transform currentLock;

    [Header("MacMovement")]
    [SerializeField]
    float maxSpeedX;
    [SerializeField]
    float Force;
    [SerializeField]
    float jumpForce;

    bool alive = true;

    // get rb reference from self on start
    private void Start() {
        rb = gameObject.GetComponent<Rigidbody2D>();
        currentLock = pos1;

        StartCoroutine("randomJumps");
    }

    IEnumerator randomJumps(){
        Debug.Log("le jump");
        while (alive){
            if (Vector3.Magnitude(transform.position - currentLock.position) < 3f){
                Debug.Log("Changind dir");
                currentLock = currentLock == pos1 ? pos2 : pos1;
            }

            var moveTowards = Vector3.MoveTowards(transform.position, currentLock.position, maxSpeedX) - transform.position;
            rb.velocity =  new Vector2(moveTowards.x * Force, jumpForce);
            Debug.Log("le jump");
            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }

    private void killSelf(){
        alive = false;
        Destroy(transform.parent.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")){
            killSelf();
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.collider.CompareTag("Player")){
            other.gameObject.GetComponent<playerController>().killPlayer();
        }
    }
}
