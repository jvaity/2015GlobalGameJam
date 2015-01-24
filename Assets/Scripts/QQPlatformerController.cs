using UnityEngine;
using System.Collections;

public class QQPlatformerController : MonoBehaviour 
{
    //private Vector2 currentTile;
    private Vector3 velocity;

    public float acceleration    = 0.01f;
    public float maxXSpeed       = 1.0f;
    public float maxYSpeed       = 1.0f;
    public float jumpSpeed       = 10.0f;
    public float gravity         = 0.001f;

    private bool grounded;
    private bool jumped;
    private QQLevelGenerator levelGenerator;

    private Transform floorCheck;
    private float radius;
    private float floorCheckOffset = 0.01f;
    private Vector3 MinYPos
    {
        get { return floorCheck.position; }
    }

    private float ledgeTolerance = 0.3f;

    //Debug
    public TileType currentTileType;

	void Start () 
    {
        levelGenerator = QQGameManager.Instance.LevelGenerator;
        floorCheck = transform.Find("BottomPos");

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

        if (transform.position.y < 0.5f)
            transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
    }

    private void CheckIfGrounded()
    {
        TileType tileTypeBelow = levelGenerator.TileTypeAtPosition(MinYPos - (Vector3.up * floorCheckOffset));

        currentTileType = tileTypeBelow;
        
        switch (tileTypeBelow)
        {
            case TileType.Empty:
                grounded = false;
                break;
            case TileType.Block:
                if (!jumped && velocity.y <= 0.0f)
                {
                    if ((Mathf.Abs(Mathf.Ceil(MinYPos.y) - MinYPos.y) <= ledgeTolerance))
                    {
                        grounded = true;
                        transform.position = new Vector3(transform.position.x, Mathf.Ceil(MinYPos.y) + radius, transform.position.z);
                    }
                }
                break;
        }
    }

    private void ApplyAcceleration()
    {
        if (velocity.x < maxXSpeed)
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
        {
            jumped = true;
            velocity.y += jumpSpeed * Time.deltaTime;
        }
    }

    private Vector3 LimitVel()
    {
        Vector3 vel = velocity;

        if (vel.x > maxXSpeed)
            vel.x = maxXSpeed;

        if (Mathf.Abs(vel.y) > maxYSpeed)
            vel.y = (vel.y >= 0) ? maxYSpeed : -maxYSpeed;

        return vel;
    }
}
