using UnityEngine;
using System.Collections;

public class BreakableTile : MonoBehaviour 
{
	public enum TileType
	{
		Empty,
		Block,
		Death,
		Coin
	}
 
	private TileType type;

	public TileType Type
	{
		get { return type; }
		set 
		{
			ChangeTileType(value);
		}
	}

	private void ChangeTileType(TileType newType)
	{
		type = newType;
		this.gameObject.tag = type.ToString();
	}

	private void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.CompareTag ("Player")) 
		{
			switch (type) 
			{
				case TileType.Empty:
					break;
				case TileType.Block:
					break;
				case TileType.Death:
					break;
				case TileType.Coin:
                    GameManager.Instance.collect();
					break;
			}
		}

	}

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == ("Player"))
        {
            switch (type)
            {
                case TileType.Empty:
                    //Shouldn't be colliding
                    break;
                case TileType.Block:
                    //Kill block
                    ChangeTileType(TileType.Empty);
                    break;
                case TileType.Death:
                    //Die
                    break;
                case TileType.Coin:
                    break;
            }
        }
    }
}
