using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimationController : MonoBehaviour
{
    public GameController gameController;

    public GameObject chefPrefab;

    private bool chefsSpawned = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (gameController.playersInitialised && !chefsSpawned)
        {
            SpawnChefs();
            chefsSpawned = true;
        }
    }

    public void SpawnChefs()
    {
        int playerCount = gameController.playerCount;
        for (int i = 0; i < playerCount; i++) Instantiate(chefPrefab);
    }
}
