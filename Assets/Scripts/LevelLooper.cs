using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelLooper : MonoBehaviour 
{
	private GameObject tilePrefab;
	private TileType[,] tileStates = new TileType[30,10];

	private Stack<GameObject> tilePool;
	private int columnCounter;

//	public void DefineNewLevel(TileType[,] newTileStates)
//	{
//		tileStates = newTileStates;
//	}

	public void DefineNewLevel(Texture2D levelMap)
	{
		tileStates = LevelCreator.CreateLevel (levelMap);
	}

	public void GenerateNextColumn()
	{
		++columnCounter;

	}

	// Use this for initialization
	private void Start () 
	{
		tilePool = new Stack<GameObject> ();
	}
	
	// Update is called once per frame
	private void Update () 
	{
	
	}
}
