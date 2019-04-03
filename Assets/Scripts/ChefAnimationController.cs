using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefAnimationController : MonoBehaviour
{
    private GameController gameController;
    private Animator anim; 

    // Start is called before the first frame update
    void Start()
    {
        anim = this.transform.GetComponent<Animator>();
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameController.isRoundPaused && gameController.isGameStarted)
        {
            anim.SetBool("isInGame", true);
            anim.SetBool("isEndRoundSuccessful", false);
        }
        else if (gameController.isGameStarted)
        {
            anim.SetBool("isEndRoundSuccessful", true);
            anim.SetBool("isInGame", false);
        }
    }
}
