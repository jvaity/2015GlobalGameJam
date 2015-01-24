using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QQLevelGenerator : MonoBehaviour
{
	private  Vector3 TILE_POS_OFFSET = new Vector3(0.5f, 0.5f);
	private TileType[,] tilesArray;
	private Texture2D textureMap;
	private int mapWidth, mapHeight;
	private GameObject tilePrefab;
	private List<QQTile> tileInstances;

	public TileType TileTypeAtPosition(Vector3 pos)
	{
		return tilesArray[(int)pos.x % mapWidth, (int)pos.y % mapHeight];
	}

	public void Init (Texture2D texMap, GameObject tile) 
	{
		textureMap = texMap;
		tilesArray = QQLevelParser.ParseMap (texMap);
		mapWidth = textureMap.width;
		mapHeight = textureMap.height;
		tilePrefab = tile;
		tileInstances = new List<QQTile>();
		
		
	}

	private IEnumerator ColumnGeneratorRoutine()
	{
		yield return null;
	}

	private void GenerateColumn(int columnIndex)
	{
		TileType[] typeSettings = new TileType[mapHeight];
		for (int y = 0; y < mapHeight; y++) 
		{
			typeSettings[y] = tilesArray[columnIndex % mapWidth, y];
			tileInstances.Add(QQTile.CreateTile(new Vector2(columnIndex, y), typeSettings[y]));
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
		for (int i = removalStartIdx; i < mapHeight; i++)
			tiles[i] = tileInstances[i];
		tileInstances.RemoveRange(removalStartIdx, mapHeight);
	}

	private void UpdateTypeArray(Vector2 coordinates, TileType newType)
	{
		if (coordinates.x < mapWidth && coordinates.y < mapHeight && coordinates.x > 0f && coordinates.y > 0f) 
		{
			tilesArray[(int)coordinates.x, (int)coordinates.y] =  newType;
		}
	}
}
