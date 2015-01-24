using UnityEngine;
using System.Collections;

public class QQLevelParser {

    public static TileType[,] ParseMap(Texture2D image)
    {
        TileType[,] tiles = new TileType[image.width, image.height];
        for (int y = 0; y < image.height; y++)
        {
            for (int x = 0; x < image.width; x++)
            {
                Color pixel = image.GetPixel(x, y);
                if (pixel == Color.red)
                {
                    tiles[x, y] = TileType.Death;
                }
                else if (pixel == Color.green)
                {
                    tiles[x, y] = TileType.Block;
                }
                else if (pixel == Color.blue)
                {
                    tiles[x, y] = TileType.Coin;
                }
                else if (pixel == Color.black)
                {
                    tiles[x, y] = TileType.Spawn;
                }
                else
                    tiles[x, y] = TileType.Empty;
            }
        }
        return tiles;
    }
}
