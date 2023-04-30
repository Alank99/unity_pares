using System.Collections;
using UnityEngine;

/*
check if it is touching the floor of the map 
Alan Hernandez
Alejandro Fernández
*/


public class CheckGround : MonoBehaviour
{
    public controllerplayer daddy;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag.Equals("ground")){
            daddy.TouchGrass();
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.tag.Equals("ground")){
            daddy.StopTouchGrass();
        }
    }
}
