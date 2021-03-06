﻿using UnityEngine;
using System.Collections;

public class QQPlatformerController : MonoBehaviour 
{
    //private Vector2 currentTile;
    private Vector3 velocity;

    public float acceleration    = 0.01f;
    public float maxXSpeed       = 1.0f;
    public float maxYSpeed       = 1.0f;
    public float jumpSpeed       = 0.1f;
    public float gravity         = 0.001f;
    public float maxDrillSpeed   = 1.5f;

    private bool grounded;
    private bool jumped;
    private bool jumpButtonDown;
    //private QQLevelGenerator levelGenerator;
    private float jumpBoost;
    private float jumpBoostMax = 0.05f;

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
        //levelGenerator = QQGameManager.Instance.LevelGenerator;
        floorCheck = transform.Find("BottomPos");

        if (floorCheck != null)
            radius = Vector3.Distance(transform.position, floorCheck.position);

        //while (true)
        //{
        //    Move();
            //yield return null;
        //}
	}

    //void Update()
    //{
    //    Move();
    //}

    public void Spawn(Vector3 position)
    {
        velocity = Vector3.zero;
        transform.position = position;
        previousForwardCheckPos = previousGroundCheckPos = previousUpCheckPos = new Vector3 (-1,-1,-1);
        previousUpCheckType = previousGroundCheckType = previousForwardCheckType = TileType.Empty;
        jumpButtonDown = false;
        jumpBoost = 0;
        grounded = false;
        jumped = false;
    }

    public void Move()
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
        TileType tileTypeBelow = previousGroundCheckType = QQGameManager.Instance.LevelGenerator.CollideAtPosition(MinYPos, ref previousGroundCheckPos, previousGroundCheckType);

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
                        if (transform.position.y < previousGroundCheckPos.y + 1.5f)
                        {
                            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, previousGroundCheckPos.y + 1.5f, transform.position.z), 0.2f);
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
		previousUpCheckType = QQGameManager.Instance.LevelGenerator.CollideAtPosition(MaxYPos, ref previousUpCheckPos, previousUpCheckType, false, false);
        
    }

    private void CheckCollisionRight()
    {
		previousForwardCheckType = QQGameManager.Instance.LevelGenerator.CollideAtPosition(MaxXPos, ref previousForwardCheckPos, previousForwardCheckType, true);
    }

    private void ApplyAcceleration()
    {
            velocity.x += acceleration * Time.deltaTime;
    }

    private void ApplyGravity()
    {
        if (!grounded)
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
        	jumpButtonDown = true;
            jumped = true;
            grounded = false;
            velocity.y = jumpSpeed;
            //Debug.Log("Jump:" + velocity.y);
        }
        
        if (jumpButtonDown)
        {
        	if (Input.GetKeyUp(KeyCode.Space) || grounded)
        	{
        		jumpBoost = 0;
        		jumpButtonDown = false;
        	}
        	else
        	{
				jumpBoost = jumpBoostMax * Time.deltaTime * 5f;
				if (jumpBoost > jumpBoostMax)
				{
					jumpButtonDown = false;
					jumpBoost = 0;
				}
				else
	        		velocity.y += jumpBoostMax * Time.deltaTime;
        	}
        }

        if (velocity.y > 0 && (previousUpCheckType == TileType.Block || previousUpCheckType == TileType.Coin))
            velocity.y = 0.0f;
    }

    private Vector3 LimitVel()
    {
        Vector3 vel = velocity;

        if (previousForwardCheckType == TileType.Block || previousForwardCheckType == TileType.Coin)
        {
            if (vel.x > maxDrillSpeed)
                vel.x = maxDrillSpeed;
        }
        else if (vel.x > maxXSpeed)
            vel.x = Mathf.Lerp(vel.x, maxXSpeed, 0.1f);

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
            if (snapPos.y < previousForwardCheckPos.y + 0.5f)
            {
                snapPos.y = previousForwardCheckPos.y + 0.5f;
                transform.position = Vector3.Lerp(transform.position, snapPos, 0.1f);
                velocity.y = velocity.y < 0 ? 0 : velocity.y;
            }
        }
    }
}
