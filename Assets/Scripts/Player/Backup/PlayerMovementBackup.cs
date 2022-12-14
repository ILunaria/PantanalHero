using System.Collections;
using UnityEngine;

public class PlayerMovementBackup : MonoBehaviour
{
	public PlayerData Data;

	#region COMPONENTS
		public Rigidbody2D RB { get; private set; }
		//Script to handle all player animations, all references can be safely removed if you're importing into your own project.
	#endregion

	#region STATE PARAMETERS
		//Variables control the various actions the player can perform at any time.
		//These are fields which can are public allowing for other sctipts to read them
		//but can only be privately written to.
		public bool IsFacingRight { get; private set; }

		public bool IsJumping;
		public bool IsDashing { get; private set; }

		//Timers (also all fields, could be private and a method returning a bool could be used)
		public float LastOnGroundTime;

		//Jump
		public bool _isJumpCut;
		public bool _isJumpFalling;

		//Dash
		private int _dashesLeft;
		private bool _dashRefilling;
		private Vector2 _lastDashDir;
		private bool _isDashAttacking;

	#endregion

	#region INPUT PARAMETERS
		private Vector2 _moveInput;

		public float LastPressedJumpTime;
		public float LastPressedDashTime { get; private set; }
	#endregion

	#region CHECK PARAMETERS
		//Set all of these up in the inspector
		[Header("Checks")]
		[SerializeField] private Transform _groundCheckPoint;
		//Size of groundCheck depends on the size of your character generally you want them slightly small than width (for ground) and height (for the wall check)
		[SerializeField] private Vector2 _groundCheckSize;
	#endregion

	#region LAYERS & TAGS
		[Header("Layers & Tags")]
		[SerializeField] private LayerMask _groundLayer;
	#endregion

	#region AWAKE && START

		private void Awake()
		{
			RB = GetComponent<Rigidbody2D>();
		
		}

		private void Start()
		{
			SetGravityScale(Data.gravityScale);
			IsFacingRight = true;
		}

	#endregion

	#region UPDATE && FIXED UPDATE

		private void Update()
		{
	#region TIMERS
			LastOnGroundTime -= Time.deltaTime;

			LastPressedJumpTime -= Time.deltaTime;
			LastPressedDashTime -= Time.deltaTime;
	#endregion

	#region INPUT HANDLER
			_moveInput.x = Input.GetAxisRaw("Horizontal");
			_moveInput.y = Input.GetAxisRaw("Vertical");

			if (_moveInput.x != 0)
				CheckDirectionToFace(_moveInput.x > 0);

			// When you press Jump, the script sets LastPressedJumpTime to zero, so you can Jump
			// Check Jump Region
			if (Input.GetKeyDown(KeyCode.Space))
			{
				OnJumpInput(); 
			}

			// When you release Jump, the script sets _isJumpCut to true
			//Jump Cut is a higher gravity applied when the jump button is released, so you can perform higher jumps or lower jumps
			// Check Gravity Region
			if (Input.GetKeyUp(KeyCode.Space))
			{
				OnJumpUpInput();
			}

			if (Input.GetKeyDown(KeyCode.LeftShift))
			{
				OnDashInput();
			}
	#endregion

	#region COLLISION CHECKS
			if (!IsJumping)
			{
				//Ground Check
				if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer) && !IsJumping) //checks if set box overlaps with ground
				{
					LastOnGroundTime = Data.coyoteTime; //if so sets the lastGrounded to coyoteTime
				}
			}
	#endregion

	#region JUMP CHECKS
			// Check if the player is falling
			if (IsJumping && RB.velocity.y < 0) 
			{
				IsJumping = false;
				_isJumpFalling = true;
			}
			// Check if the player is on ground
			if (LastOnGroundTime > 0 && !IsJumping) // if (LastOnGroundTime > 0 && !IsJumping && !IsWallJumping)
				{
				_isJumpCut = false;
				_isJumpFalling = false;
			}

			if (!IsDashing)
			{
				//Jump
				if (CanJump() && LastPressedJumpTime > 0) // return LastOnGroundTime > 0 && !IsJumping;
				{
					IsJumping = true;
					_isJumpCut = false;
					_isJumpFalling = false;
					Jump();

				}
			}
	#endregion

	#region DASH CHECKS
			if (CanDash() && LastPressedDashTime > 0)
			{
				//Freeze game for split second. Adds juiciness and a bit of forgiveness over directional input
				Sleep(Data.dashSleepTime);

				//If no direction is pressed, dash forward
				if (_moveInput.x != 0f)
					_lastDashDir = _moveInput;
				else
					_lastDashDir = IsFacingRight ? Vector2.right : Vector2.left;



				IsDashing = true;
				IsJumping = false;
				//IsWallJumping = false;
				_isJumpCut = false;

				StartCoroutine(nameof(StartDash), _lastDashDir);
			}
	#endregion

	#region GRAVITY
			if (!_isDashAttacking)
			{
				//Higher gravity if we've released the jump input or are falling
				if (RB.velocity.y < 0 && _moveInput.y < 0) // else if
				{
					//Much higher gravity if holding down
					SetGravityScale(Data.gravityScale * Data.fastFallGravityMult);
					//Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
					RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFastFallSpeed));
				}
				else if (_isJumpCut)
				{
					//Higher gravity if jump button released, so you can perform higher jumps or lower jumps
					SetGravityScale(Data.gravityScale * Data.jumpCutGravityMult);
					RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFallSpeed));
				}
				else if ((IsJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold) // Increases Air Time
				{
					SetGravityScale(Data.gravityScale * Data.jumpHangGravityMult);
				}
				else if (RB.velocity.y < 0)
				{
					//Higher gravity if falling
					SetGravityScale(Data.gravityScale * Data.fallGravityMult);
					//Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
					RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFallSpeed)); 

				
				}
				else
				{
					//Default gravity if standing on a platform or moving upwards
					SetGravityScale(Data.gravityScale);
				}
			}
			else
			{
				//No gravity when dashing (returns to normal once initial dashAttack phase over)
				SetGravityScale(0);
			}
	#endregion
		}

		private void FixedUpdate()
		{
			//Handle Run

			if (!IsDashing)
			{
				Run(1);
			}
			else if (_isDashAttacking)
			{
				Run(Data.dashEndRunLerp); // Reduces Player Target Speed Whilst Dashing
			}
		}

	#endregion

	#region INPUT CALLBACKS
		//Methods which handle input detected in Update()
		// Jump Buffering
		public void OnJumpInput()
		{
			LastPressedJumpTime = Data.jumpInputBufferTime;
		}

		public void OnJumpUpInput()
		{
			//if (CanJumpCut() || CanWallJumpCut())
			//	_isJumpCut = true;
			if (CanJumpCut())
				_isJumpCut = true;
		}

		public void OnDashInput()
		{
			LastPressedDashTime = Data.dashInputBufferTime;
		}
	#endregion

	#region GENERAL METHODS
		public void SetGravityScale(float scale)
		{
			RB.gravityScale = scale;
		}

		private void Sleep(float duration)
		{
			//Method used so we don't need to call StartCoroutine everywhere
			//nameof() notation means we don't need to input a string directly.
			//Removes chance of spelling mistakes and will improve error messages if any
			StartCoroutine(nameof(PerformSleep), duration);
		}

		private IEnumerator PerformSleep(float duration)
		{
			Time.timeScale = 0;
			yield return new WaitForSecondsRealtime(duration); //Must be Realtime since timeScale with be 0 
			Time.timeScale = 1;
		}
	#endregion

	#region RUN METHODS
		private void Run(float lerpAmount)
		{
			//Calculate the direction we want to move in and our desired velocity
			float targetSpeed = _moveInput.x * Data.runMaxSpeed;
			//We can reduce our control using Lerp() this smooths changes to our direction and speed
			targetSpeed = Mathf.Lerp(RB.velocity.x, targetSpeed, lerpAmount); // a + ( b - a ) * Clamp01(t)

	#region Calculate AccelRate
			float accelRate;

			//Gets an acceleration value based on if we are accelerating (includes turning) 
			//or trying to decelerate (stop). As well as applying a multiplier if we're in air.
			if (LastOnGroundTime > 0)
				accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
			else
				accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;
	#endregion

	#region Add Bonus Jump Apex Acceleration
			//Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
			//if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
			if ((IsJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
			{
				accelRate *= Data.jumpHangAccelerationMult; // Increases Acceleration Rate Whilst on Jump Apex
				targetSpeed *= Data.jumpHangMaxSpeedMult; // Increases Max Speed Whilst on Jump Apex
			}
	#endregion

	#region Conserve Momentum
			//We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
			if (Data.doConserveMomentum && Mathf.Abs(RB.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(RB.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
			{
				//Prevent any deceleration from happening, or in other words conserve are current momentum
				//You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
				accelRate = 0;
			}
	#endregion

			//Calculate difference between current velocity and desired velocity
			float speedDif = targetSpeed - RB.velocity.x;
			//Calculate force along x-axis to apply to thr player

			float movement = speedDif * accelRate;

			//Convert this to a vector and apply to rigidbody
			RB.AddForce(movement * Vector2.right, ForceMode2D.Force);
		}

		private void Turn()
		{
			//stores scale and flips the player along the x axis, 
			Vector3 scale = transform.localScale;
			scale.x *= -1;
			transform.localScale = scale;

			IsFacingRight = !IsFacingRight;
		}
	#endregion

	#region JUMP METHODS
		private void Jump()
		{
			//Ensures we can't call Jump multiple times from one press
			LastPressedJumpTime = 0;
			LastOnGroundTime = 0;

	#region Perform Jump
			//We increase the force applied if we are falling
			//This means we'll always feel like we jump the same amount 
			float force = Data.jumpForce;
			if (RB.velocity.y < 0)
				force -= RB.velocity.y;

			RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);
	#endregion
		}
	#endregion

	#region DASH METHODS
		//Dash Coroutine
		private IEnumerator StartDash(Vector2 dir) // For Multi Direction Dash Use This: private IEnumerator StartDash(Vector2 dir)
		{
			dir = dir.normalized * Vector2.right; // Horizontal Dash Only
			//Overall this method of dashing aims to mimic Celeste, if you're looking for
			// a more physics-based approach try a method similar to that used in the jump
			LastOnGroundTime = 0;
			LastPressedDashTime = 0;

			float startTime = Time.time;

			_dashesLeft--;
			_isDashAttacking = true;

			SetGravityScale(0);

			//We keep the player's velocity at the dash speed during the "attack" phase (in celeste the first 0.15s)
			while (Time.time - startTime <= Data.dashDragTime)
			{
				RB.velocity =  dir.normalized * Data.dashSpeed;
				//Pauses the loop until the next frame, creating something of a Update loop. 
				//This is a cleaner implementation opposed to multiple timers and this coroutine approach is actually what is used in Celeste :D
				yield return null;
			}

			startTime = Time.time;

			_isDashAttacking = false;

			//Begins the "end" of our dash where we return some control to the player but still limit run acceleration (see Update() and Run())
			SetGravityScale(Data.gravityScale);
			RB.velocity =  Data.dashEndSpeed * dir.normalized;

			while (Time.time - startTime <= Data.dashEndTime)
			{
				yield return null;
			}

			//Dash over
			IsDashing = false;
		}

		//Short period before the player is able to dash again
		private IEnumerator RefillDash() // int amount
		{
			//SHoet cooldown, so we can't constantly dash along the ground, again this is the implementation in Celeste, feel free to change it up
			_dashRefilling = true;
			yield return new WaitForSeconds(Data.dashRefillTime);
			_dashRefilling = false;
			_dashesLeft = Mathf.Min(Data.dashAmount, _dashesLeft + 1);
		}
	#endregion

	#region CHECK METHODS
		public void CheckDirectionToFace(bool isMovingRight)
		{
			if (isMovingRight != IsFacingRight)
				Turn();
		}

		// LastOnGrounTime refers to the player staying on the ground or if he's fallen from the platform but still is able to jump for some time.
		private bool CanJump()
		{
			return LastOnGroundTime > 0 && !IsJumping;
		}

		private bool CanJumpCut()
		{
			return IsJumping && RB.velocity.y > 0;
		}
		private bool CanDash()
		{
			if (!IsDashing && _dashesLeft < Data.dashAmount && LastOnGroundTime > 0 && !_dashRefilling)
			{
				StartCoroutine(nameof(RefillDash)); // StartCoroutine(nameof(RefillDash), 1);
			}

			return _dashesLeft > 0;
		}
	#endregion

	#region EDITOR METHODS
		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
			Gizmos.color = Color.blue;
		}
	#endregion
}