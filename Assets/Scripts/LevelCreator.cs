using UnityEngine;
using System.Collections;

public class LevelCreator : MonoBehaviour {

    [SerializeField]
    private GameObject tilePrefab;

	private BreakableTile.TileType[][] CreateLevel(Texture2D image)
    {

        BreakableTile.TileType[][] tiles = new BreakableTile.TileType[image.height][];
        for (int y = 0; y < image.height; y++)
        {
            tiles[y] = new BreakableTile.TileType[image.width];
            for (int x = 0; x < image.width; x++)
            {
                Color pixel = image.GetPixel(x, y);
                if (pixel.r == 1)
                {
                    tiles[y][x] = BreakableTile.TileType.Death;
                }
                else if (pixel.g == 1)
                {
                    tiles[y][x] = BreakableTile.TileType.Block;
                }
                else if (pixel.b == 1)
                {
                    tiles[y][x] = BreakableTile.TileType.Coin;
                }
                else
                    tiles[y][x] = BreakableTile.TileType.Empty;
            }
        }
        return tiles;
    }
}
