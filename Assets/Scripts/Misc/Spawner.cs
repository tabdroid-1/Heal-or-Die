using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject heartContainer;
    public Transform[] spawnpoints;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    [System.Obsolete]
    void Update()
    {
        int randomSpawnPositionIndex;
        if (GameObject.Find("HeartContainer(Clone)") == null)
        {
            randomSpawnPositionIndex = Random.Range(0, 6);
            Instantiate(heartContainer, spawnpoints[randomSpawnPositionIndex]);
        }
    }
}
