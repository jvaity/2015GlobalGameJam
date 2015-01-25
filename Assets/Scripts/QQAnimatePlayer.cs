using UnityEngine;
using System.Collections;

public class QQAnimatePlayer : MonoBehaviour 
{
	public SpriteRenderer spriteRenderer;
	public Sprite sprite1, sprite2;
	public float waitTime;

	// Use this for initialization
	IEnumerator Start () {
		while (true) 
		{
			if (spriteRenderer.sprite == sprite1)
				spriteRenderer.sprite = sprite2;
			else
				spriteRenderer.sprite = sprite1;
				
			yield return new WaitForSeconds(waitTime);
		}
	}
}
