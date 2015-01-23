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
	}

	private void OnTrigger2DExit(Collider2D collider)
	{
		if (collider.CompareTag ("Player")) 
		{
			this.gameObject.SetActive(false);
		}

	}
}
