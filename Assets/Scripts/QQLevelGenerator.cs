using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QQLevelGenerator
{
	private  Vector3 TILE_POS_OFFSET = new Vector3(0.5f, 0.5f);
	private TileType[,] tilesArray;
	private Texture2D textureMap;
	private int mapWidth, mapHeight;
	private List<QQTile> tileInstances;

	public int MapHeight
	{
		get { return mapHeight; }
	}
	
	public int MapWidth
	{
		get { return mapWidth; }
	}

	public QQLevelGenerator (Texture2D texMap) 
	{
		textureMap = texMap;
		tilesArray = QQLevelParser.ParseMap (texMap);
		mapWidth = textureMap.width;
		mapHeight = textureMap.height;
		tileInstances = new List<QQTile>();
		
		//StartCoroutine(ColumnGeneratorRoutine(mapWidth));
	}

	public TileType TileTypeAtPosition(Vector3 pos)
	{
		if (tilesArray == null || tilesArray.GetLength(0) < pos.x || tilesArray.GetLength(1) < pos.y)
			return TileType.Empty;
			
		return tilesArray[(int)pos.x % mapWidth, (int)pos.y % mapHeight];
	}
	
	public IEnumerator ColumnGeneratorRoutine(int startingAmount = 0)
	{
		if (startingAmount > 0)
			GenerateColumns(0, startingAmount);
		
		int playerPos = (int)QQGameManager.Instance.PlatformController.transform.position.x;
		int playerMapPos = 0;
		bool playerAheadOfColumnRef = false;
		int columnIndex = startingAmount;
		while (true) 
		{
			playerMapPos = playerPos % mapWidth;
			playerAheadOfColumnRef = playerMapPos > columnIndex % mapWidth;
		
			if (playerAheadOfColumnRef)
			{
				GenerateColumn(columnIndex++);
				DeleteColumn(0);
			}	
			yield return null;//new WaitForSeconds(0.2f);
		}
	}

	private void GenerateColumn(int columnIndex)
	{
		TileType[] typeSettings = new TileType[mapHeight];
		for (int y = 0; y < mapHeight; y++) 
		{
			typeSettings[y] = tilesArray[columnIndex % mapWidth, y];
			tileInstances.Add(QQTile.CreateTile(new Vector3(columnIndex, y) + TILE_POS_OFFSET, typeSettings[y]));
		}
	}
	
	private void GenerateColumns(int startColumnIndex, int count)
	{
		for (int i = startColumnIndex; i < startColumnIndex + count; i++)
			GenerateColumn(i);
	}

	private void DeleteColumn(int columnIndex)
	{
		// Early exit if column tiles don't exist
		if (columnIndex > (tileInstances.Count / mapHeight))
			return;
		
		int removalStartIdx = columnIndex / mapHeight;
		QQTile[] tiles = new QQTile[mapHeight];
		for (int i = removalStartIdx; i < removalStartIdx + mapHeight; i++)
			tiles[i] = tileInstances[i];
			
		tileInstances.RemoveRange(removalStartIdx, mapHeight);
		
		for (int i = 0; i < tiles.Length; i++) 
		{
			if (tiles[i] != null)
				GameObject.Destroy(tiles[i].gameObject);
		}
	}

	private void UpdateTypeArray(Vector2 coordinates, TileType newType)
	{
		if (coordinates.x < mapWidth && coordinates.y < mapHeight && coordinates.x > 0f && coordinates.y > 0f) 
		{
			tilesArray[(int)coordinates.x, (int)coordinates.y] =  newType;
		}
	}
}
