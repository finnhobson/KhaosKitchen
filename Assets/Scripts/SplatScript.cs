using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplatScript : MonoBehaviour
{

    public float spinx = 0;
    public float spiny = 0;
    public float spinz = 0; 

    
    // Start is called before the first frame update

   

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(spinx, spiny, spinz);

    }
}
