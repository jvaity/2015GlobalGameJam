using UnityEngine;
using System.Collections;

public class QQTile : MonoBehaviour {

    [SerializeField]
    Sprite[] blockSprites;//, CollectibleSprite;
    [SerializeField]
    Color firstColour, secondColour;

    private Vector2 coordinates;
    private TileType currentType;

    private float dropAcceleration = 0.1f;
    private float maxDropSpeed = 2.0f;
    private float currentSpeed = 0.0f;

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

    public void DropBlock()
    {
        print("Drop Block");
        StartCoroutine(DropBlockRoutine());
    }

    private IEnumerator DropBlockRoutine()
    {
        //Change this to edit how long after the player hits it that it drops
        yield return new WaitForSeconds(0.5f);

        currentSpeed = 0.0f;

        while (transform.position.y > -2.0f)
        {
            if (currentSpeed < maxDropSpeed)
                currentSpeed += dropAcceleration;

            if (currentSpeed > maxDropSpeed)
                currentSpeed = maxDropSpeed;

            transform.position -= (Vector3.up * currentSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
