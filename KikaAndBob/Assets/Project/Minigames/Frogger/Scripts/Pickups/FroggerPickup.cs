using UnityEngine;
using System.Collections;

public class FroggerPickup : FroggerLaneItem 
{
	protected override void EnterSurfaceEffect (FroggerCharacter character)
	{
		GetComponent<BoxCollider2D>().enabled = false;

		foreach (Renderer r in GetComponentsInChildren<Renderer>())
		{
			r.enabled = false;	
		}

		FroggerGameManager.use.IncreasePickupCount(1);
	}
}
