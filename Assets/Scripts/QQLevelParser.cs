using UnityEngine;
using System.Collections;

public class QQLevelParser {

    public static BreakableTile.TileType[,] ParseMap(Texture2D image)
    {
        BreakableTile.TileType[,] tiles = new BreakableTile.TileType[image.width, image.height];
        for (int y = 0; y < image.height; y++)
        {
            for (int x = 0; x < image.width; x++)
            {
                Color pixel = image.GetPixel(x, y);
                if (pixel == Color.red)
                {
                    tiles[x, y] = BreakableTile.TileType.Death;
                }
                else if (pixel == Color.green)
                {
                    tiles[x, y] = BreakableTile.TileType.Block;
                }
                else if (pixel == Color.blue)
                {
                    tiles[x, y] = BreakableTile.TileType.Coin;
                }
                else if (pixel == Color.black)
                {
                    //tiles[x, y] = BreakableTile.TileType.Spawn;
                }
                else
                    tiles[x, y] = BreakableTile.TileType.Empty;
            }
        }
        return tiles;
    }
}
