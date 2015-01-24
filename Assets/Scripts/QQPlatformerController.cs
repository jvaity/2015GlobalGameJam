using UnityEngine;
using System.Collections;

public class QQPlatformerController : MonoBehaviour 
{
    //private Vector2 currentTile;
    private Vector3 velocity;

    public float acceleration   = 0.01f;
    public float maxSpeed       = 1.0f;
    public float jumpSpeed      = 10.0f;
    public float gravity        = 0.001f;

    private bool grounded;
    private bool jumped;
    private QQLevelGenerator levelGenerator;

    private Transform floorCheck;
    private float radius;
    private Vector3 MinYPos
    {
        get { return floorCheck.position; }
    }

    public TileType currentTileType;

	void Start () 
    {
        levelGenerator = QQGameManager.Instance.LevelGenerator;
        floorCheck = transform.FindChild("BottomPos");

        if (floorCheck != null)
            radius = Vector3.Distance(transform.position, floorCheck.position);
	}
	
	void Update () 
    {
        Move();
	}

    public void Spawn(Vector3 position)
    {
        velocity = Vector3.zero;
        transform.position = position;
    }

    private void Move()
    {
        CheckIfGrounded();

        ApplyAcceleration();
        ApplyGravity();
        Jump();

        velocity = LimitVel();

        transform.position += velocity;
    }

    private void CheckIfGrounded()
    {
        TileType tileTypeBelow = levelGenerator.TileTypeAtPosition(MinYPos);

        currentTileType = tileTypeBelow;
        
        switch (tileTypeBelow)
        {
            case TileType.Empty:
                grounded = false;
                break;
            case TileType.Block:
                if (!jumped)
                {
                    grounded = true;

                    Debug.Log("Setting y Pos to: " + (Mathf.CeilToInt(MinYPos.y) + (radius * 2.0f)));
                    transform.position = new Vector3(transform.position.x, Mathf.CeilToInt(MinYPos.y) + (radius * 2.0f), transform.position.z);
                }
                break;
        }
    }

    private void ApplyAcceleration()
    {
        if (velocity.x < maxSpeed)
            velocity.x += acceleration * Time.deltaTime;
    }

    private void ApplyGravity()
    {
        if (!grounded)
            velocity.y -= gravity * Time.deltaTime;
        else
            velocity.y = 0.0f;

        if (velocity.y <= 0.0f && jumped)
            jumped = false;
    }

    private void Jump()
    {
        if (grounded && Input.GetKeyDown(KeyCode.Space))
            velocity.y += jumpSpeed * Time.deltaTime;
    }

    private Vector3 LimitVel()
    {
        Vector3 vel = velocity;

        if (vel.x > maxSpeed)
            vel.x = maxSpeed;

        if (Mathf.Abs(vel.y) > maxSpeed)
            vel.y = (vel.y >= 0) ? maxSpeed : -maxSpeed;

        return vel;
    }
}
