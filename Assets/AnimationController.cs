using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimationController : MonoBehaviour
{
    public GameController gameController;

    public GameObject chefPrefab;

    // Start is called before the first frame update
    void Start()
    {
        int playerCount = gameController.playerCount;
        for (int i = 0; i < playerCount; i++) SpawnChef();
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    public void SpawnChef()
    {
        Instantiate(chefPrefab);
    }
}
