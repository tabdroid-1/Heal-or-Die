using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class NewPlayerMovement : MonoBehaviour
{
	private PlayerControls playerControls;
	[SerializeField] private float m_JumpForce = 10f;                          // Amount of force added when the player jumps.
	[SerializeField] private float runSpeed = 40f;
	[Range(0, 1)][SerializeField] private float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
	[SerializeField] private float groundCheckCircleRadius;
	[Range(0, .3f)][SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
	[SerializeField] private Collider2D checkCeilingColider;
	[SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
	[SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching
	[SerializeField] private Collider2D m_CircleCollider;

	private Rigidbody2D m_Rigidbody2D;
	private Vector3 m_Velocity = Vector3.zero;
	Vector2 moveValue;


	[Space]
	[Header("Jump Stuff")]
	[Space]
	public float jumpBufferTime = 0.2f;                                         // Used to register jump before landing for better movement.
	[HideInInspector] public float jumpBufferCounter;                           // Counter used for jump buffer.
	[SerializeField] private float jumpTime = 1f;
	[SerializeField] private float jumpTimeCounter = 0f;

	[Space]
	[Header("Dash stuff")]
	[Space]
	[SerializeField] private float _dashSpeed;
	[SerializeField] private float _dashDistance = 5f;
	[SerializeField] private float _dashBufferLenght = 0.2f;
	[HideInInspector] public bool _isDashing;
	[SerializeField] private float _dashCoolDown = 0.75f;
	[SerializeField] private bool _canDash = true;

	public bool canStandUp = true;
	[Space]
	[Header("Stun")]
	[SerializeField] private float stunDuration = 2f;
	[SerializeField] private float stunTimer = 0f;
	[Space]
	[Header("Animation stuff")]
	[Space]
	[SerializeField] Animator animator;
	[Space]
	[Header("Player State")]
	[Space]

	[SerializeField] private bool shouldBeFallStunned = false; //whether or not player should be stunned
	[SerializeField] private bool canMove = true;      //whether or not player can move
	public bool canJump = true;
	public float airborneTimer = 0f;
	public float fallForUntilStun = 1.3f;  //how long player should be airborne until stun
	public bool m_Grounded = false;            // Whether or not the player is grounded.
	private bool m_FacingRight = true;        // whether or not facing right
	[SerializeField] private bool canFall = true;

	[Space]
	[Space]
	[Space]

	public bool stunned;
	public bool jumping;
	public bool running;
	public bool falling;
	public bool dashing;
	public bool crouching;
	public bool crawling;
	public bool dead;
	[Space]
	[Header("Input")]
	[Space]
	[SerializeField] private bool crouchInput;
	[SerializeField] private bool jumpTrigerInput;
	[SerializeField] private bool jumpPressInput;
	[Space]
	[Header("Events")]
	[Space]

	bool canEnvokeFallStunEvent = true;

	public UnityEvent OnLandEvent;

	public UnityEvent OnFallStunEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;

	private void Awake()
	{
		playerControls = new PlayerControls();

		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnFallStunEvent == null)
			OnFallStunEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}

	private void OnEnable()
	{
		playerControls.Enable();
	}

	private void OnDisable()
	{
		playerControls.Disable();
	}

	private void Update()
	{
		Movement(Input.GetKey(KeyCode.W), Input.GetKey(KeyCode.S));
		AirborneTime();
		Stun();
	}


	private void FixedUpdate()
	{
		bool wasGrounded = m_Grounded;

		Animation();



		m_Grounded = Physics2D.OverlapCircle(m_GroundCheck.position, groundCheckCircleRadius, m_WhatIsGround); // checks if you are within 0.15 position in the Y of the ground

		if (m_Grounded) if (!wasGrounded) OnLandEvent.Invoke();

		if (dead) m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y);

	}

	private void MovementInput()
    {
		if (Input.GetKeyDown(KeyCode.W)) jumpTrigerInput = true;

		if (Input.GetKey(KeyCode.W)) jumpPressInput = true;
		if (Input.GetKeyUp(KeyCode.W)) jumpPressInput = false;
	}

	public void AirborneTime()
	{
		if (!m_Grounded && m_Rigidbody2D.velocity.y <= 0)
		{
			airborneTimer += Time.deltaTime;
		}
		else
		{
			airborneTimer = 0f;
		}

		if (airborneTimer >= fallForUntilStun)
		{
			shouldBeFallStunned = true;
		}
	}

	public void Stun()
	{
		//stun from fall

		if (shouldBeFallStunned && m_Grounded)
		{


			stunned = true;
			stunTimer += Time.deltaTime;

			m_Rigidbody2D.velocity = new Vector2(0, m_Rigidbody2D.velocity.y); //doesnt lets player move while stunned(bug fix)

			StopCoroutine(Dash());  //cancels dash during stun

			if (canEnvokeFallStunEvent && stunned)
			{
				canEnvokeFallStunEvent = false;
				OnFallStunEvent.Invoke();
			}
		}

		if (stunTimer >= stunDuration)
		{
			canEnvokeFallStunEvent = true;
			shouldBeFallStunned = false;
			stunned = false;
			stunTimer = 0f;
		}
	}

	private void Movement(bool jump ,bool crouch)
    {

		moveValue = playerControls.Player.Move.ReadValue<Vector2>();

		float move = moveValue.x * runSpeed * Time.fixedDeltaTime;

		// If crouching, check to see if the character can stand up
		if (!crouch)
		{
			if (!canStandUp)
			{
				crouch = true;
			}
		}

		if (!dead && canMove && !stunned)
        {
			if (m_Grounded || m_AirControl && !_isDashing && !stunned)
            {

				if (playerControls.Player.Crouch.IsPressed() && !_isDashing && !jumping)
                {
					if (!m_wasCrouching)
					{
						m_wasCrouching = true;
						OnCrouchEvent.Invoke(true);
					}


					// Reduce the speed by the crouchSpeed multiplier
					move *= m_CrouchSpeed;

					// Disable one of the colliders when crouching
					if (m_CrouchDisableCollider != null)
						m_CrouchDisableCollider.enabled = false;
				}
				else if (!playerControls.Player.Crouch.IsPressed())
				{
					// Enable the collider when not crouching
					if (m_CrouchDisableCollider != null)
						m_CrouchDisableCollider.enabled = true;

					if (m_wasCrouching)
					{
						m_wasCrouching = false;
						OnCrouchEvent.Invoke(false);
					}
				}


				// Move the character by finding the target velocity
				Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
				// And then smoothing it out and applying it to the character
				m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

				if (move > 0 && !dashing && !stunned && !dead)
				{
					// ... flip the player.
					transform.rotation = new Quaternion(0, 0, 0, 0);
					m_FacingRight = true;
				}
				// Otherwise if the input is moving the player left and the player is facing right...
				else if (move < 0 && !dashing && !stunned && !dead)
				{
					// ... flip the player.
					transform.rotation = new Quaternion(0, 180, 0, 0);
					m_FacingRight = false;
				}

				//----------jump----------------

				if (jump)
				{
					jumpBufferCounter = jumpBufferTime;
				}
				else
				{
					jumpBufferCounter -= Time.deltaTime;
				}



				if (jumpBufferCounter > 0 && canJump && jumpTimeCounter <= jumpTime)
				{
					jumping = true;
					falling = false;
					jumpTimeCounter += Time.deltaTime;
					m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, m_JumpForce);
					animator.Play("PlayerJump", -1, 0f);
				}
				else if (jumpTimeCounter >= jumpTime || !jump)
				{
					canJump = false;
				}

				if (m_Grounded)
				{
					jumpTimeCounter = 0;
					if (!jump)
					{
						canJump = true;
					}
				}

			}
        }

		//----------------State Zone----------------
		//fall state
		if (!m_Grounded && !jumping && !dashing && !dead && canFall) falling = true;
		else if (m_Grounded || dead || jumping || dashing || jumpTimeCounter != 0 || !canFall) falling = false;

		//dash state
		dashing = _isDashing;

		//run state 
		if (move != 0 && !playerControls.Player.Crouch.IsPressed() && !dashing && !crawling && !falling && !jumping && !crouching && !dead && !stunned && m_Grounded) running = true;
		else if (move == 0 || dashing || jumping || falling || crouching || crawling || dead || stunned && !m_Grounded) running = false;

		//crouch state
		if (playerControls.Player.Crouch.IsPressed() && move == 0 && !dashing && !dead && !stunned) crouching = true;
		if (!playerControls.Player.Crouch.IsPressed() || dashing || move != 0 || falling || dead || stunned) crouching = false;

		//jump state stuff
		if (m_Grounded || dashing || dead) jumping = false;

		//crawling state
		if (move != 0 && playerControls.Player.Crouch.IsPressed() && !falling && !dead && !stunned) crawling = true;
		else if (!(move != 0 && playerControls.Player.Crouch.IsPressed()) || falling || dead) crawling = false;
	}

	private void Animation()
	{
		if (running) animator.SetBool("Running", true);
		else animator.SetBool("Running", false);

		if (jumping) animator.SetBool("Jumping", true);
		else animator.SetBool("Jumping", false);

		if (falling) animator.SetBool("Falling", true);
		else animator.SetBool("Falling", false);

		if (crouching) animator.SetBool("Crouching", true);
		else animator.SetBool("Crouching", false);

		if (dashing) animator.SetBool("Dashing", true);
		else animator.SetBool("Dashing", false);

		if (crawling) animator.SetBool("Crawling", true);
		else animator.SetBool("Crawling", false);

		if (dead) animator.SetBool("Dead", true);
		else animator.SetBool("Dead", false);
	}

	IEnumerator Dash()
	{
		float dashStartTime = Time.time;
		_canDash = false;
		//_hasDashed = true;
		_isDashing = true;

		m_Rigidbody2D.velocity = Vector2.zero;
		m_Rigidbody2D.gravityScale = 0f;

		Vector2 dir;
		if (m_FacingRight) dir = new Vector2(1f, 0f);
		else dir = new Vector2(-1f, 0f);

		while (Time.time < dashStartTime + _dashBufferLenght)
		{
			m_Rigidbody2D.velocity = dir * _dashSpeed;
			airborneTimer = 0f;
			shouldBeFallStunned = false;
			yield return null;
		}

		m_Rigidbody2D.gravityScale = 2f;
		StartCoroutine(CanDash(_dashCoolDown));
		_isDashing = false;
	}

	IEnumerator CanDash(float coolDown)
	{
		yield return new WaitForSeconds(coolDown);
		_canDash = true;
	}

	private void OnTriggerStay2D(Collider2D collision)
	{

		if (collision.gameObject.layer == LayerMask.NameToLayer("Environment"))
		{
			canStandUp = false;
		}

		/*
		if (collision.gameObject.layer == LayerMask.NameToLayer("Projectile") || collision.gameObject.CompareTag("Trap"))
        {
			Physics2D.IgnoreCollision(checkCeilingColider, collision);
			Physics2D.IgnoreCollision(m_CrouchDisableCollider, collision);
			Physics2D.IgnoreCollision(m_CircleCollider, collision);
		}*/

	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Environment") && !collision.isTrigger)
		{
			canStandUp = true;
		}
	}

	public void EnterDoor()
	{
		StartCoroutine(MoveAtDoor());
	}

	IEnumerator MoveAtDoor()
	{
		canMove = false;
		// Move the character by finding the target velocity
		Vector3 targetVelocity = new Vector2(35 * 10f, m_Rigidbody2D.velocity.y);
		// And then smoothing it out and applying it to the character
		m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
		yield return new WaitForSeconds(0.5f);
		canMove = true;
	}



}
