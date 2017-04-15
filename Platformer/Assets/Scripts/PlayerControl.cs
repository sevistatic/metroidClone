using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
	[HideInInspector]
	public bool jump = false;				// Condition for whether the player should jump.
	[HideInInspector]
	public bool crouch = false;				//Condition for whether the player should crouch.
	[HideInInspector]
	public bool facingRight = true;			//Whether or not the player is currently facing right.
	public float moveForce = 365f;			// Amount of force added to move the player left and right.
	public float maxSpeed = 5f;				// The fastest the player can travel in the x axis.
	public float jumpForce = 1000f;			// Amount of force added when the player jumps.
	
	private Transform groundCheck;			// A position marking where to check if the player is grounded.
	private Transform groundCheck1;			// A position marking where to check if the player is grounded.
	private bool grounded = false;			// Whether or not the player is grounded.
	private Animator anim;					// Reference to the player's animator component.


	void Awake()
	{
		// Setting up references.
		groundCheck = transform.Find("groundCheck");
		groundCheck1 = transform.Find ("groundCheck1");
		anim = GetComponent<Animator>();
	}


	void Update()
	{
		// The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.

		grounded = isGroundedRays ();

		anim.SetBool ("Grounded", grounded);
		anim.SetFloat ("YVelocity", GetComponent<Rigidbody2D> ().velocity.y); 
		//Debug.DrawLine (transform.position, groundCheck.position, Color.red, .25f);


		// If the jump button is pressed and the player is grounded then the player should jump.
		if(Input.GetButtonDown("Jump"))
			jump = true;
	}


	void FixedUpdate ()
	{
		// Cache the horizontal input.
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis ("Vertical");
		int speed = 0;
		if (v < 0)
			crouch = true;
		else
			crouch = false;

		if (h < 0)
			speed = -1;
		else if (h > 0)
			speed = 1;
		else
			speed = 0;
		
		anim.SetBool ("Crouch", crouch);

		// The Speed animator parameter is set to the absolute value of the horizontal input.
		anim.SetFloat("Speed", Mathf.Abs(speed));

		// If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
		if(h * GetComponent<Rigidbody2D>().velocity.x < maxSpeed && !(crouch && grounded))
			// ... add a force to the player.
			GetComponent<Rigidbody2D>().AddForce(Vector2.right * speed * moveForce);



		// If the player's horizontal velocity is greater than the maxSpeed...
		if(Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x) > maxSpeed)
			// ... set the player's velocity to the maxSpeed in the x axis.
			GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Sign(GetComponent<Rigidbody2D>().velocity.x) * maxSpeed, GetComponent<Rigidbody2D>().velocity.y);

		// If the input is moving the player right and the player is facing left...
		if (h > 0 && !facingRight) {
			// ... flip the player.
			facingRight = !facingRight;
			if (crouch)
				anim.Play("RotateCrouching");
			else
				anim.Play ("RotateStanding");
			//anim.SetBool("ChangeDirection", true);
		}
		// Otherwise if the input is moving the player left and the player is facing right...
		else if(h < 0 && facingRight){
			// ... flip the player.
			facingRight = !facingRight;
			if (crouch)
				anim.Play("RotateCrouching");
			else
				anim.Play ("RotateStanding");
			//anim.SetBool("ChangeDirection", true);

		}

		// If the player should jump...
		if(jump && grounded)
		{
			// Set the Jump animator trigger parameter.
			anim.SetTrigger("Jump");

			// Add a vertical force to the player.
			GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, jumpForce));

			// Make sure the player can't jump again until the jump conditions from Update are satisfied.
			jump = false;
		}
	}

	bool isGroundedRays(){
		bool groundedLeft = Physics2D.Linecast (transform.position, groundCheck.position, 1 << LayerMask.NameToLayer ("Ground"));
		Debug.DrawLine (transform.position, groundCheck.position, Color.red, 1);
		bool groundedRight = Physics2D.Linecast (transform.position, groundCheck1.position, 1 << LayerMask.NameToLayer ("Ground"));
		Debug.DrawLine (transform.position, groundCheck1.position, Color.blue, 1);
		return groundedLeft || groundedRight;
	}

	bool isGroundedColliders(){
		return true;
	}
}
