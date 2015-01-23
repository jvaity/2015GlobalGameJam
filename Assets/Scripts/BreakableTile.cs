using UnityEngine;
using System.Collections;

public class BreakableTile : MonoBehaviour 
{
	public bool willThisTileFuckYouUp;

	private void OnTrigger2DExit(Collider2D collider)
	{
		if (collider.CompareTag ("Player")) 
		{
			this.gameObject.SetActive(false);
		}

	}
}
