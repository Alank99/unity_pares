/* Create copies of a ball object to fall on the game scene*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowBall : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject ball;
    public float delay;
    public float limit;

    void Start()
    {
        //create the specified function at regular intervals
        InvokeRepeating("CreateBall",delay,delay);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateBall()
    {
        Vector3 newPos = new Vector3(Random.Range(-limit,limit),
                                                        10.9f,0);
        Instantiate(ball, newPos, Quaternion.identity);
    }
}
