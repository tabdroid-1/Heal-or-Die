using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Essentials")]
    [Space]
    [SerializeField] private NewPlayerMovement playerController;
    [SerializeField] private GameObject deadUI;
    [SerializeField] private Slider slider1;
    [SerializeField] private Slider slider2;

    [Space]
    [Header("Properties")]
    [Space]
    public float hitPoint = 5;
    [SerializeField] private float maxHitPoint = 5;
    [Space]
    [Header("Player State")]
    [Space]
    public bool dead = false;


    // Update is called once per frame
    void Update()
    {
        Dead();
        Damage();


        slider1.value = hitPoint;
        slider2.value = hitPoint;
    }

    public void Damage()
    {
        hitPoint -= Time.deltaTime;
    }

    public void Heal()
    {
        hitPoint = maxHitPoint;

    }

    void Dead()
    {
        if (hitPoint <= 0)
        {
            dead = true;
            playerController.dead = true;
            deadUI.SetActive(true);
            Time.timeScale = 0.3f;
        }
    }



}
