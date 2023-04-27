using System.Collections;
using UnityEngine;

public class CheckGround : MonoBehaviour
{
    public controllerplayer blizzar;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag.Equals("ground")){
            blizzar.TouchGrass();
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.tag.Equals("ground")){
            blizzar.StopTouchGrass();
        }
    }
}