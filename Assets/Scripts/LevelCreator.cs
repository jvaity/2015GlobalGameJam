using UnityEngine;
using System.Collections;

public class LevelCreator : MonoBehaviour {

    [SerializeField]
    private GameObject tilePrefab;

	private object[][] CreateLevel(Texture2D image)
    {

        object[][] tiles = new object[image.height][];
        for (int y = 0; y < image.height; y++)
        {
            tiles[y] = new object[image.width];
            for (int x = 0; x < image.width; x++)
            {
                Color pixel = image.GetPixel(x, y);
                if (pixel.r == 1)
                {
                    tiles[y][x] = new object();
                }
                else if (pixel.g == 1)
                {
                    tiles[y][x] = new object();
                }
                else if (pixel.b == 1)
                {
                    tiles[y][x] = new object();
                }
                else
                    tiles[y][x] = new object();
            }
        }
        return tiles;
    }
}
