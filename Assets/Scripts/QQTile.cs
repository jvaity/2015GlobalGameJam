using UnityEngine;
using System.Collections;

public class QQTile : MonoBehaviour {

    [SerializeField]
    Sprite[] blockSprites;//, CollectibleSprite;
    [SerializeField]
    Color firstColour, secondColour;
    [SerializeField]
    AudioClip blockAudio, coinAudio;

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
	//Debug.Log(blockSprites.Length);
        switch (type)
        {
            case TileType.Empty:
                break;
            case TileType.Block:
                SetSprite(renderer, GetColour(), blockSprites[Random.Range(0, blockSprites.Length)]);
                break;
            case TileType.Death:
                SetSprite(renderer, Color.clear);
                break;
            case TileType.Coin:
				SetSprite(renderer, Color.cyan, blockSprites[Random.Range(0, blockSprites.Length)]);
                break;
            case TileType.Spawn:
                SetSprite(renderer, Color.clear);
                break;
            default:
                break;
        }
    }

	private Color GetColour()
	{
		return Color.Lerp(firstColour, secondColour, Random.Range(0f, 1f));
	}

    private void SetSprite(SpriteRenderer renderer, Color colour, Sprite sprite = null)
    {
        if (sprite == null)
            renderer.enabled = false;
        else
        {
            renderer.enabled = true;
            renderer.sprite = sprite;
            renderer.color = colour;
        }
    }

    public void playAudio(TileType type)
    {
        switch (type)
        {
            case TileType.Empty:
                break;
            case TileType.Block:
                AudioSource.PlayClipAtPoint(blockAudio, QQGameManager.Instance.PlatformController.transform.position);
                break;
            case TileType.Death:
                break;
            case TileType.Coin:
                AudioSource.PlayClipAtPoint(coinAudio, QQGameManager.Instance.PlatformController.transform.position);
                break;
            case TileType.Spawn:
                break;
            default:
                break;
        }
        
    }
}
