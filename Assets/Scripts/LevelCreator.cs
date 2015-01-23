using UnityEngine;
using System.Collections;

public class LevelCreator : MonoBehaviour {

	private GameObject CreateLevel(Texture2D image)
    {
        Transform parent = new GameObject("Level").transform;
        for (int y = 0; y < image.height; y++)
        {
            for (int x = 0; x < image.width; x++)
            {
                Color pixel = image.GetPixel(x, y);
                if (pixel == Color.white)
                    continue;
                else if (pixel.r == 1)
                {
                    GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Quad);
                }
                else if (pixel.g == 1)
                {
                    GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    tile.renderer.enabled = false;
                }
            }
        }
        return parent.gameObject;
    }
}
