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
    public int customerNumber = 0;

    private bool chefsSpawned = false;
    private int currentRound = 0;
    private int fireCount = 0;
    private bool bonusCustomers = false;

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
            bonusCustomers = false; 
        }

        if (fireCount < gameController.fireCount)
        {
            SpawnFire();
            fireCount = gameController.fireCount;
        }
        if (gameController.customerSatisfaction > 75 && customerNumber < 20 && bonusCustomers == false && !gameController.isRoundPaused)
        {
            bonusCustomers = true;
            SpawnCustomers();


        }
    }

    private IEnumerator SpawnChefs()
    {
        yield return new WaitForSecondsRealtime(3);
        var players = FindObjectsOfType<Player>();
        for (int i = 0; i < players.Length; i++)
        {
            Vector3 pos = new Vector3(((2 * i * 40) + 40) / (players.Length * 2) - 20, 0, 0);
            GameObject newChef = Instantiate(chefPrefab, pos, transform.rotation);
            Image newArrow = Instantiate(arrow);
            newArrow.color = players[i].PlayerColour;
            newArrow.transform.SetParent(GameObject.FindGameObjectWithTag("ServerCanvas").transform, false);
            newChef.GetComponent<ChefController>().arrow = newArrow;
        }
    }

    private void SpawnCustomers()
    {

        GameObject customer = Instantiate(customerPrefab, new Vector3(-50, 0.5f, -28), transform.rotation);
        customerNumber++;
        GameObject shirt = customer.GetComponent<CustomerController>().shirt;
        Material randColour = new Material(shirt.GetComponent<MeshRenderer>().material);
        List<GameObject> hatParts = customer.GetComponent<CustomerController>().hatParts;
        int rand = UnityEngine.Random.Range(0, 7);
        if (rand == 0) randColour.color = Color.red;
        if (rand == 1) randColour.color = Color.yellow;
        if (rand == 2) randColour.color = Color.green;
        if (rand == 3) randColour.color = Color.blue;
        if (rand == 4) randColour.color = Color.cyan;
        if (rand == 5) randColour.color = Color.magenta;
        if (rand == 6) randColour.color = Color.white;
        shirt.GetComponent<MeshRenderer>().material = randColour;

        rand = UnityEngine.Random.Range(0, 3);
        if (rand == 0) randColour.color = Color.red;
        if (rand == 1) randColour.color = Color.blue;
        if (rand == 2) randColour.color = Color.cyan;
        foreach (GameObject part in hatParts)
        {
            part.GetComponent<MeshRenderer>().material = randColour;
        }

        customer = Instantiate(customerPrefab, new Vector3(60, 0.5f, -28), transform.rotation);
        customerNumber++;
        shirt = customer.GetComponent<CustomerController>().shirt;
        randColour = new Material(shirt.GetComponent<MeshRenderer>().material);
        rand = UnityEngine.Random.Range(0, 7);
        if (rand == 0) randColour.color = Color.red;
        if (rand == 1) randColour.color = Color.yellow;
        if (rand == 2) randColour.color = Color.green;
        if (rand == 3) randColour.color = Color.blue;
        if (rand == 4) randColour.color = Color.cyan;
        if (rand == 5) randColour.color = Color.magenta;
        if (rand == 6) randColour.color = Color.white;
        shirt.GetComponent<MeshRenderer>().material = randColour;

        rand = UnityEngine.Random.Range(0, 3);
        if (rand == 0) randColour.color = Color.red;
        if (rand == 1) randColour.color = Color.blue;
        if (rand == 2) randColour.color = Color.cyan;
        foreach (GameObject part in hatParts)
        {
            part.GetComponent<MeshRenderer>().material = randColour;
        }
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
