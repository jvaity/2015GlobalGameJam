using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelLooper : MonoBehaviour 
{
	[SerializeField]
	private Texture2D levelMap;
	[SerializeField]
	private GameObject tilePrefab;
	[SerializeField]
	private float generationFrequency = 0.2f;
	[SerializeField]
	private bool deleteTiles;

    private BreakableTile.TileType[,] tileStates = new BreakableTile.TileType[30, 10];
	//private GameObject[,] tileInstances = new GameObject[30, 10];
	private Queue<GameObject> objectDeletionQueue;

	private Stack<GameObject> tilePool;
	private int columnCounter;

//	public void DefineNewLevel(TileType[,] newTileStates)
//	{
//		tileStates = newTileStates;
//	}

	public void DefineNewLevel(Texture2D newlevelMap)
	{
		levelMap = newlevelMap;
		tileStates = LevelCreator.CreateLevel (newlevelMap);

		GameObject newTile;
		Vector3 offset = new Vector3 (0.5f, 0.5f);
		for (int x = 0; x < levelMap.width; x++) 
		{
			for (int y = 0; y < levelMap.height; y++) 
			{
				BreakableTile tileScript;

				switch (tileStates[x,y]) 
				{
					case BreakableTile.TileType.Empty:
						break;
					case BreakableTile.TileType.Block:
						newTile = Instantiate(tilePrefab) as GameObject;
						tileScript = newTile.GetComponent<BreakableTile>();
						newTile.transform.position = new Vector3(x,y) + offset;
						objectDeletionQueue.Enqueue(newTile);
						break;
					case BreakableTile.TileType.Death:
						newTile = Instantiate(tilePrefab) as GameObject;
						tileScript = newTile.GetComponent<BreakableTile>();
						newTile.transform.position = new Vector3(x,y) + offset;
						newTile.renderer.material.color = Color.red;
						objectDeletionQueue.Enqueue(newTile);
						break;
					case BreakableTile.TileType.Coin:
						newTile = Instantiate(tilePrefab) as GameObject;
						tileScript = newTile.GetComponent<BreakableTile>();
						newTile.transform.position = new Vector3(x,y) + offset;
						newTile.renderer.material.color = Color.yellow;
						objectDeletionQueue.Enqueue(newTile);
					break;
				}
			}
		}
	}

	public void GenerateNextColumn()
	{
		BreakableTile.TileType[] newColumnTypes = new BreakableTile.TileType[levelMap.height];
		int x = columnCounter % levelMap.width;
		for (int y = 0; y < levelMap.height; y++) 
		{
			newColumnTypes[y] = tileStates[x,y];
		}

		GameObject newTile;
		BreakableTile tileScript;
		Vector3 offset = new Vector3 (0.5f, 0.5f);
		for (int y = 0; y < newColumnTypes.Length; y++) 
		{
			switch (tileStates[x,y]) 
			{
				case BreakableTile.TileType.Empty:
					objectDeletionQueue.Enqueue(null);
					break;
				case BreakableTile.TileType.Block:
					newTile = Instantiate(tilePrefab) as GameObject;
					tileScript = newTile.GetComponent<BreakableTile>();
					newTile.transform.position = new Vector3(columnCounter,y) + offset;
					objectDeletionQueue.Enqueue(newTile);
					break;
				case BreakableTile.TileType.Death:
					newTile = Instantiate(tilePrefab) as GameObject;
					tileScript = newTile.GetComponent<BreakableTile>();
					newTile.transform.position = new Vector3(columnCounter,y) + offset;
					newTile.renderer.material.color = Color.red;
					objectDeletionQueue.Enqueue(newTile);
					break;
				case BreakableTile.TileType.Coin:
					newTile = Instantiate(tilePrefab) as GameObject;
					tileScript = newTile.GetComponent<BreakableTile>();
					newTile.transform.position = new Vector3(columnCounter,y) + offset;
					newTile.renderer.material.color = Color.yellow;
					objectDeletionQueue.Enqueue(newTile);
					break;
			}
		}

		++columnCounter;
	}

	private void DeleteTrailingColumn()
	{
		for (int i = 0; i < levelMap.height; i++) 
		{
			if (objectDeletionQueue.Peek() != null)
				DestroyImmediate(objectDeletionQueue.Dequeue());
			else
				objectDeletionQueue.Dequeue();
		}
	}

	// Use this for initialization
	private IEnumerator Start () 
	{
		tilePool = new Stack<GameObject> ();
		objectDeletionQueue = new Queue<GameObject> ();

		if (levelMap != null)
			DefineNewLevel (levelMap);

		columnCounter = levelMap.width;

		while (true) 
		{
			yield return new WaitForSeconds(generationFrequency);
			GenerateNextColumn();
			if (deleteTiles)
				DeleteTrailingColumn ();
		}

	}
}
