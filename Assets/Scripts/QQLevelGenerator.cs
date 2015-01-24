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
    private int deleteCounter = 0;

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

	public TileType CollideAtPosition(Vector3 pos, ref Vector2 previousPositionInGrid, TileType preiousTileType, bool inBlock = false)
	{
        
        if (pos.x < 0 || pos.y < 0)
            return TileType.Empty;

        Vector2 positionInGrid = new Vector2((int)pos.x % mapWidth, (int)pos.y % mapHeight);

        if (positionInGrid == previousPositionInGrid)
            return preiousTileType;
        previousPositionInGrid = positionInGrid;
        RVDrawer.QDrawCross(pos, Color.white, 1f, Vector3.forward);

        TileType type = tilesArray[(int)positionInGrid.x, (int)positionInGrid.y];

        Debug.Log("Is in Block: "+inBlock);

        switch (type)
        {
            case TileType.Empty:
                break;
            case TileType.Block:
                tilesArray[(int)positionInGrid.x, (int)positionInGrid.y] = TileType.Empty;
                if (inBlock)
                {
                    int index = ((int)positionInGrid.x * mapHeight) + (deleteCounter % mapHeight) + (int)positionInGrid.y;
                    QQTile tileToDelete = tileInstances[index];
                    if (tileToDelete != null)
                        GameObject.Destroy(tileToDelete.gameObject);
                }
                break;
            case TileType.Death:
                QQGameManager.Instance.GameOver();
                break;
            case TileType.Coin:
                if (inBlock)
                {
                    QQGameManager.Instance.CollectiblePickedUp();
                    tilesArray[(int)positionInGrid.x, (int)positionInGrid.y] = TileType.Empty;

                    int index = ((int)positionInGrid.x * mapHeight) + (deleteCounter % mapHeight) + (int)positionInGrid.y;
                    QQTile tileToDelete = tileInstances[index];
                    if (tileToDelete != null)
                        GameObject.Destroy(tileToDelete.gameObject);
                }
                break;
            case TileType.Spawn:
                break;
            default:
                break;
        }
			
		return type;
	}
	
	public IEnumerator ColumnGeneratorRoutine(int startingAmount = 0)
	{
		if (startingAmount > 0)
			GenerateColumns(0, startingAmount);
		
		int playerMapPos = 0;
		bool playerAheadOfColumnRef = false;
		int columnIndex = startingAmount;
		while (true) 
		{
			playerMapPos = (int)QQGameManager.Instance.PlatformController.transform.position.x;
			playerAheadOfColumnRef = playerMapPos > columnIndex - (mapWidth /2);
			Debug.Log(playerMapPos + ":" + columnIndex + ":" +  playerAheadOfColumnRef);
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
		
		int removalStartIdx = columnIndex * mapHeight;
		QQTile[] tiles = new QQTile[mapHeight];
		for (int i = removalStartIdx; i < removalStartIdx + mapHeight; i++)
			tiles[i] = tileInstances[i];
			
		tileInstances.RemoveRange(removalStartIdx, mapHeight);
        deleteCounter += mapHeight;
		
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

    public void Dispose()
    {
        foreach (QQTile tile in tileInstances)
        {
            GameObject.Destroy(tile.gameObject);
        }
    }
}
