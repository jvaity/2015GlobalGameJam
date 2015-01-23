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

	private TileType Type
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

	private void OnTrigger2DExit(Collider2D collider)
	{
		if (collider.CompareTag ("Player")) 
		{
			switch (type) 
			{
				case TileType.Empty:
					//Shouldn't be colliding
					break;
				case TileType.Block:
					//Kill block
					ChangeTileType (TileType.Empty);
					break;
				case TileType.Death:
					//Die
					break;
				case TileType.Coin:
					//GetCoin()
					break;
			}
		}

	}
}
