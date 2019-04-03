using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefAnimationController : MonoBehaviour
{

    Animator anim; 

    // Start is called before the first frame update
    void Start()
    {
        anim = this.transform.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
