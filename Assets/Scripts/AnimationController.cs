using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AnimationController : MonoBehaviour
{
    public GameController gameController;

    public Image arrow;

    public GameObject chefPrefab;
    public GameObject customerPrefab;
    public GameObject kitchenPrefab;
    public GameObject firePrefab;

    private bool chefsSpawned = false;
    private int currentRound = 0;
    private int fireCount = 0;

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

        if (currentRound < gameController.roundNumber && gameController.isGameStarted && fireCount > 0)
        {
            DestroyFires();
            fireCount = 0;
        }

        if (currentRound < gameController.roundNumber && gameController.isGameStarted && !gameController.isRoundPaused)
        {
            SpawnCustomers();
            currentRound = gameController.roundNumber;
        }

        if (fireCount < gameController.fireCount)
        {
            SpawnFire();
            fireCount = gameController.fireCount;
        }
    }

    private IEnumerator SpawnChefs()
    {
        yield return new WaitForSecondsRealtime(2);
        int playerCount = gameController.playerCount;
        for (int i = 0; i < playerCount; i++)
        {
            Vector3 pos = new Vector3(((2 * i * 40) + 40) / (playerCount * 2) - 20, 0, 0);
            GameObject newChef = Instantiate(chefPrefab, pos, transform.rotation);
            Image newArrow = Instantiate(arrow);
            newArrow.color = new Color(0, 0, 1);
            newArrow.transform.SetParent(GameObject.FindGameObjectWithTag("ServerCanvas").transform, false);
            newChef.GetComponent<ChefController>().arrow = newArrow;
        }
    }

    private void SpawnCustomers()
    {
        for (int i = 0; i < 2; i++) Instantiate(customerPrefab);
    }

    private void DestroyFires()
    {
        var fires = GameObject.FindGameObjectsWithTag("Fire");
        foreach (var fire in fires)
        {
            Destroy(fire);
        }
    }

    private void SpawnFire()
    {
        float x = UnityEngine.Random.Range(-25, 25);
        float z = UnityEngine.Random.Range(-10, 8);
        Instantiate(firePrefab, new Vector3(x, 0, z), transform.rotation);
    }
}
