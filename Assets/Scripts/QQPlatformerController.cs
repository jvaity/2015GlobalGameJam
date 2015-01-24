using UnityEngine;
using System.Collections;

public class QQPlatformerController : MonoBehaviour 
{
    //private Vector2 currentTile;
    private Vector3 velocity;

    public float acceleration   = 0.2f;
    public float maxSpeed       = 5.0f;
    public float jumpSpeed      = 10.0f;
    public float gravity        = 2.0f;

    private bool grounded;
    private CircleCollider2D circleCollider;

    private QQLevelGenerator levelGenerator;

    private Vector3 MinYPos
    {
        get { return transform.position - (Vector3.up * circleCollider.radius); }
    }

    private Vector3 MaxXPos
    {
        get { return transform.position + (Vector3.right * circleCollider.radius); }
    }

	// Use this for initialization
	void Start () 
    {
        circleCollider = GetComponent<CircleCollider2D>();
        levelGenerator = QQGameManager.Instance.LevelGenerator;
	}
	
	// Update is called once per frame
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
        TileType tileTypeBelow = levelGenerator.TileTypeAtPosition(MinYPos);

        switch (tileTypeBelow)
        {
            case TileType.Empty:
                grounded = false;
                break;
            case TileType.Block:
                grounded = true;
                //DeleteOnNextLoop();
                transform.position = new Vector3(transform.position.x, Mathf.CeilToInt(MinYPos.y) + circleCollider.radius, transform.position.z);
                break;
            case TileType.Death:
                //Die();
                break;
            case TileType.Coin:
                //Collect();
                break;
            case TileType.Spawn:
                break;
        }
    }

    private void CheckCollisionForward()
    {
        TileType tileTypeForward = levelGenerator.TileTypeAtPosition(MaxXPos);

        switch (tileTypeForward)
        {
            case TileType.Block:
                grounded = true;
                transform.position = new Vector3(transform.position.x, Mathf.CeilToInt(MinYPos.y) + circleCollider.radius, transform.position.z);
                break;
            case TileType.Coin:
                //Collect();
                break;
            case TileType.Spawn:
                break;
        }
    }

    //private void EatTile()
    //{
    //    //Eat Sprite/Anim

    //    //Remove Block on GameManager
    //    QQGameManager.Instance.RemoveBlock(true);
    //}

    //private void Collect()
    //{
    //    //Collect Coin on GameManager
    //    QQGameManager.Instance.PickUpCollectible();
    //}

    //private void DeleteOnNextLoop()
    //{
    //    QQGameManager.Instance.RemoveBlock(false);
    //}

    //private void Die()
    //{
    //    velocity = Vector3.zero;
    //    QQGameManager.Instance.GameOver();
    //}
}
