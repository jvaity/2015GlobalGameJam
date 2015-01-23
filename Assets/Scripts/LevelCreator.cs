using UnityEngine;
using System.Collections;

public class LevelCreator {

    [SerializeField]
    private GameObject tilePrefab;

	public static BreakableTile.TileType[,] CreateLevel(Texture2D image)
    {

        BreakableTile.TileType[,] tiles = new BreakableTile.TileType[image.width, image.height];
        for (int y = 0; y < image.height; y++)
        {
            for (int x = 0; x < image.width; x++)
            {
                Color pixel = image.GetPixel(x, y);
                if (pixel.r == 1)
                {
                    tiles[x,y] = BreakableTile.TileType.Death;
                }
                else if (pixel.g == 1)
                {
                    tiles[x,y] = BreakableTile.TileType.Block;
                }
                else if (pixel.b == 1)
                {
                    tiles[x,y] = BreakableTile.TileType.Coin;
                }
                else
                    tiles[x,y] = BreakableTile.TileType.Empty;
            }
        }
        return tiles;
    }
}
