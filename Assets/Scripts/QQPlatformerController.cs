using UnityEngine;
using System.Collections;

public class PlatformerController : MonoBehaviour 
{
    private Vector2 currentTile;
    private Vector3 velocity;

    public float acceleration;
    public float maxSpeed;
    public float jumpSpeed;
    public float gravity;

    private bool grounded;
    private CircleCollider2D circleCollider;

    private QQLevelGenerator levelGenerator;

	// Use this for initialization
	void Start () 
    {
        circleCollider = GetComponent<CircleCollider2D>();
        levelGenerator = QQGameManager.Instance.GetComponent<QQLevelGenerator>();
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    private void Move()
    {
        CheckIfGrounded();

        ApplyAcceleration();
        ApplyGravity();
        Jump();

        transform.position += velocity;
    }

    private void ApplyAcceleration()
    {
        if (grounded)
        {
            if (velocity.x <= maxSpeed)
            {
                velocity.x += acceleration;

                if (velocity.x >= maxSpeed)
                    velocity.x = maxSpeed;
            }
        }
    }

    private void ApplyGravity()
    {
        if (grounded)
            velocity.y -= gravity;
    }

    private void Jump()
    {
        if (grounded && Input.GetKeyDown(KeyCode.Space))
            velocity.y += jumpSpeed;
    }

    private void CheckIfGrounded()
    {
        
    }

    private void CheckCollisionForward()
    {

    }
}
