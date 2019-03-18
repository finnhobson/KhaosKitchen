using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimationController : MonoBehaviour
{
    public GameController gameController;

    public GameObject chefPrefab;
    public GameObject customerPrefab;
    public GameObject kitchenPrefab;

    private bool chefsSpawned = false;
    private int currentRound = 0;

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(kitchenPrefab);
    }

    // Update is called once per frame
    void Update()
    {
        if (gameController.playersInitialised && !chefsSpawned)
        {
            StartCoroutine(SpawnChefs());
            chefsSpawned = true;
        }

        if (currentRound < gameController.roundNumber && gameController.isGameStarted && !gameController.isRoundPaused)
        {
            SpawnCustomers();
            currentRound = gameController.roundNumber;
        }
    }

    private IEnumerator SpawnChefs()
    {
        yield return new WaitForSecondsRealtime(2);
        int playerCount = gameController.playerCount;
        for (int i = 0; i < playerCount; i++) Instantiate(chefPrefab, new Vector3(((2*i*40)+40)/(playerCount*2)-20, 0, 0), transform.rotation);
    }

    private void SpawnCustomers()
    {
        for (int i = 0; i < 3; i++) Instantiate(customerPrefab);
    }
}
