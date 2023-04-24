using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotion : MonoBehaviour
{
    private Animator animator;
    private GameObject player;
    private Rigidbody2D playerRB;
    [Header("Properties")]
    [Space]
    [SerializeField] private float cameraFallStunShakeOffset = -2f;
    [SerializeField] private float fullStunShakeArriveTime = 0.05f;
    [Space]
    [Header("State")]
    [Space]
    [SerializeField] private float playerSight;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        playerRB = player.GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {

        Vector3 target = Vector3.zero;

        if (player.transform.rotation.y == 0 && playerRB.velocity.x >= 0)
        {
            animator.SetBool("MovingRight", true);
            animator.SetBool("MovingLeft", false);
        }
        else if (player.transform.rotation.y != 0 && playerRB.velocity.x <= 0)
        {
            animator.SetBool("MovingRight", false);
            animator.SetBool("MovingLeft", true);
        } else
        {
            animator.SetBool("MovingRight", false);
            animator.SetBool("MovingLeft", false);
        }


    }

    public void FallStunAnimTriger()
    {
        animator.SetTrigger("FallStun");
    }

}
