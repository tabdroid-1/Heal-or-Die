using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAttach : MonoBehaviour
{
    [Header("Properties")]
    [Space]
    [SerializeField] private Vector2 attachPosition;
    [SerializeField] private float arriveTime;
    private GameObject cameraHolder;

    // Start is called before the first frame update
    void Start()
    {
        cameraHolder = GameObject.Find("CameraHolder");
    }

    private void FixedUpdate()
    {
        if (cameraHolder.GetComponent<FollowPlayer>().attached)
        {
            cameraHolder.transform.position = Vector2.Lerp(cameraHolder.transform.position, attachPosition, arriveTime);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            cameraHolder.GetComponent<FollowPlayer>().attached = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            cameraHolder.GetComponent<FollowPlayer>().attached = false;
        }
    }
}
