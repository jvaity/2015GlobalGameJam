//using UnityEngine;
//using System.Collections;
//
//public class BreakableTile : MonoBehaviour 
//{
//	public enum TileType
//	{
//		Empty,
//		Block,
//		Death,
//		Coin
//	}
// 
//	public TileType type;
//	public Vector2 coordinate;
//	public LevelLooper looper;
//
//	public TileType Type
//	{
//		get { return type; }
//		set 
//		{
//			ChangeTileType(value);
//		}
//	}
//
//	public void Init(LevelLooper looper, Vector2 coords)
//	{
//		this.looper = looper;
//		coordinate = coords;
//	}
//
//	private void ChangeTileType(TileType newType)
//	{
//		type = newType;
//		this.gameObject.tag = type.ToString();
//		looper.RegisterTileState (newType, coordinate);
//	}
//
//	private void OnTriggerEnter2D(Collider2D collider)
//	{
//		if (collider.CompareTag ("Player")) 
//		{
//			switch (type) 
//			{
//				case TileType.Empty:
//					break;
//				case TileType.Block:
//					break;
//				case TileType.Death:
//					break;
//				case TileType.Coin:
//                    GameManager.Instance.collect();
//				this.renderer.material.color = Color.green;
//					break;
//			}
//		}
//
//	}
//
//	private void OnCollisionEnter2D(Collision2D col)
//	{
//		if (col.gameObject.tag == ("Player"))
//		{
//			switch (type)
//			{
//			case TileType.Empty:
//				//Shouldn't be colliding
//				break;
//			case TileType.Block:
//				//Kill block
//				this.renderer.material.color = Color.blue;
//				break;
//			case TileType.Death:
//				//Die
//				break;
//			case TileType.Coin:
//				break;
//			}
//		}
//	}
//	
//	private void OnCollisionExit2D(Collision2D col)
//	{
//		if (col.gameObject.tag == ("Player"))
//		{
//			switch (type)
//			{
//			case TileType.Empty:
//				//Shouldn't be colliding
//				break;
//			case TileType.Block:
//				//Kill block
//                    ChangeTileType(TileType.Empty);
//					this.renderer.material.color = Color.cyan;
//                    break;
//                case TileType.Death:
//                    //Die
//                    break;
//                case TileType.Coin:
//                    break;
//            }
//        }
//    }
//}
