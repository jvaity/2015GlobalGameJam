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
        get
        {
            return floorCheck.position;
        }
    }
    private Vector3 MaxYPos
    {
        get
        {
            return transform.position + new Vector3(0, 0.5f, 0);
        }
    }

    private Vector3 MaxXPos
    {
        get { return transform.position + (Vector3.right * radius); }
    }

    //Tolerance to snapping on top of tiles or falling through them
    private float ledgeTolerance = 0.3f;

    private Vector2 previousGroundCheckPos;
    private TileType previousGroundCheckType;

    private Vector2 previousForwardCheckPos;
    private TileType previousForwardCheckType;

    private Vector2 previousUpCheckPos;
    private TileType previousUpCheckType;

    //Debug
    public TileType currentTileType;

	void Start () 
    {
        levelGenerator = QQGameManager.Instance.LevelGenerator;
        floorCheck = transform.Find("BottomPos");

        if (floorCheck != null)
            radius = Vector3.Distance(transform.position, floorCheck.position);

        //while (true)
        //{
        //    Move();
            //yield return null;
        //}
	}

    void FixedUpdate()
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
        CheckCollisionRight();
        CheckCollisionUp();

        ApplyAcceleration();
        ApplyGravity();
        Jump();

        velocity = LimitVel();

        transform.position += velocity;

        if (transform.position.y < 0.5f)
            transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);

        SnapPos();
    }

    private void CheckIfGrounded()
    {
        TileType tileTypeBelow = previousGroundCheckType = levelGenerator.CollideAtPosition(MinYPos, ref previousGroundCheckPos, previousGroundCheckType);

        currentTileType = tileTypeBelow;

        //Debug.Log("<b><color=red>Tile type below: " + tileTypeBelow + "</color></b>");

        switch (tileTypeBelow)
        {
            case TileType.Empty:
                grounded = false;
                break;
            case TileType.Block:
            case TileType.Coin:
                //if (!jumped && velocity.y <= 0.0f)
                //{
                    //if ((Mathf.Abs(Mathf.Ceil(MinYPos.y) - MinYPos.y) <= ledgeTolerance))
                    //{
                        grounded = true;
                        if (transform.position.y < previousGroundCheckPos.y + 1.4f)
                        {
                            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, previousGroundCheckPos.y + 1.4f, transform.position.z), 0.2f);
                        }
                    //}
                //}
                break;
        }

        //Debug while we have no kill floor
        if (transform.position.y <= 0.5f)
            grounded = true;
    }

    private void CheckCollisionUp()
    {
        previousUpCheckType = levelGenerator.CollideAtPosition(MaxYPos, ref previousUpCheckPos, previousUpCheckType, false, false);
    }

    private void CheckCollisionRight()
    {
        previousForwardCheckType = levelGenerator.CollideAtPosition(MaxXPos, ref previousForwardCheckPos, previousForwardCheckType, true);
    }

    private void ApplyAcceleration()
    {
        if (velocity.x < maxXSpeed)
            velocity.x += acceleration * Time.deltaTime;
    }

    private void ApplyGravity()
    {
        if (!grounded && previousForwardCheckType != TileType.Block && previousForwardCheckType != TileType.Coin)
        {
            velocity.y -= gravity * Time.deltaTime;
        }
        else
            velocity.y = 0.0f;

        //If falling again, set jumped to false so we can collide with the top of blocks again
        if (velocity.y <= 0.0f && jumped)
            jumped = false;
    }

    private void Jump()
    {
        if ((grounded || previousForwardCheckType == TileType.Block || previousForwardCheckType == TileType.Coin) && 
            (previousUpCheckType != TileType.Coin || previousUpCheckType != TileType.Block) && 
            Input.GetKeyDown(KeyCode.Space))
        {
            jumped = true;
            grounded = false;
            velocity.y = jumpSpeed;
            //Debug.Log("Jump:" + velocity.y);
        }

        if (velocity.y > 0 && (previousUpCheckType == TileType.Block || previousUpCheckType == TileType.Coin))
            velocity.y = 0.0f;
    }

    private Vector3 LimitVel()
    {
        Vector3 vel = velocity;

        if (vel.x > maxXSpeed)
            vel.x = maxXSpeed;

        if (Mathf.Abs(vel.y) > maxYSpeed)
            vel.y = (vel.y >= 0) ? maxYSpeed : -maxYSpeed;

        //if (jumped)
        //    print("JUMPED:: Limit Vel Y: " + vel.y);

        return vel;
    }

    private void SnapPos()
    {
        if (previousForwardCheckType == TileType.Block || previousForwardCheckType == TileType.Coin)
        {
            Vector3 snapPos = transform.position;
            snapPos.y = previousForwardCheckPos.y + 0.5f;
            transform.position = Vector3.Lerp(transform.position, snapPos, 0.1f);
        }
    }
}
