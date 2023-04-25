/*almacena los puntos*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class score : MonoBehaviour
{

    [SerializeField ] 
    ParticleSystem particulas; 
    [SerializeField] 
    TMP_Text score_txt;
    int point;
    // Start is called before the first frame update
    void Start()
    {
        point = 0;

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col){
        point += 1;
        score_txt.text = "Score :" + point;
        Destroy(col.transform.parent.gameObject);
        particulas.Emit(20);
        
    }
}
