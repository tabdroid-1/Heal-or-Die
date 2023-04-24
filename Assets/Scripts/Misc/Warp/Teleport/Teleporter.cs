using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [Header("Essentials")]
    [Space]
    [SerializeField] private Transform teleportPosition;
    [Space]
    [Header("Properties")]
    [Space]
    [SerializeField] private float teleportSpeed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.transform.position = teleportPosition.position;
        }
    }
}
