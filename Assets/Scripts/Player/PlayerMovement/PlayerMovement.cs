using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    [Header("Essentials")]
    [Space]
    private PlayerControls playerControls;
    public PlayerController controller;
    public Collider2D collision;
    [Space]
    [Header("Properties")]
    [Space]
    public float runSpeed = 40f;
    Vector2 moveValue;
    [SerializeField] private bool jumpIsTriger;
    bool jump = false;
    bool crouch = false;
    bool dash = false;
    [SerializeField] private LayerMask groundIs;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        moveValue = playerControls.Player.Move.ReadValue<Vector2>();

        /*
        if (jumpIsTriger)
        {
            if (playerControls.Player.Jump.triggered)
            {
                jump = true;
            }
        }
        else
        {
            if (playerControls.Player.Jump.IsPressed())
            {
                jump = true;
            }
            else
            {
                jump = false;
            }
        }*/


        if (playerControls.Player.Jump.IsPressed())
        {
            if (controller.m_Grounded)
            {
                controller.jumping = true;
                controller.falling = false;
            }
            jump = true;
        }
        else
        {
            jump = false;
            if (controller.jumping)
            {
                controller.canJump = false;
            }
        }

        if (playerControls.Player.Dash.triggered)
        {
            dash = true;
        }

        
        

        


        if (playerControls.Player.Crouch.IsPressed())
        {
            crouch = true;
        }
        if (!playerControls.Player.Crouch.IsPressed())
        {
            crouch = false;
        }
    }

    void FixedUpdate()
    {
        controller.Move(moveValue.x * runSpeed * Time.fixedDeltaTime, crouch, jump, dash);
        dash = false;
        //if (jumpIsTriger) jump = false;
    }
}
