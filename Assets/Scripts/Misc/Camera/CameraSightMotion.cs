using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSightMotion : MonoBehaviour
{
    private float sightOffset;
    private GameObject player;
    [SerializeField] private float arriveTime;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");

    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.rotation.y == 0)
        {
            sightOffset = 2f;
        }
        else
        {
            sightOffset = -2f;
        }

        Vector3 target = new Vector3(sightOffset, transform.localPosition.y, transform.localPosition.z);

        transform.localPosition = Vector3.Lerp(transform.localPosition, target, arriveTime);
    }
}
