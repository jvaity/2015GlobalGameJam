using UnityEngine;
using System.Collections;

public class QQTile : MonoBehaviour {

    [SerializeField]
    Sprite blockSprite, CollectibleSprite;

    private Vector2 coordinates;
    private TileType currentType;

    public TileType CurrentType
    {
        get
        {
            return currentType;
        }
    }

	public static QQTile CreateTile (Vector2 coordinates, TileType type)
    {
        if (type == TileType.Empty)
            return null;

        GameObject tileObject = Instantiate(QQGameManager.Instance.TilePrefab) as GameObject;
        QQTile tile = tileObject.GetComponent<QQTile>();
        tile.Init(type);
        tile.transform.position = coordinates;
        return tile;
    }

    public void Init(TileType type)
    {
        this.currentType = type;

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        switch (type)
        {
            case TileType.Empty:
                break;
            case TileType.Block:
                SetSprite(renderer, blockSprite);
                break;
            case TileType.Death:
                SetSprite(renderer);
                break;
            case TileType.Coin:
                SetSprite(renderer, CollectibleSprite);
                break;
            case TileType.Spawn:
                SetSprite(renderer);
                break;
            default:
                break;
        }
    }

    private void SetSprite(SpriteRenderer renderer, Sprite sprite = null)
    {
        if (sprite == null)
            renderer.enabled = false;
        else
        {
            renderer.enabled = true;
            renderer.sprite = sprite;
        }
    }
}
